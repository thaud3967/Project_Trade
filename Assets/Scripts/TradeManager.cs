using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance;

    // 현재 플레이어가 있는 도시를 에디터에서 직접 할당합니다.
    [Header("현재 도시 설정")]
    // 이 값은 TravelManager의 FinishTravel() 함수에서 갱신됩니다.
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
        if (currentCity == null) return item.basePrice;

        float price = item.basePrice;

        // 특산품 할인 적용
        if (item == currentCity.specialtyItem)
        {
            price *= currentCity.priceMultiplier; // 0.8f 할인
        }

        // 랜덤 변동폭 적용 (-Volatility ~ +Volatility)
        // CityData에 추가한 priceVolatility 변수 사용
        float randomAdjustment = Random.Range(-currentCity.priceVolatility, currentCity.priceVolatility);
        price *= (1f + randomAdjustment);

        // 최종 가격 정수 반환
        return Mathf.RoundToInt(price);
    }

    // 플레이어가 팔 때의 가격 (도시의 매입 가격)
    public int CalculateSellPrice(ItemData item)
    {
        if (currentCity == null) return Mathf.RoundToInt(item.basePrice * GenericBuybackMultiplier);

        float price = item.basePrice;

        // 특산품 헐값 매입 적용 (0.5f) 또는 비싸게 매입 (1.5f)
        if (item == currentCity.specialtyItem)
        {
            price *= 0.5f;
        }
        else
        {
            price *= GenericBuybackMultiplier; // 1.5f
        }

        // 랜덤 변동폭 적용 (여기도 적용)
        float randomAdjustment = Random.Range(-currentCity.priceVolatility, currentCity.priceVolatility);
        price *= (1f + randomAdjustment);

        // 최종 가격 정수 반환
        return Mathf.RoundToInt(price);
    }

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