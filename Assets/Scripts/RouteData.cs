using UnityEngine;

[CreateAssetMenu(fileName = "New Route", menuName = "TradeGame/Route")]
public class RouteData : ScriptableObject
{
    [Header("시작/도착 도시")]
    public CityData startCity;
    public CityData endCity;

    [Header("여행 정보")]
    public int travelTimeInDays = 3; // 이동에 걸리는 날짜
    public int travelCost = 500;    // 이동에 드는 비용
}