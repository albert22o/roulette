public class PlaqueDependencies
{
    public TableInfo TableInfo { get; private set; }
    public BetCalculator BetCalculator { get; private set; }

    public PlaqueDependencies(TableInfo tableInfo, BetCalculator betCalculator)
    {
        TableInfo = tableInfo;
        BetCalculator = betCalculator; 
    }
}  
