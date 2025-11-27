using UnityEngine;

public class TradeTabController : MonoBehaviour
{
    public GameObject buyPanel;
    public GameObject sellPanel;
    public InventoryController inventoryController; // InventoryController 연결

    void Start()
    {
        // 시작 시 구매 패널만 활성화
        ShowBuyPanel();
    }

    public void ShowBuyPanel()
    {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
    }

    public void ShowSellPanel()
    {
        sellPanel.SetActive(true);
        buyPanel.SetActive(false);

        // 판매 패널을 열 때마다 인벤토리 목록을 갱신합니다.
        if (inventoryController != null)
        {
            inventoryController.RefreshInventorySlots();
        }
    }
}