using UnityEngine;
using UnityEngine.SceneManagement;

public class PortInteraction : MonoBehaviour
{
    [Header("이 항구의 데이터 연결")]
    public CityData cityData;

    // 플레이어와 충돌(상호작용) 감지
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (cityData == TravelSceneController.targetCity)
            {
                Debug.Log($"{cityData.cityName}에 도착했습니다! (목표 일치)");
                ArriveAtPort();
            }
            else
            {
                // 잘못된 항구에 도착했을 때의 처리
                Debug.LogWarning($"잘못된 항구에 도착했습니다! 목표는 {TravelSceneController.targetCity.cityName} 입니다.");

            }
        }
    }

    void ArriveAtPort()
    {
        TravelManager.Instance.FinishTravel(cityData, 1);
        SceneManager.LoadScene(0);
    }
}