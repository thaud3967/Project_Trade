using System.Collections.Generic;
using UnityEngine;

// JSON 데이터 파싱용
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
    public int money = 5000;
    // 인벤토리: <아이템데이터, 개수>
    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    [Header("데이터베이스")]
    public List<ItemData> allItemSOs;

    void Awake()
    {
        // 싱글톤 패턴 구현 및 DDOL
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동해도 파괴 안 됨
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // JSON 데이터를 읽어서 ScriptableObject 값을 업데이트
    void LoadGameData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("items");

        if (jsonFile != null)
        {
            JsonWrapper wrapper = JsonUtility.FromJson<JsonWrapper>(jsonFile.text);

            foreach (JsonItemData jsonItem in wrapper.items)
            {
                ItemData targetSO = allItemSOs.Find(x => x.id == jsonItem.id);

                if (targetSO != null)
                {
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

    // 아이템 구매/획득 로직
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
        Debug.Log($"{item.itemName} {count}개 획득. 현재 총 개수: {inventory[item]}개");

        // 획득 후 UI 갱신 통보
        NotifyTradeOccurred();
    }

    // 아이템 판매/제거 로직
    public bool RemoveItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item) && inventory[item] >= count)
        {
            inventory[item] -= count;
            if (inventory[item] <= 0) inventory.Remove(item);
            NotifyTradeOccurred();
            return true; // 제거 성공
        }
        return false; // 개수 부족
    }

    // 거래가 발생할 때마다 UI 갱신을 호출
    public void NotifyTradeOccurred()
    {
        // UIManager가 존재하면 상태를 갱신하도록 명령
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdatePlayerStatus();
        }
    }
}