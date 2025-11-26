using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MoveSelectionController : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject travelButtonPrefab;
    public Transform contentParent; // MoveSelectionPanel 내부의 Content/Layout Group

    [Header("연결 패널")]
    // 상점 창이 켜지지 않도록 참조
    public GameObject shopPanel;

    // 이동 메뉴를 열 때 호출
    void OnEnable()
    {
        // 상점 패널 비활성화
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        RefreshAvailableRoutes();
    }

    public void RefreshAvailableRoutes()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (TradeManager.Instance == null || TravelManager.Instance == null) return;

        CityData currentCity = TradeManager.Instance.currentCity;
        List<RouteData> allRoutes = TravelManager.Instance.allRoutes;

        // 이미 버튼을 만든 목적지를 추적할 HashSet 
        HashSet<CityData> availableDestinations = new HashSet<CityData>();

        // 현재 도시를 포함하는 모든 경로를 순회
        foreach (RouteData route in allRoutes)
        {
            CityData destinationCity = null;

            // 현재 도시가 시작 도시라면, 도착 도시가 목적지
            if (route.startCity == currentCity)
            {
                destinationCity = route.endCity;
            }
            // 현재 도시가 도착 도시라면, 시작 도시가 목적지
            else if (route.endCity == currentCity)
            {
                destinationCity = route.startCity;
            }

            if (destinationCity != null)
            {
                // 중복 목적지인지 확인하고 건너뜁니다.
                if (availableDestinations.Contains(destinationCity))
                {
                    continue;
                }
                availableDestinations.Add(destinationCity);

                // 목적지 도시로 가는 직접적인 경로(StartCity -> DestinationCity)를 찾습니다.
                // 이 경로에서 정확한 비용과 시간을 가져와야 합니다.
                RouteData finalRoute = TravelManager.Instance.allRoutes.Find(r =>
                    r.startCity == currentCity && r.endCity == destinationCity);

                // 버튼 생성 및 설정
                GameObject buttonObj = Instantiate(travelButtonPrefab, contentParent);

                // 버튼 텍스트 설정
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null && finalRoute != null)
                {
                    // 소요 일수
                    buttonText.text = $"Go to {destinationCity.cityName} ({finalRoute.travelCost}G, {finalRoute.travelTimeInDays}일)";
                }
                else if (buttonText != null)
                {
                    buttonText.text = $"Go to {destinationCity.cityName} (Error: No Direct Route)";
                }

                // 버튼 이벤트 연결
                UnityEngine.UI.Button button = buttonObj.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnTravelButtonClick(destinationCity));
                }
            }
        }
    }

    // 버튼 클릭 시 TravelManager 호출 및 패널 닫기
    public void OnTravelButtonClick(CityData destination)
    {
        if (TravelManager.Instance.StartTravel(destination))
        {
            // 이동 시작에 성공하면 패널 닫기 (자동으로 SailingScene으로 전환될 것임)
            gameObject.SetActive(false);
        }
        // 실패 시 (돈 부족 등) 패널은 그대로 열려있음
    }

    // 메뉴 닫기 버튼용 (UI에 연결)
    public void ClosePanel()
    {
        // 이동 패널 닫기
        gameObject.SetActive(false);

        // 상점 패널 다시 활성화
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
    }
}