using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "TradeGame/Item")]
public class ItemData : ScriptableObject
{
    [Header("데이터 연결용 ID")]
    public int id; // JSON의 "id"와 일치해야 함

    [Header("게임 내 정보")]
    public string itemName;
    public int basePrice;
    public Sprite icon; // 이미지는 JSON으로 못 넣으니 여기서 직접 지정
    [TextArea] public string description;
}