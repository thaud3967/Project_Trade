using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TravelSceneController : MonoBehaviour
{
    // 이동 시 TradeManager에서 임시로 저장해둔 데이터
    public static CityData targetCity;
    public static int daysToTravel;

    [Header("UI 연결")]
    public TextMeshProUGUI dayCounterText;
    public Slider progressBar;

    private int elapsedDays = 0;
    private float timePerDay = 1.0f; // 1초당 1일이 흐르게 설정

    void Start()
    {
        if (targetCity == null || daysToTravel <= 0)
        {
            Debug.LogError("여행 데이터가 없습니다. 도시 씬으로 복귀합니다.");
            // 씬 0번이 도시 씬이라고 가정하고 복귀
            SceneManager.LoadScene(0);
            return;
        }

        dayCounterText.text = $"항해 중... {elapsedDays} / {daysToTravel}일";
        InvokeRepeating("PassDay", timePerDay, timePerDay); // 1초마다 PassDay 함수 호출
    }

    void PassDay()
    {
        elapsedDays++;

        // UI 업데이트
        dayCounterText.text = $"항해 중... {elapsedDays} / {daysToTravel}일";
        progressBar.value = (float)elapsedDays / daysToTravel;

        if (elapsedDays >= daysToTravel)
        {
            CancelInvoke("PassDay"); // 반복 호출 중지
            FinishTravelSequence();
        }
    }

    void FinishTravelSequence()
    {
        // TravelManager에게 최종 도착 처리를 명령합니다.
        TravelManager.Instance.FinishTravel(targetCity, daysToTravel);

        // 도시 씬(Scene 0번)으로 돌아갑니다.
        // 현재 도시 씬의 빌드 인덱스가 0번이라고 가정합니다.
        SceneManager.LoadScene(0);
    }
}