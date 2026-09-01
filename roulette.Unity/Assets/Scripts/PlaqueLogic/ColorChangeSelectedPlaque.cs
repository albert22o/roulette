using System;
public class ColorChangeSelectedPlaque : Plaque
{
    private ColorType targetColor;
    public ColorChangeSelectedPlaque(ColorChangeSelectedPlaqueData data, PlaqueDependencies dependencies)
        : base(data, dependencies)
    {
        selectionRequests = new SelectionRequest[]
        {
            new SelectionRequest()
            {
                Count = data.SelectionCount,
                Filter = CreateFilter(data),
                Hint = data.Hint,
            }
        };
        targetColor = data.TargetColor;
    }

    private readonly SelectionRequest[] selectionRequests;

    public override SelectionRequest[] SelectionRequests => selectionRequests;

    public override void Apply(int[][] selectedFields)
    {
        for (int i = 0; i < selectedFields[0].Length; i++)
        {
            PlaqueDependencies.TableInfo.SetPocketColor(selectedFields[0][i], targetColor);
        }
    }

    public override void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields)
    {

    }

    private Func<BetInfo, bool> CreateFilter(ColorChangeSelectedPlaqueData data)
    {
        return cell => (data.Filter & ConvertColor(cell.FieldColor)) != 0;
    }

    private ColorMask ConvertColor(ColorType color)
    {
        return color switch
        {
            ColorType.Red => ColorMask.Red,
            ColorType.Black => ColorMask.Black,
            ColorType.Green => ColorMask.Green,
            _ => ColorMask.None
        };
    }
}