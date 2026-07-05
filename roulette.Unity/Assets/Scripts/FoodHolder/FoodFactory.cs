
public class FoodFactory
{
    private readonly FoodDependencies deps;

    public FoodFactory(BetTable betTable, TableInfo tableInfo)
    {
        deps = new FoodDependencies();
        deps.BetTable = betTable;
        deps.TableInfo = tableInfo;
    }

    public Food Create(FoodData data) => data.CreateFood(deps);
}