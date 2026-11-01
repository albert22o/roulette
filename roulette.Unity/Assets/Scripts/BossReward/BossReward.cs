using System;
using System.Collections.Generic;
using System.Linq;

public class BossReward
{
    private BossRewardConfig config;
    private ItemData[] items = new ItemData[5];
    private ShopItemsPool cardsPool, otherItemsPool;
    private GameStateManager gameStateManager;
    private BossChips bossChips;
    private ChipHolder chipHolder;
    private BallHolder ballHolder;

    public event Action<ItemData[]> RewardCreated;
    public event Action BossChipsGiven;

    public BossReward(BossRewardConfig config, GameStateManager gameStateManager, BossChips bossChips,
        BallHolder ballHolder, ChipHolder chipHolder)
    {
        this.config = config;
        cardsPool = new ShopItemsPool(config.GeneralMultiplierCardsTable);
        otherItemsPool = new ShopItemsPool(config.OtherItemsTable);
        this.gameStateManager = gameStateManager;
        this.bossChips = bossChips;
        this.ballHolder = ballHolder;
        this.chipHolder = chipHolder;
    }

    public void GiveReward()
    {
        HashSet<ItemData> chosenItems = new HashSet<ItemData>();
        for (int i = 0; i < 3; i++)
        {
            ItemData card = GetValidItemFromPool(config.GeneralMultiplierCardsTable, chosenItems);
            items[i] = card;
            if (card != null) chosenItems.Add(card);
        }

        for (int i = 3; i < 5; i++)
        {
            ItemData otherItem = GetValidItemFromPool(config.OtherItemsTable, chosenItems);
            items[i] = otherItem;
            if (otherItem != null) chosenItems.Add(otherItem);
        }
        RewardCreated?.Invoke(items);
        gameStateManager.ChangeGameState(GameStateManager.GameState.BossReward);
    }

    private ItemData GetValidItemFromPool(ShopItemsTable table, HashSet<ItemData> excludedItems)
    {
        if (table == null || table.Tables == null || table.Tables.Length == 0) return null;

        var validElements = new List<ItemsTableElement>();
        float totalWeight = 0f;

        foreach (var element in table.Tables)
        {
            if (element == null || element.RarityGroup == null || element.RarityGroup.Items == null) continue;

            bool hasAllowedItems = element.RarityGroup.Items.Any(item => IsItemAllowed(item) && !excludedItems.Contains(item));
            if (hasAllowedItems)
            {
                validElements.Add(element);
                totalWeight += element.Rarity;
            }
        }

        if (validElements.Count == 0) return null;

        ShopItemsRarityGroup selectedGroup = null;
        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var element in validElements)
        {
            cumulative += element.Rarity;
            if (roll <= cumulative)
            {
                selectedGroup = element.RarityGroup;
                break;
            }
        }

        if (selectedGroup == null) selectedGroup = validElements.Last().RarityGroup;

        var allowedItemsInGroup = selectedGroup.Items
            .Where(item => IsItemAllowed(item) && !excludedItems.Contains(item))
            .ToList();

        if (allowedItemsInGroup.Count == 0) return null;

        int randomIndex = UnityEngine.Random.Range(0, allowedItemsInGroup.Count);
        return allowedItemsInGroup[randomIndex];
    }

    private bool IsItemAllowed(ItemData item)
    {
        if (item == null) return false;

        if (item is BallInfoData && ballHolder.FreeSlotsCount <= 0)
        {
            return false;
        }

        if (item is ChipData && chipHolder.FreeSlotCount <= 0)
        {
            return false;
        }

        return true;
    }

    public void GiveBossChips()
    {
        bossChips.Add(config.BossChipsCount);
        BossChipsGiven?.Invoke();
    }
}
