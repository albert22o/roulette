public class PlaqueDependencies
{
    public TableConfig TableConfig { get; private set; }
    public int[][] SelectedFields { get; set; }

    public PlaqueDependencies(TableConfig tableConfig)
    {
        this.TableConfig = tableConfig;
    }
}  
