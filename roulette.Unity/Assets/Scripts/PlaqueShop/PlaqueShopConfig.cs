using UnityEngine;

[CreateAssetMenu(fileName = "PlaqueShopConfig", menuName = "Scriptable Objects/PlaqueShopConfig")]
public class PlaqueShopConfig : ScriptableObject
{
    [field : SerializeField] public ShopItemsRarityGroup Plaques { get; private set; }
}
