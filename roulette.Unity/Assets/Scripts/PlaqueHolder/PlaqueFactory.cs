public class PlaqueFactory
{
    private TableConfig tableConfig;

    public PlaqueFactory(TableConfig tableConfig)
    {
        this.tableConfig = tableConfig;
    }

    public Plaque CreatePlaque(PlaqueData plaqueData)
    {
        return plaqueData.CreatePlaque(new(tableConfig));
    }
}
