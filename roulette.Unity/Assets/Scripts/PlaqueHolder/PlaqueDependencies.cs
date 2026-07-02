public class PlaqueDependencies
{
    public TableConfig TableConfig { get; private set; }
    public BetCalculator BetCalculator { get; private set; }

    public PlaqueDependencies(TableConfig tableConfig, BetCalculator betCalculator)
    {
        TableConfig = tableConfig;
        BetCalculator = betCalculator; 
    }
}  
