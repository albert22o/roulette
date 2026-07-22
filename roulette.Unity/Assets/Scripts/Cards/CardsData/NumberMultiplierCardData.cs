using UnityEngine;

[CreateAssetMenu(fileName = "NumberMultiplierCardData", menuName = "Cards/NumberMultiplierCardData")]
public class NumberMultiplierCardData : CardData
{
    public override Card CreateCard(BetCalculator betCalculator)
    {
        return new NumberMultiplierCard(betCalculator, this);
    }
}
