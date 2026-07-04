internal class CherryFood : Food
{
    private readonly TableConfig tableConfig;

    public CherryFood(TableConfig tableConfig)
    {
        this.tableConfig = tableConfig;
    }

    public override void Apply()
    {
        tableConfig.AddToStartSum(400);
    }
}