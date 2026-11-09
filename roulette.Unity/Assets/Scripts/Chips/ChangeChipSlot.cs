using TMPro;

public class ChangeChipSlot : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ChipHolder chipHolder;
    private readonly ChangeChipSlotData changeChipSlotData;

    private bool applyConditionsMatched;
    private bool multiplierApplied = false;
    private readonly BetTable betTable;
    private readonly BallResults ballResults;

    public override float AppliedValue => changeChipSlotData.Multiplier;
    public override OperationType Operation => OperationType.Multiply;

    public ChangeChipSlot(BetCalculator betCalculator, ChipHolder chipHolder, ChangeChipSlotData lostChipData,
        BallResults ballResults, BetTable betTable) : base(lostChipData)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        this.changeChipSlotData = lostChipData;
        this.ballResults = ballResults;
        this.betTable = betTable;   
        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo hit)
    {
        applyConditionsMatched = BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit);
    }

    public override bool Apply()
    {
        if (applyConditionsMatched && changeChipSlotData.Multiplier != 1)
        {
            betCalculator.MultiplyGeneralModifier(changeChipSlotData.Multiplier);
            multiplierApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (multiplierApplied)
        {
            betCalculator.DivideGeneralModifier(changeChipSlotData.Multiplier);
            multiplierApplied = false;
        }
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        if (changeChipSlotData.SlotsToRemove < 0)
            chipHolder.AddSlots(-changeChipSlotData.SlotsToRemove);
        else
            chipHolder.RemoveLastSlots(changeChipSlotData.SlotsToRemove);
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        if (changeChipSlotData.SlotsToRemove < 0)
            chipHolder.RemoveLastSlots(-changeChipSlotData.SlotsToRemove);
        else
            chipHolder.AddSlots(changeChipSlotData.SlotsToRemove);
    }
}
