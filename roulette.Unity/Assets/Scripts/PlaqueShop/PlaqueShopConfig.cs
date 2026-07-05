using UnityEngine;

[CreateAssetMenu(fileName = "PlaqueShopConfig", menuName = "Configs/PlaqueShopConfig")]
public class PlaqueShopConfig : ScriptableObject
{
    [field : SerializeField] public ShopItemsRarityGroup Plaques { get; private set; }
}
