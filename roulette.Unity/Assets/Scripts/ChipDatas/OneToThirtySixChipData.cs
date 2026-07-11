using UnityEngine;

[CreateAssetMenu(fileName = "OneToThirtySixChip", menuName = "Chips/OneToThirtySixChip")]
public class OneToThirtySixChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new OneToThirtySixChip(deps.BetCalculator, this);
    }
}


[CreateAssetMenu(fileName = "StackChip", menuName = "Chips/StackChip")]
public class StackChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new StackChip(deps.TableInfo, this);
    }
}

internal class StackChip : Chip
{
    private readonly TableInfo tableInfo;

    public StackChip(TableInfo tableInfo, StackChipData stackChipData) : base(stackChipData)
    {
        this.tableInfo = tableInfo;
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        tableInfo.AddToBetCount(1);
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        tableInfo.AddToBetCount(-1);
    }
    public override void Apply(){}
    public override void Revert(){}
}