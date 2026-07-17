using UnityEngine;

[CreateAssetMenu(fileName = "BossRewardConfig", menuName = "Configs/BossRewardConfig")]
public class BossRewardConfig : ScriptableObject
{
    [field : SerializeField] public ShopItemsTable GeneralMultiplierCardsTable{ get; private set; }
    [field: SerializeField] public ShopItemsTable OtherItemsTable { get; private set; }
    [field: SerializeField] public int BossChipsCount { get; private set; } = 1;
}
