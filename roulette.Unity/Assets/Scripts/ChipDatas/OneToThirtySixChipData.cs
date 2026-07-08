using UnityEngine;

[CreateAssetMenu(fileName = "OneToThirtySixChip", menuName = "Chips/OneToThirtySixChip")]
public class OneToThirtySixChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new OneToThirtySixChip(deps.BetCalculator, this);
    }
}