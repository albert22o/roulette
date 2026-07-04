using System;

public class BossReward
{
    private BossRewardConfig config;
    private ItemData[] items = new ItemData[5];
    private ShopItemsPool cardsPool, otherItemsPool;

    public event Action<ItemData[]> RewardCreated;

    public BossReward(BossRewardConfig config)
    {
        this.config = config;
        cardsPool = new ShopItemsPool(config.GeneralMultiplierCardsTable);
        otherItemsPool = new ShopItemsPool(config.OtherItemsTable);
    }



    public void GiveReward()
    {
        items[0] = cardsPool.GetRandomItem();
        items[1] = cardsPool.GetRandomItem();
        items[2] = cardsPool.GetRandomItem();

        items[3] = otherItemsPool.GetRandomItem();
        items[4] = otherItemsPool.GetRandomItem();
        RewardCreated?.Invoke(items);
    }
}
