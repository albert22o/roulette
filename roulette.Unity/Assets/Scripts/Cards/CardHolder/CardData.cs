using UnityEngine;

public abstract class CardData : ItemData
{
    [SerializeField] private int multiplier;
    public int Multiplier => multiplier;

    public abstract Card CreateCard(BetCalculator betCalculator);
}
