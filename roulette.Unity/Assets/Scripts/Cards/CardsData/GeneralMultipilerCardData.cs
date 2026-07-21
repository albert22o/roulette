using UnityEngine;

[CreateAssetMenu(fileName = "GeneralMultipilerCardData", menuName = "Cards/GeneralMultipilerCardData")]
public class GeneralMultipilerCardData : CardData
{
    public override Card CreateCard(BetCalculator betCalculator)
    {
        return new GeneralMultiplierCard(betCalculator, this);
    }
}
