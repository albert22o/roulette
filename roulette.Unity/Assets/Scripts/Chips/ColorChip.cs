public class ColorChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ColorType color;
    private readonly int modifyAmount;

    public ColorChip(BetCalculator betCalculator, ColorChipData colorChipData) : base(colorChipData)
    {
        this.betCalculator = betCalculator;
        color = colorChipData.Color;
        modifyAmount = colorChipData.ModifyAmount;
    }

    public override bool Apply()
    {
        betCalculator.ModifyColorMultiplier(color, modifyAmount);
        return true;
    }

    public override void Revert()
    {
        betCalculator.ModifyColorMultiplier(color, -modifyAmount);
    }
}
