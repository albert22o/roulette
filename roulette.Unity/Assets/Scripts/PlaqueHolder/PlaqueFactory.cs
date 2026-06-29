public class PlaqueFactory
{
    private readonly TableConfig tableConfig;
    private readonly BetCalculator betCalculator;
    

    public PlaqueFactory(TableConfig tableConfig, BetCalculator betCalculator)
    {
        this.tableConfig = tableConfig;
        this.betCalculator = betCalculator;
    }

    public Plaque CreatePlaque(PlaqueData plaqueData)
    {
        return plaqueData.CreatePlaque(new(tableConfig, betCalculator));
    }
}
