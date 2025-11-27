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

    // 게임 클리어 변수 및 조건 
    [Header("게임 클리어")]
    public int winMoneyAmount = 50000; // 목표 금액
    private bool hasWon = false; // 클리어 상태 (한 번 달성하면 유지)


    void Awake()
    {
        // 싱글톤 패턴 구현 및 DDOL (기존 코드)
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

    //  money 변경 처리 함수 
    public bool ChangeMoney(int amount)
    {
        // money 부족 시 거래 실패 방지
        if (money + amount < 0)
        {
            Debug.LogWarning("Gold가 부족하여 거래 실패!");
            return false;
        }

        money += amount;

        // money가 변경될 때마다 클리어 조건을 검사합니다. 
        CheckWinCondition();

        NotifyTradeOccurred();
        return true;
    }

    // 클리어 조건 검증 함수 
    public void CheckWinCondition()
    {
        if (hasWon) return; // 이미 클리어했으면 무시

        if (money >= winMoneyAmount)
        {
            hasWon = true;
            Debug.Log($" 게임 클리어! {winMoneyAmount} money 달성! ");
        }
    }
}