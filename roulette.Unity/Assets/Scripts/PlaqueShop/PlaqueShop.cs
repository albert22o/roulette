﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaqueShop
{
    public int Cost { get; private set; } = 1;
    public bool CanBuy => bossChips.AvailableAmount >= Cost;

    public event Action PlaquesBought;
    private BossChips bossChips;
    private ShopItemsRarityGroup itemsGroup;
    private TableInfo tableInfo;

    public PlaqueShop(PlaqueShopConfig config, BossChips bossChips, TableInfo tableInfo)
    {
        itemsGroup = config.Plaques;
        this.bossChips = bossChips;
        this.tableInfo = tableInfo;
    }

    public bool TrySpendBossChip()
    {
        if (bossChips.AvailableAmount >= 1)
        {
            bossChips.Remove(1);
            PlaquesBought?.Invoke();
            return true;
        }
        return false;
    }

    public PlaqueData[] GetThreeRandomPlaques()
    {
        List<PlaqueData> availableToUsePlaques = new();
        foreach(var item in itemsGroup.Items)
        {
            if (item is PlaqueData plaqueData)
            {
                Debug.Log(item);
                if (plaqueData.CanUse(tableInfo.PocketMapping))
                    availableToUsePlaques.Add(plaqueData);
            }
        }
        if (availableToUsePlaques.Count == 0)
        {
            Debug.LogError("No available plaques found");
            return null;
        }
        PlaqueData[] result = new PlaqueData[3];
        for (int i = 0; i < 3; i++)
            result[i] = availableToUsePlaques[UnityEngine.Random.Range(0,availableToUsePlaques.Count)];
        return result;
    }
}
