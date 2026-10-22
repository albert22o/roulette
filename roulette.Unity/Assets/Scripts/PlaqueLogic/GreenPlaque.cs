public class GreenPlaque : Plaque
{
    public GreenPlaque(PlaqueData data, PlaqueDependencies dependencies) : base(data, dependencies)
    {

    }

    private readonly SelectionRequest[] selectionRequests = new SelectionRequest[]
    {
        new() {
            Count = 1,
            Filter = _ => true,
            Hint = Loc.Get("plaque.green.hint_pick")
        },
        new() {
            Count = 3,
            Filter = _ => true,
            Hint = Loc.Get("plaque.green.hint_replace")
        }
    };

    public override SelectionRequest[] SelectionRequests => selectionRequests;

    public override void Apply(int[][] selectedFields)
    {
        for (int i = 0; i < selectedFields[1].Length; i++)
        {
            PlaqueDependencies.TableInfo.SetPocketNum(selectedFields[1][i],
                PlaqueDependencies.TableInfo.PocketMapping[selectedFields[0][0]].FieldNumber);
            PlaqueDependencies.TableInfo.SetPocketColor(selectedFields[1][i],
                PlaqueDependencies.TableInfo.PocketMapping[selectedFields[0][0]].FieldColor);
        }
    }

    public override void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields)
    {
        if (phaseIndex == 0)
        {
            int firstPhaseSelectedIndex = selectedFields[0];

            selectionRequests[1].Filter = betInfo => betInfo.Index != firstPhaseSelectedIndex;

            int displayNum = PlaqueDependencies.TableInfo.PocketMapping[firstPhaseSelectedIndex].FieldNumber;
            selectionRequests[1].Hint = Loc.Get("plaque.green.hint_replace_num", displayNum);
        }
    }
}
