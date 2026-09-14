
public class DebtChip : Chip
{
    private readonly GoldenChips goldenChips;
    private readonly int debtAmount;

    public override float AppliedValue => 0;
    public override OperationType Operation => OperationType.None;

    public DebtChip(GoldenChips goldenChips, DebtChipData data) : base(data)
    {
        this.goldenChips = goldenChips;
        debtAmount = data.DebtAmount; 
    }

    public override bool Apply()
    {
        return true;
    }

    public override void Revert()
    {
    }

    public override void OnAddedToHolder()
    {
        goldenChips.OverdraftLimit += debtAmount;
    }

    public override void OnRemovedFromHolder()
    {
        goldenChips.OverdraftLimit -= debtAmount;
    }
}