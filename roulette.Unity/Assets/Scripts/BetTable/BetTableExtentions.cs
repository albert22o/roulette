using System.Linq;

public static class BetTableExtentions
{
    public static bool CheckNumberMatch(BetTable betTable, HitInfo hit)
    {
        return betTable.NotNullBetAmounts.Any(bet => bet.Item1.FieldNumber == hit.FieldNumber);
    }

    public static bool CheckColorMatch(BetTable betTable, HitInfo hit)
    {
        return betTable.NotNullBetAmounts.Any(bet => bet.Item1.FieldColor == hit.FieldColor);
    }

    public static bool CheckDozenMatch(BetTable betTable, HitInfo hit)
    {
        return betTable.NotNullBetAmounts.Any(bet => bet.Item1.Dozen == hit.Dozen);
    }
}
