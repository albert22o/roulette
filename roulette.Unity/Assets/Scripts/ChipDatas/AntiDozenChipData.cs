using UnityEngine;

[CreateAssetMenu(fileName = "AntiDozenChip", menuName = "Chips/AntiDozenChip")]
public class AntiDozenChipData : ChipData
{
    [SerializeField] private DozenType targetDozen;

    public DozenType TargetDozen => targetDozen;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new AntiDozenChip(deps.RouletteWheel, targetDozen, this);
    }
}
