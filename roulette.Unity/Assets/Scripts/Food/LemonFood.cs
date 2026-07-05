public class LemonFood : Food
{
    private readonly TableInfo tableInfo;

    public LemonFood(TableInfo tableInfo)
    {
        this.tableInfo = tableInfo;
    }

    public override void Apply()
    {
        tableInfo.AddToBetCount(1);
    }
}