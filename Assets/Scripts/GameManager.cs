using System.Collections.Generic;
using UnityEngine;

// JSON 데이터 파싱용 클래스 (껍데기)
[System.Serializable]
public class JsonItemData
{
    public int id;
    public string name;
    public int price;
}

[System.Serializable]
public class JsonWrapper
{
    public List<JsonItemData> items;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("플레이어 상태")]
    public int money = 1000;
    // 인벤토리: <아이템데이터, 개수>
    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    [Header("데이터베이스")]
    // 에디터에서 만든 모든 ItemData SO를 여기에 등록해둬야 함
    public List<ItemData> allItemSOs;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // 씬 이동해도 파괴 안 됨

        // 게임 시작 시 JSON 데이터 로드하여 SO에 적용
        LoadGameData();
    }

    // 1. JSON 데이터를 읽어서 ScriptableObject 값을 업데이트 (과제 핵심)
    void LoadGameData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("items"); // items.json 읽기

        if (jsonFile != null)
        {
            JsonWrapper wrapper = JsonUtility.FromJson<JsonWrapper>(jsonFile.text);

            foreach (JsonItemData jsonItem in wrapper.items)
            {
                // 리스트에서 ID가 같은 SO를 찾음
                ItemData targetSO = allItemSOs.Find(x => x.id == jsonItem.id);

                if (targetSO != null)
                {
                    // JSON에 적힌 값으로 게임 데이터를 덮어씌움 (데이터 주도 설계)
                    targetSO.itemName = jsonItem.name;
                    targetSO.basePrice = jsonItem.price;
                    Debug.Log($"[데이터 갱신] {targetSO.itemName} 가격을 {targetSO.basePrice}원으로 설정 완료.");
                }
            }
        }
        else
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다.");
        }
    }

    // 2. 아이템 구매/획득 로직
    public void AddItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += count;
        }
        else
        {
            inventory.Add(item, count);
        }
        Debug.Log($"{item.itemName} {count}개 획득. 현재 {inventory[item]}개");
    }

    // 3. 아이템 판매/제거 로직
    public bool RemoveItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item) && inventory[item] >= count)
        {
            inventory[item] -= count;
            if (inventory[item] <= 0) inventory.Remove(item);
            return true; // 제거 성공
        }
        return false; // 개수 부족
    }
}