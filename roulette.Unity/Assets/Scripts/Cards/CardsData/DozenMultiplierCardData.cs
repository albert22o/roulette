using UnityEngine;

[CreateAssetMenu(fileName = "DozenMultiplierCardData", menuName = "Cards/DozenMultiplierCardData")]
public class DozenMultiplierCardData : CardData
{
    public override Card CreateCard(BetCalculator betCalculator)
    {
        return new DozenMultiplierCard(betCalculator, this);
    }
}
