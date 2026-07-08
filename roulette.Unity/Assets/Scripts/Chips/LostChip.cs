public class LostChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ChipHolder chipHolder;
    private readonly LostChipData lostChipData;

    public LostChip(BetCalculator betCalculator, ChipHolder chipHolder, LostChipData lostChipData) : base(lostChipData)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        this.lostChipData = lostChipData;
    }

    public override void Apply()
    {
        betCalculator.MuliplyGeneralModifier(lostChipData.Multiplier);
    }

    public override void Revert()
    {
        betCalculator.DivideGeneralModifier(lostChipData.Multiplier);
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        chipHolder.RemoveLastSlots(lostChipData.SlotsToRemove);
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        chipHolder.AddSlots(lostChipData.SlotsToRemove);
    }
}
