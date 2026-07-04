
public class FoodFactory
{
    private readonly FoodDependencies deps;

    public FoodFactory(BetTable betTable, TableConfig tableConfig)
    {
        deps = new FoodDependencies();
        deps.BetTable = betTable;
        deps.TableConfig = tableConfig;
    }

    public Food Create(FoodData data) => data.CreateFood(deps);
}