using System;

public class BossReward
{
    private BossRewardConfig config;
    private ItemData[] items = new ItemData[5];
    private ShopItemsPool cardsPool, otherItemsPool;
    private GameStateManager gameStateManager;
    private BossChips bossChips;

    public event Action<ItemData[]> RewardCreated;

    public BossReward(BossRewardConfig config, GameStateManager gameStateManager, BossChips bossChips)
    {
        this.config = config;
        cardsPool = new ShopItemsPool(config.GeneralMultiplierCardsTable);
        otherItemsPool = new ShopItemsPool(config.OtherItemsTable);
        this.gameStateManager = gameStateManager;
        this.bossChips = bossChips;
    }

    public void GiveReward()
    {
        gameStateManager.ChangeGameState(GameStateManager.GameState.BossReward);
        items[0] = cardsPool.GetRandomItem();
        items[1] = cardsPool.GetRandomItem();
        items[2] = cardsPool.GetRandomItem();

        items[3] = otherItemsPool.GetRandomItem();
        items[4] = otherItemsPool.GetRandomItem();
        RewardCreated?.Invoke(items);
    }

    public void GiveBossChips()
    {
        bossChips.Add(config.BossChipsCount);
    }
}
