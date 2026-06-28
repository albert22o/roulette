
public abstract class Plaque
{
    public PlaqueData PlaqueData { get; }
    protected PlaqueDependencies PlaqueDependencies { get; }

    public Plaque(PlaqueData data, PlaqueDependencies plaqueDependencies)
    {
        PlaqueData = data;
        PlaqueDependencies = plaqueDependencies;
    }

    public abstract void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields);
    public abstract SelectionRequest[] SelectionRequests { get; }
    public abstract void Apply(int[][] selectedFields);

}
