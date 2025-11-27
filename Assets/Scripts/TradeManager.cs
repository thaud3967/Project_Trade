using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance;
    [Header("UI 연결")]
    public TextMeshProUGUI currentCityTextUI;
    public Image backgroundImage;

    [Header("현재 도시 설정")]
    // 인스펙터에서 초기 도시 SO를 할당하기 위한 전용 필드 (Inspector에 보임)
    [SerializeField]
    private CityData initialCityData;

    private static CityData savedCurrentCity;

    private CityData _currentCity;

    public CityData currentCity
    {
        get { return _currentCity; }
        set
        {
            _currentCity = value;

            savedCurrentCity = value;
            // 도시가 변경될 때마다 UI를 업데이트합니다. (현재 위치 텍스트 갱신)
            UpdateCityUI();

            // 상점 UI 갱신 활성화: 도시가 변경될 때마다 상점 목록을 다시 불러옵니다.
            if (ShopController.Instance != null)
            {
                ShopController.Instance.LoadShopItems();
            }
        }
    }

    // 플레이어가 물건을 팔 때 도시가 사주는 기본 마진율
    private const float GenericBuybackMultiplier = 1.5f;

    void Start()
    {
        // 게임 시작 시 초기 UI 상태 설정
        UpdateCityUI();

        // 씬이 로드된 직후 상점 목록을 강제로 갱신합니다.
        if (ShopController.Instance != null)
        {
            ShopController.Instance.LoadShopItems();
        }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 게임 시작 시 인스펙터에 할당된 초기 데이터를 currentCity 속성에 대입합니다.
        if (savedCurrentCity != null)
        {
            // 속성을 통해 대입해야 UI 갱신 로직이 실행됨
            currentCity = savedCurrentCity;
        }
        else if (initialCityData != null)
        {
            currentCity = initialCityData;
        }

        // 초기 UI 상태 설정
        UpdateCityUI();
    }

    private void UpdateCityUI()
    {
        if (currentCityTextUI != null && _currentCity != null)
        {
            currentCityTextUI.text = $"현재 위치: {_currentCity.cityName}";

            if (backgroundImage != null && _currentCity.cityBackground != null)
            {
                backgroundImage.sprite = _currentCity.cityBackground;
            }
        }
        else if (currentCityTextUI != null)
        {
            currentCityTextUI.text = "현재 위치: 로딩 중...";
        }
    }


    // 플레이어가 살 때의 가격 (도시의 판매 가격)
    public int CalculateBuyPrice(ItemData item)
    {
        if (_currentCity == null) return item.basePrice;

        float price = item.basePrice;

        // 특산품 할인 적용
        if (item == _currentCity.specialtyItem)
        {
            price *= _currentCity.priceMultiplier;
        }

        // 랜덤 변동폭 적용
        float randomAdjustment = Random.Range(-_currentCity.priceVolatility, _currentCity.priceVolatility);
        price *= (1f + randomAdjustment);

        return Mathf.RoundToInt(price);
    }

    // 플레이어가 팔 때의 가격 (도시의 매입 가격)
    public int CalculateSellPrice(ItemData item)
    {
        if (_currentCity == null) return Mathf.RoundToInt(item.basePrice * GenericBuybackMultiplier);

        float price = item.basePrice;

        // 특산품 헐값 매입 또는 비싸게 매입 적용
        if (item == _currentCity.specialtyItem)
        {
            price *= 0.5f;
        }
        else
        {
            price *= GenericBuybackMultiplier;
        }

        // 랜덤 변동폭 적용
        float randomAdjustment = Random.Range(-_currentCity.priceVolatility, _currentCity.priceVolatility);
        price *= (1f + randomAdjustment);

        return Mathf.RoundToInt(price);
    }

    public bool TryToBuy(ItemData item, int quantity)
    {
        int cost = CalculateBuyPrice(item) * quantity;

        // GameManager의 money를 변경하는 함수를 호출하고 돈이 충분한지 확인합니다.
        if (GameManager.Instance.ChangeMoney(-cost))
        {
            GameManager.Instance.AddItem(item, quantity);
            return true;
        }
        return false;
    }

    public bool TryToSell(ItemData item, int quantity)
    {
        int revenue = CalculateSellPrice(item) * quantity;

        if (!GameManager.Instance.RemoveItem(item, quantity))
        {
            return false;
        }

        // GameManager의 money를 변경하는 함수를 호출합니다.
        GameManager.Instance.ChangeMoney(revenue);
        return true;
    }

}