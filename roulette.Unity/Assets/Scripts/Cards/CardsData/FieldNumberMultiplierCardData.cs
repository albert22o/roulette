using UnityEngine;

[CreateAssetMenu(fileName = "FieldNumberMultiplierCardData", menuName = "Cards/FieldNumberMultiplierCardData")]
public class FieldNumberMultiplierCardData : CardData
{
    [SerializeField] private int number;

    public int Number => number;

    public override Card CreateCard(BetCalculator betCalculator)
    {
        return new FieldNumberMultiplierCard(betCalculator, this);
    }
}
