using UnityEngine;

public class ShopController : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public Transform contentParent; // 아이템 슬롯이 들어갈 부모 오브젝트

    void OnEnable()
    {
        // 패널이 활성화될 때마다 상점 목록 새로고침
        RefreshShop();
    }

    public void RefreshShop()
    {
        // 안전장치 : TradeManager가 아직 준비 안 됐으면 중단
        if (TradeManager.Instance == null)
        {
            Debug.LogWarning("TradeManager가 아직 없습니다.");
            return;
        }

        // 안전장치 : 현재 도시 설정이 안 되어 있으면 중단
        if (TradeManager.Instance.currentCity == null)
        {
            Debug.LogWarning("TradeManager에 Current City가 할당되지 않았습니다!");
            return;
        }

        // 기존 아이템 슬롯 삭제 (새로고침을 위해)
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 현재 도시의 특산품만 판매
        ItemData specialty = TradeManager.Instance.currentCity.specialtyItem;

        if (specialty != null)
        {
            // 가격 계산
            int price = TradeManager.Instance.CalculateBuyPrice(specialty);

            // 아이템 슬롯 생성
            GameObject slotObj = Instantiate(itemSlotPrefab, contentParent);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            // 슬롯 정보 설정
            slot.Setup(specialty, price);
        }
    }
}