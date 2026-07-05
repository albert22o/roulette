public class GrayPlaque : Plaque
{
    private readonly System.Random random = new System.Random();

    public override SelectionRequest[] SelectionRequests => null;

    public GrayPlaque(PlaqueData data, PlaqueDependencies deps)
        : base(data, deps)
    {

    }

    public override void Apply(int[][] selectedFields)
    {
        int randomIndex = random.Next(0, PlaqueDependencies.TableInfo.PocketMapping.Count);

        PlaqueDependencies.TableInfo.SetPocketColor(randomIndex, ColorType.Green); //TODO change to Gray
        PlaqueDependencies.BetCalculator.GeneralModifier += 3;
    }

    public override void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields)
    {

    }
}
