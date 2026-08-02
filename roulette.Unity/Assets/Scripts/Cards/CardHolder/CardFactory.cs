public class CardFactory
{
    private readonly BetCalculator betCalculator;

    public CardFactory(BetCalculator betCalculator)
    {
        this.betCalculator = betCalculator;
    }

    public Card CreateCard(CardData data)
    {
        return data.CreateCard(betCalculator);
    }
}

