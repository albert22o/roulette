using UnityEngine;

[CreateAssetMenu(fileName = "ShopConfig", menuName = "Configs/ShopConfig")]
public class ShopConfig : ScriptableObject
{
    [Header("Cost")]
    [SerializeField] private int foodCost = 10;

    [Header("Item tables")]
    [SerializeField] private ShopItemsTable chipsTable;

    [Header("Ball machine")]
    [SerializeField] private int ballCost;
    [SerializeField] private int ballMachineUseCount;

    [Header("Sell")]
    [SerializeField] private float chipSellMultiplier = 0.5f;
    [SerializeField] private int ballSellCost = 3;

    public int FoodCost => foodCost;
    public ShopItemsTable ChipsTable => chipsTable; 
    public int BallCost => ballCost;
    public int BallMachineUseAfterBoss => ballMachineUseCount;
    public float ChipSellMultiplier => chipSellMultiplier;
    public int BallSellCost => ballSellCost;

}
