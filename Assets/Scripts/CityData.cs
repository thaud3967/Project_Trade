using UnityEngine;

[CreateAssetMenu(fileName = "New City", menuName = "TradeGame/City")]
public class CityData : ScriptableObject
{
    public string cityName;

    [Header("특산품 설정")]
    public ItemData specialtyItem; // 이 도시의 특산품
    public float priceMultiplier = 0.8f; // 특산품은 80% 가격에 팜
}