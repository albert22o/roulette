using UnityEngine;

[CreateAssetMenu(fileName = "MultiplyDivisibilityBonusChip", menuName = "Chips/MultiplyDivisibilityBonusChip")]
public class MultiplyDivisibilityBonusChipData : ChipData
{
    public int Divisor;
    public int Remainder = 0;
    public float Multiplier = 2f;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new MultiplyDivisibilityBonusChip( deps.BetCalculator, deps.RouletteBallsManager, deps.OnBallSelected, this);
    }   
}