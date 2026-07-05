internal class CherryFood : Food
{
    private readonly TableInfo tableInfo;

    public CherryFood(TableInfo tableInfo)
    {
        this.tableInfo = tableInfo;
    }

    public override void Apply()
    {
        tableInfo.AddToStartSum(400);
    }
}