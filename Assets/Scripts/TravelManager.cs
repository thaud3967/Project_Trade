using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class TravelManager : MonoBehaviour
{
    public static TravelManager Instance;

    [Header("전체 경로 목록")]
    public List<RouteData> allRoutes; // 인스펙터에 SO 할당 필요

    [Header("현재 게임 시간")]
    public int currentDay = 1;

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
    }

    // 두 도시 사이의 경로를 찾는 헬퍼 함수
    public RouteData GetRoute(CityData start, CityData end)
    {
        return allRoutes.Find(r =>
            (r.startCity == start && r.endCity == end) ||
            (r.startCity == end && r.endCity == start) // 양방향 검색
        );
    }

    // 플레이어가 이동을 시작하는 함수 (씬 전환 로직 추가 예정)
    public bool StartTravel(CityData destinationCity)
    {
        CityData startCity = TradeManager.Instance.currentCity;
        RouteData route = GetRoute(startCity, destinationCity);

        if (route == null)
        {
            Debug.LogError($"경로를 찾을 수 없습니다: {startCity.cityName} -> {destinationCity.cityName}");
            return false;
        }

        if (GameManager.Instance.money < route.travelCost)
        {
            Debug.Log("여행 비용이 부족합니다.");
            return false;
        }

        // 비용 지불
        GameManager.Instance.money -= route.travelCost;
        UIManager.Instance.UpdatePlayerStatus();

        TravelSceneController.targetCity = destinationCity;
        TravelSceneController.daysToTravel = route.travelTimeInDays;

        SceneManager.LoadScene(1);

        Debug.Log($"[항해 시작] {startCity.cityName}에서 {destinationCity.cityName}로 이동 시작.");

        // 여기서 SceneManager.LoadScene("SailingScene") 호출
        // 지금은 임시로 바로 이동 처리.

        FinishTravel(destinationCity, route.travelTimeInDays);

        return true;
    }

    // 도착 처리 및 매니저 업데이트 함수 (항해 씬 종료 시 호출될 예정)
    public void FinishTravel(CityData destinationCity, int travelDays)
    {
        // 시간 경과 및 유지비 지불 (유지비 로직 추가 예정)
        currentDay += travelDays;

        // 도시 위치 갱신
        TradeManager.Instance.currentCity = destinationCity;

        // 물가 변동 로직 호출 (추가 예정)

        // 씬 전환 (도시 씬 로드)

        Debug.Log($"[항해 완료] {destinationCity.cityName}에 도착. 현재 {currentDay}일째.");
    }
}