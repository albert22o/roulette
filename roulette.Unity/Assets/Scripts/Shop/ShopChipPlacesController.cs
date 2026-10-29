using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ShopChipPlacesController : MonoBehaviour
{
    [SerializeField] private ChipShopPlace[] shopPlaces;
    [SerializeField] private ShopItemsTable itemsTable;

    private ShopItemsPool itemsPool;

    public event Action ChipBought;

    private void Awake()
    {
        itemsPool = new ShopItemsPool(itemsTable);
        foreach (ChipShopPlace place in shopPlaces)
        {
            place.ItemBought += OnChipBought;
        }
    }

    public void Fill()
    {
        HashSet<ItemData> chosenChips = new HashSet<ItemData>();
        foreach (ChipShopPlace place in shopPlaces)
        {
            ChipData uniqueChip = GetValidChipFromPool(chosenChips);
            place.SetItem(uniqueChip);

            if (uniqueChip != null)
            {
                chosenChips.Add(uniqueChip);
            }
        }
    }

    private void OnChipBought() => ChipBought?.Invoke();

    public void Reroll()
    {
        HashSet<ItemData> chosenChips = new HashSet<ItemData>();

        foreach (ChipShopPlace place in shopPlaces)
        {
            if (place.Item != null)
            {
                ChipData uniqueChip = GetValidChipFromPool(chosenChips);
                place.SetItem(uniqueChip);

                if (uniqueChip != null)
                {
                    chosenChips.Add(uniqueChip);
                }
            }
        }
    }

    public bool CheckRerollAvailable()
    {
        bool hasItem = false;
        foreach (ChipShopPlace place in shopPlaces)
        {
            if (place.IsAnimationPlaying) return false;
            if (place.Item != null) hasItem = true;
        }
        if (hasItem == false) return false;
        return true;
    }

    private ChipData GetValidChipFromPool(HashSet<ItemData> excludedItems)
    {
        if (itemsTable == null || itemsTable.Tables == null || itemsTable.Tables.Length == 0) return null;

        var validElements = new List<ItemsTableElement>();
        float totalWeight = 0f;

        foreach (var element in itemsTable.Tables)
        {
            if (element == null || element.RarityGroup == null || element.RarityGroup.Items == null) continue;

            bool hasAllowedItems = element.RarityGroup.Items.Any(item => item is ChipData && !excludedItems.Contains(item));
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
            .Where(item => item is ChipData && !excludedItems.Contains(item))
            .Cast<ChipData>()
            .ToList();

        if (allowedItemsInGroup.Count == 0) return null;

        int randomIndex = UnityEngine.Random.Range(0, allowedItemsInGroup.Count);
        return allowedItemsInGroup[randomIndex];
    }
}
