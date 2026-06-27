using UnityEngine;

public class BlackPlaque : Plaque
{
    public BlackPlaque(PlaqueData data) : base(data)
    {

    }

    public override void Apply(PlaqueDependencies dependencies)
    {
        for (int i = 0; i < dependencies.SelectedFields[0].Length; i++)
        {
            dependencies.TableConfig.SetPocketColor(dependencies.SelectedFields[0][i], ColorType.Black);
        }
    }
}
