public class AntiDozenChip : Chip
{
    private readonly IRouletteWheel rouletteWheel;
    private readonly DozenType targetDozen;
    private bool rerollAbility = true;

    public override OperationType Operation => OperationType.None;

    public AntiDozenChip(IRouletteWheel rouletteWheel, DozenType targetDozen, AntiDozenChipData antiDozenChipData) : base(antiDozenChipData)
    {
        this.targetDozen = targetDozen;
        this.rouletteWheel = rouletteWheel;
        rouletteWheel.AllBallsStopped += OnAllBallsStopped;
    }

    public override float AppliedValue => 0;

    public override bool Apply()
    {
        return false;
    }

    public override void Revert()
    {
        rerollAbility = true;
    }

    private void OnAllBallsStopped(HitInfo[] hit)
    {
        if (!rerollAbility)
            return;
        rerollAbility = false;
        foreach (var hitInfo in hit)
        {
            if (hitInfo.Dozen == targetDozen)
            {
                rouletteWheel.AddBallsToAutoReroll(hitInfo.BallHash);
                return;
            }
        }
    }
}