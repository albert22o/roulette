using System.Collections.Generic;
using UnityEngine;

public class CardHolder
{
    private readonly List<ItemData> collectedCards = new List<ItemData>();

    public void AddCard(ItemData card)
    {
        if (card is GeneralMultipilerCardData)
        {
            collectedCards.Add(card);
        }
    }

    public IReadOnlyList<ItemData> GetCollectedCards()
    {
        return collectedCards;
    }

    public void Clear()
    {
        collectedCards.Clear();
    }
}