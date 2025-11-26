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

    // 플레이어가 이동을 시작하는 함수 (씬 전환 로직 포함)
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

        // 항해 씬 컨트롤러에 데이터 전달
        TravelSceneController.targetCity = destinationCity;
        TravelSceneController.daysToTravel = route.travelTimeInDays;

        // SailingScene 로드
        SceneManager.LoadScene(1);

        Debug.Log($"[항해 시작] {startCity.cityName}에서 {destinationCity.cityName}로 이동 시작. {route.travelTimeInDays}일 소요 예정.");

        return true;
    }

    // 도착 처리 및 매니저 업데이트 함수 (항해 씬 종료 시 TravelSceneController에 의해 호출됨)
    public void FinishTravel(CityData destinationCity, int travelDays)
    {
        // 시간 경과
        currentDay += travelDays;

        // 도시 위치 갱신
        TradeManager.Instance.currentCity = destinationCity;

        Debug.Log($"[항해 완료] {destinationCity.cityName}에 도착. 현재 {currentDay}일째.");
    }
}