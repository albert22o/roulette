public class PlaqueFactory
{
    private readonly TableInfo tableInfo;
    private readonly BetCalculator betCalculator;
    

    public PlaqueFactory(TableInfo tableInfo, BetCalculator betCalculator)
    {
        this.tableInfo = tableInfo;
        this.betCalculator = betCalculator;
    }

    public Plaque CreatePlaque(PlaqueData plaqueData)
    {
        return plaqueData.CreatePlaque(new(tableInfo, betCalculator));
    }
}
