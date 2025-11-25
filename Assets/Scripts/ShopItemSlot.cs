using UnityEngine;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    public ItemData item; // 이 슬롯이 표시하는 아이템 데이터
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;

    // 상점 초기화 시 외부에서 호출
    public void Setup(ItemData itemData, int price)
    {
        item = itemData;
        nameText.text = item.itemName;
        priceText.text = $"{price}G";
    }

    // 구매 버튼에 연결될 함수
    public void OnBuyButtonClicked()
    {
        const int quantity = 1; // MVP는 1개씩만 구매

        if (TradeManager.Instance.TryToBuy(item, quantity))
        {
            // 구매 성공 시 UI 메시지 (나중에 추가)
        }
        else
        {
            // 구매 실패 시 UI 메시지 (나중에 추가)
        }
    }
}