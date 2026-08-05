using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings/Settings")]

public class Settings : ScriptableObject
{
    public CurrenciesConfig CurrenciesConfig;
    public TableConfig TableConfig;
    public RoundsConfig RoundsConfig;
    public BallHolderConfig BallHolderConfig;
    public ChipHolderConfig ChipHolderConfig;
    public PlaqueShopConfig PlaqueShopConfig;
    public BossRewardConfig BossRewardConfig;
    public RarityData RarityData;
    public ShopFontConfig ShopFontConfig;
}
