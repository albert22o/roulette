public class MultiplyPerRareChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ChipHolder chipHolder;
    private readonly MultiplyPerRareChipData multiplyPerRareChipData;
    float multiplier = 1f;
    public MultiplyPerRareChip(BetCalculator betCalculator, ChipHolder chipHolder, MultiplyPerRareChipData multiplyPerRareChipData) : base(multiplyPerRareChipData)
    {
        this.betCalculator = betCalculator;
        this.chipHolder = chipHolder;
        this.multiplyPerRareChipData = multiplyPerRareChipData;
    }

    public override int AppliedMultiplier => (int)multiplier;

    public override bool Apply()
    {
        multiplier = 1f;
        foreach (var chip in chipHolder.Chips) 
        {
            if (chip == null) continue;
            if (chip.ChipData.RarityType == multiplyPerRareChipData.RarityTypeEffect)
                multiplier += multiplyPerRareChipData.MultiplierPerChip;
        }
        betCalculator.MuliplyGeneralModifier(multiplier);
        return multiplier != 1f;
    }

    public override void Revert()
    {
        betCalculator.DivideGeneralModifier(multiplier);
    }
}