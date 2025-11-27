using UnityEngine;
using TMPro;

public class InventoryItemSlot : MonoBehaviour
{
    private ItemData currentItem;
    private int currentSellPrice;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI priceText;

    public void Setup(ItemData itemData, int quantity, int sellPrice)
    {
        currentItem = itemData;
        currentSellPrice = sellPrice;

        nameText.text = itemData.itemName;
        quantityText.text = $"보유: {quantity}개";
        priceText.text = $"{sellPrice}G";
    }

    // 판매 버튼에 연결할 함수
    public void OnSellButtonClicked()
    {
        const int quantity = 1; // 1개씩 판매 가정

        if (TradeManager.Instance.TryToSell(currentItem, quantity))
        {
            // 판매 성공 시: UI 갱신을 위해 InventoryController의 새로고침 호출
            GetComponentInParent<InventoryController>().RefreshInventorySlots();
        }
        else
        {
            // 판매 실패 (개수 부족 등)
            Debug.Log("판매 실패!");
        }
    }
}