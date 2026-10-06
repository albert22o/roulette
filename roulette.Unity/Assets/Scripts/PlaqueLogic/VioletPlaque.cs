using System.Collections.Generic;
using UnityEngine;

public class VioletPlaque : Plaque
{
    private static readonly System.Random random = new System.Random();

    public override SelectionRequest[] SelectionRequests => null;

    public VioletPlaque(PlaqueData data, PlaqueDependencies deps)
        : base(data, deps)
    {

    }

    public override void Apply(int[][] selectedFields)
    {
        List<int> availableIndexes = new();
        for (int i = 0; i < PlaqueDependencies.TableInfo.PocketMapping.Count; i++)
        {
            if (PlaqueDependencies.TableInfo.PocketMapping[i].FieldColor != ColorType.Violet)
                availableIndexes.Add(i);
        }
        if (availableIndexes.Count == 0)
        {
            Debug.LogWarning("No available fields to set to violet.");
            return;
        }
        int randomIndex = availableIndexes[random.Next(0, availableIndexes.Count)];

        PlaqueDependencies.TableInfo.SetPocketColor(randomIndex, ColorType.Violet);
        PlaqueDependencies.BetCalculator.ModifyGeneralModifier(3);
    }

    public override void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields)
    {

    }
}
