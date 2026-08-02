using UnityEngine;

[CreateAssetMenu(fileName = "ColorMultipilerCardData", menuName = "Cards/ColorMultipilerCardData")]
public class ColorMultipilerCardData : CardData
{
    [SerializeField] private ColorType color;

    public ColorType Color => color;

    public override Card CreateCard(BetCalculator betCalculator)
    {
        return new ColorMultiplierCard(betCalculator, this);
    }
}
