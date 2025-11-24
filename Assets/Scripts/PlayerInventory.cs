using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public int money = 1000;
    public int tradeCount = 0;

    // 아이템과 수량 관리
    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    void Awake() { Instance = this; }

    public void AddItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item)) inventory[item] += count;
        else inventory.Add(item, count);
    }

    public bool HasItem(ItemData item)
    {
        return inventory.ContainsKey(item) && inventory[item] > 0;
    }
}