using UnityEngine;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public Transform contentParent; // 판매 슬롯이 들어갈 부모 오브젝트

    void OnEnable()
    {
        RefreshInventorySlots(); // 패널 활성화 시 목록 갱신
    }

    public void RefreshInventorySlots()
    {
        //  기존 슬롯 모두 삭제
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (GameManager.Instance == null) return;

        // GameManager의 인벤토리 데이터(Dictionary) 순회
        foreach (var pair in GameManager.Instance.inventory)
        {
            ItemData item = pair.Key;
            int quantity = pair.Value;

            // 보유량이 0이상이면 슬롯 생성
            if (quantity > 0)
            {
                int sellPrice = TradeManager.Instance.CalculateSellPrice(item);

                GameObject slotObj = Instantiate(inventorySlotPrefab, contentParent);
                InventoryItemSlot slot = slotObj.GetComponent<InventoryItemSlot>();

                // 슬롯 설정 함수 호출
                slot.Setup(item, quantity, sellPrice);
            }
        }
    }
}