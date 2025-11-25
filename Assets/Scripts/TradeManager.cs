using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance;

    // 현재 플레이어가 있는 도시를 에디터에서 직접 할당합니다.
    [Header("현재 도시 설정")]
    public CityData currentCity;

    // 플레이어가 물건을 팔 때 도시가 사주는 기본 마진율 (1.5배)
    private const float GenericBuybackMultiplier = 1.5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 플레이어가 살 때의 가격 (도시의 판매 가격)
    public int CalculateBuyPrice(ItemData item)
    {
        // Null 검사 : currentCity가 없으면 정가 판매 (안정화)
        if (currentCity == null) return item.basePrice;

        // 특산품일 경우 CityData의 할인율(0.8)을 적용
        if (item == currentCity.specialtyItem)
        {
            return Mathf.RoundToInt(item.basePrice * currentCity.priceMultiplier);
        }
        // 특산품이 아니면 정가 판매 (1.0)
        return item.basePrice;
    }

    // 플레이어가 팔 때의 가격 (도시의 매입 가격)
    public int CalculateSellPrice(ItemData item)
    {
        // Null 검사 : currentCity가 없으면 일반 물건으로 간주하여 1.5배 매입 (안정화)
        if (currentCity == null) return Mathf.RoundToInt(item.basePrice * GenericBuybackMultiplier);

        // 이 도시의 특산품을 되팔 경우 (50% 헐값에 매입)
        if (item == currentCity.specialtyItem)
        {
            return Mathf.RoundToInt(item.basePrice * 0.5f);
        }
        else
        {
            // 다른 도시의 특산품이므로 비싸게 매입 (1.5배)
            return Mathf.RoundToInt(item.basePrice * GenericBuybackMultiplier);
        }
    }

    // TryToBuy, TryToSell 함수는 그대로 유지됩니다.
    public bool TryToBuy(ItemData item, int quantity)
    {
        int cost = CalculateBuyPrice(item) * quantity;
        if (GameManager.Instance.money >= cost)
        {
            GameManager.Instance.money -= cost;
            GameManager.Instance.AddItem(item, quantity);
            Debug.Log($"[구매 성공] {(currentCity != null ? currentCity.cityName : "미정 도시")}에서 {item.itemName} {quantity}개 구매 (비용: {cost}G)");
            return true;
        }
        Debug.Log("[구매 실패] 돈이 부족합니다.");
        return false;
    }

    public bool TryToSell(ItemData item, int quantity)
    {
        int revenue = CalculateSellPrice(item) * quantity;

        if (!GameManager.Instance.RemoveItem(item, quantity))
        {
            Debug.Log("[판매 실패] 인벤토리에 해당 물건이 부족합니다.");
            return false;
        }

        GameManager.Instance.money += revenue;

        Debug.Log($"[판매 성공] {(currentCity != null ? currentCity.cityName : "미정 도시")}에 {item.itemName} {quantity}개 판매 (수익: {revenue}G)");
        return true;
    }
}