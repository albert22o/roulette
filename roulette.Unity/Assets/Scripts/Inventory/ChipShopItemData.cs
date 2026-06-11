using UnityEngine;

[CreateAssetMenu(fileName = "ChipShopItemData", menuName = "Scriptable Objects/ChipShopItemData")]
public class ChipShopItemData : ItemData
{
    [SerializeField] private ChipData chipData;

    public ChipData ChipData => chipData;   
}
