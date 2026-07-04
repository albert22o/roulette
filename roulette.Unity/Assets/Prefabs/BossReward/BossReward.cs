public class BossReward
{
    private BossRewardConfig config;
    private ItemData[] items = new ItemData[5];
    private ShopItemsPool cardsPool, otherItemsPool;

    public BossReward(BossRewardConfig config)
    {
        this.config = config;
        cardsPool = new ShopItemsPool(config.GeneralMultiplierCardsTable);
        otherItemsPool = new ShopItemsPool(config.OtherItemsTable);
    }

    public ItemData[] GetItems()
    {
        items[0] = cardsPool.GetRandomItem();
        items[1] = cardsPool.GetRandomItem();
        items[2] = cardsPool.GetRandomItem();

        items[3] = otherItemsPool.GetRandomItem();
        items[4] = otherItemsPool.GetRandomItem();
        return items;
    }
}
