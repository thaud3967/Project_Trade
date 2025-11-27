using UnityEngine;
using TMPro;
using System.Collections.Generic; 

[System.Serializable]
public struct CitySpawnInfo
{
    public CityData city;       
    public Transform spawnPoint; 
}

public class TravelSceneController : MonoBehaviour
{
    // 이전 씬에서 받아온 목표 도시 (표시용)
    public static CityData targetCity;
    public static int daysToTravel;

    [Header("플레이어 배 연결")]
    public GameObject playerShip; 

    [Header("시작 위치 설정")]
    public List<CitySpawnInfo> spawnPoints; 

    [Header("UI 연결")]
    public TextMeshProUGUI infoText;

    void Start()
    {
        if (targetCity != null && infoText != null)
        {
            infoText.text = $"목표: {targetCity.cityName}로 직접 항해하세요!";
        }

        // 현재 도시에 맞춰 배 시작 위치 옮기기
        if (TradeManager.Instance != null && TradeManager.Instance.currentCity != null)
        {
            CityData startCity = TradeManager.Instance.currentCity;

            // 리스트를 뒤져서 현재 도시와 일치하는 스폰 위치를 찾음
            foreach (var info in spawnPoints)
            {
                if (info.city == startCity)
                {
                    if (playerShip != null && info.spawnPoint != null)
                    {
                        playerShip.transform.position = info.spawnPoint.position;
                        Debug.Log($"[위치 설정] {startCity.cityName} 앞바다에서 시작합니다.");
                    }
                    break; 
                }
            }
        }
    }
}