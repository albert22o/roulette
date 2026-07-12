using UnityEngine;

[CreateAssetMenu(fileName = "ExtraLifesChip", menuName = "Chips/ExtraLifesChip")]
public class ExtraLifesChipData : ChipData
{

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ExtraLifesChip(deps.ChipHolder, deps.LoseManager, this);
    }
}
