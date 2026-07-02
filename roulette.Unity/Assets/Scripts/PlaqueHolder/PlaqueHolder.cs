using System.Collections.Generic;

public class PlaqueHolder
{
    public List<Plaque> plaques = new();
    
    public void AddPlaque(Plaque plaque, int[][] selectedFields)
    {
        plaques.Add(plaque);
        plaque.Apply(selectedFields);
    }
}