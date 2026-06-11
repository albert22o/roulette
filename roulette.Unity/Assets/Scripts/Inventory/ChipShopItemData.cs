using UnityEngine;

[CreateAssetMenu(fileName = "ChipShopItemData", menuName = "Scriptable Objects/ChipShopItemData")]
public class ChipShopItemData : ItemData
{
    [SerializeField] private ChipData chipData;
    [SerializeField] private int cost;


    public ChipData ChipData => chipData;
    public int Cost => cost;
}
