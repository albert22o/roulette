using System;
using System.Diagnostics;

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
            Hint = "Выберите 1 число для замены"
        },
        new() {
            Count = 3,
            Filter = _ => true,
            Hint = "Выберите 3 числа, которые нужно заменить на выбранное"
        }
    };

    public override SelectionRequest[] SelectionRequests => selectionRequests;

    public override void Apply(int[][] selectedFields)
    {
        for (int i = 0; i < selectedFields[1].Length; i++)
        {
            PlaqueDependencies.TableConfig.SetPocketNum(selectedFields[1][i],
                PlaqueDependencies.TableConfig.PocketMapping[selectedFields[0][0]].FieldNumber);
        }
    }

    public override void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields)
    {
        if (phaseIndex == 0)
        {
            selectionRequests[1].Hint = $"Выберите 3 числа, которые нужно заменить на " +
                $"{PlaqueDependencies.TableConfig.PocketMapping[selectedFields[0]].FieldNumber}";
        }
    }
}
