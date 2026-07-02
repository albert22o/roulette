public class PlaqueShop
{
    private ShopItemsRarityGroup items;
    private BossChips bossChips;

    public PlaqueShop(PlaqueShopConfig config, BossChips bossChips)
    {
        this.items = config.Plaques;
        this.bossChips = bossChips;
    }

    public bool TrySpendBossChip()
    {
        if (bossChips.Amount >= 1)
        {
            bossChips.Remove(1);
            return true;
        }
        return false;
    }

    public PlaqueData[] GetThreeRandomPlaques()
    {
        PlaqueData[] result = new PlaqueData[3];
        for (int i = 0; i < 3; i++)
        {
            result[i] = items.GetRandomItem() as PlaqueData;
        }
        return result;
    }
}
