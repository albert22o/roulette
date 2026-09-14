public class RerollChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RoundManager roundManager;
    private bool firstReroll = true;
    private bool effectTriggered;

    public override float AppliedValue => 0;
    public override OperationType Operation => OperationType.None;

    public RerollChip(BetCalculator betCalculator, RoundManager roundManager, 
        RerollChipData rerollChipData) : base(rerollChipData)
    {
        this.betCalculator = betCalculator;
        this.roundManager = roundManager;
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        roundManager.Rerolled += OnRerolled;
        roundManager.Spun += OnSpun;
    }

    private void OnSpun()
    {
        firstReroll = true;
    }

    private void OnRerolled()
    {
        if (!firstReroll)
            return;
        firstReroll = false;
        betCalculator.ResetRerollCoeff();
        effectTriggered = true;
    }

    public override bool Apply()
    {
        return effectTriggered;
    }

    public override void Revert()
    {
        effectTriggered = false;
    }
}