public abstract class Card
{
    protected BetCalculator BetCalculator { get; private set; }
    protected int Multiplier { get; private set; }
    public CardData Data { get; private set; }

    public Card(BetCalculator betCalculator, CardData data)
    {
        BetCalculator = betCalculator;
        Multiplier = data.Multiplier;
        Data = data;
    }

    public abstract void Apply();
}
