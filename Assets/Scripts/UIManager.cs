using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("플레이어 상태 UI")]
    public TextMeshProUGUI goldText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdatePlayerStatus();
    }

    // 돈이 바뀔 때마다 호출할 함수
    public void UpdatePlayerStatus()
    {
        // Null 검사 : GameManager가 존재하지 않으면 여기서 멈춤
        if (GameManager.Instance == null) return;

        // GameManager에서 현재 돈을 가져와 텍스트를 갱신
        goldText.text = $"Gold: {GameManager.Instance.money}G";
    }
}