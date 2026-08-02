using System.Collections.Generic;

public class CardHolder
{
    private readonly CardFactory cardFactory;
    private readonly List<Card> collectedCards = new List<Card>();

    public CardHolder(CardFactory cardFactory)
    {
        this.cardFactory = cardFactory;
    }

    public void AddCard(CardData cardData)
    {
        var card = cardFactory.CreateCard(cardData);
        collectedCards.Add(card);
        card.Apply();
    }

    public IReadOnlyList<Card> GetCollectedCards()
    {
        return collectedCards;
    }

    public void Clear()
    {
        collectedCards.Clear();
    }
}