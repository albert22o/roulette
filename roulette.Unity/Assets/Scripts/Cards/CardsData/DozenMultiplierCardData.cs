using UnityEngine;

[CreateAssetMenu(fileName = "DozenMultiplierCardData", menuName = "Cards/DozenMultiplierCardData")]
public class DozenMultiplierCardData : CardData
{
    [SerializeField] private DozenType dozen;

    public DozenType Dozen => dozen;

    public override Card CreateCard(BetCalculator betCalculator)
    {
        return new DozenMultiplierCard(betCalculator, this);
    }
}
