using UnityEngine;

[CreateAssetMenu(fileName = "BossRewardConfig", menuName = "Scriptable Objects/BossRewardConfig")]
public class BossRewardConfig : ScriptableObject
{
    [field : SerializeField] public ShopItemsTable GeneralMultiplierCardsTable{ get; private set; }
    [field: SerializeField] public ShopItemsTable OtherItemsTable { get; private set; }
}
