using UnityEngine;

[CreateAssetMenu(fileName = "TestPlaque", menuName = "Scriptable Objects/TestPlaque")]
public class TestPlaque : PlaqueData
{
    private SelectionRequest[] selectionRequests = new SelectionRequest[]
    {
        new() {
            Count = 5,
            Filter = cell => cell.FieldColor == ColorType.Red,
            Hint = "Выберите 5 красных полей для перекраски в чёрный"
        }
    };

    public override SelectionRequest[] SelectionRequests => selectionRequests;

    public override Plaque CreatePlaque()
    {
        return new BlackPlaque(this);
    }
}
