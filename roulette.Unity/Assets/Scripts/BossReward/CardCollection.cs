using System.Collections.Generic;
using UnityEngine;

public class CardCollection
{
    private readonly List<ItemData> collectedCards = new List<ItemData>();

    public void AddCard(ItemData card)
    {
        if (card is GeneralMultipilerCardData)
        {
            collectedCards.Add(card);
        }
    }

    public List<ItemData> GetCollectedCards()
    {
        return collectedCards;
    }

    public void Clear()
    {
        collectedCards.Clear(); 
    }
}