public class LemonFood : Food
{
    private readonly TableConfig tableConfig;

    public LemonFood(TableConfig tableConfig)
    {
        this.tableConfig = tableConfig;
    }

    public override void Apply()
    {
        tableConfig.BetCount++;
    }
}