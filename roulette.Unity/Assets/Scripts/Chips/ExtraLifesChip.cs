public class ExtraLifesChip : Chip
{
    private readonly ChipHolder chipHolder;
    private LoseManager loseManager;
    private ExtraLifesChipData extraLifesChipData;
    private bool extraLifeWasTaken;

    public override int AppliedMultiplier => 0;

    public ExtraLifesChip(ChipHolder chipHolder,
        LoseManager loseManager, ExtraLifesChipData extraLifesChipData) : base(extraLifesChipData)
    {
        this.chipHolder = chipHolder;
        this.loseManager = loseManager;
        this.extraLifesChipData = extraLifesChipData;
    }

    public override bool Apply() 
    {
        return true;
    }

    public override void Revert() { }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        loseManager.ExtraLifeTaken += OnExtraLifeTaken;
        loseManager.ExtraLifes++;
    }

    public override void OnRemovedFromHolder() 
    {
        base.OnRemovedFromHolder();
        loseManager.ExtraLifeTaken -= OnExtraLifeTaken;
        if (extraLifeWasTaken)
            return;
        loseManager.ExtraLifes--;
    }

    private void OnExtraLifeTaken()
    {
        extraLifeWasTaken = true;
        chipHolder.RemoveChip(extraLifesChipData);
    }
}