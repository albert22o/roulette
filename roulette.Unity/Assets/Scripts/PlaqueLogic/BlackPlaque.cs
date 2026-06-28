public class BlackPlaque : Plaque
{
    public BlackPlaque(PlaqueData data, PlaqueDependencies dependencies) : base(data, dependencies)
    {

    }

    private readonly SelectionRequest[] selectionRequests = new SelectionRequest[]
    {
        new() {
            Count = 5,
            Filter = cell => cell.FieldColor == ColorType.Red,
            Hint = "Выберите 5 красных полей для перекраски в чёрный"
        }
    };

    public override SelectionRequest[] SelectionRequests => selectionRequests;

    public override void Apply(int[][] selectedFields)
    {
        for (int i = 0; i < selectedFields[0].Length; i++)
        {
            PlaqueDependencies.TableConfig.SetPocketColor(selectedFields[0][i], ColorType.Black);
        }
    }

    public override void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields)
    {
        
    }
}
