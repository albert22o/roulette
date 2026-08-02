public class RerollChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RoundManager roundManager;
    private bool firstReroll = true;

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
    }

    public override void Apply()
    {
    }

    public override void Revert()
    {
    }
}