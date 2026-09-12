public class ChangeChipSlot : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ChipHolder chipHolder;
    private readonly ChangeChipSlotData changeChipSlotData;

    public override int AppliedMultiplier => changeChipSlotData.Multiplier;

    public ChangeChipSlot(BetCalculator betCalculator, ChipHolder chipHolder, ChangeChipSlotData lostChipData) : base(lostChipData)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        this.changeChipSlotData = lostChipData;
    }

    public override bool Apply()
    {
        betCalculator.MuliplyGeneralModifier(changeChipSlotData.Multiplier);
        return true;
    }

    public override void Revert()
    {
        betCalculator.DivideGeneralModifier(changeChipSlotData.Multiplier);
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        chipHolder.RemoveLastSlots(changeChipSlotData.SlotsToRemove);
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        chipHolder.AddSlots(changeChipSlotData.SlotsToRemove);
    }
}
