using System.Collections.Generic;

public class PlaqueHolder
{
    public List<Plaque> plaques = new();
    private TableConfig tableConfig;
    
    public PlaqueHolder(TableConfig tableConfig)
    {
        this.tableConfig = tableConfig;
    }

    public void AddPlaque(PlaqueData plaqueData, int[][] selectedFields)
    {
        var plaque = plaqueData.CreatePlaque();
        plaques.Add(plaque);
        var dependencies = new PlaqueDependencies(tableConfig)
        {
            SelectedFields = selectedFields
        };
        plaque.Apply(dependencies);
    }
}