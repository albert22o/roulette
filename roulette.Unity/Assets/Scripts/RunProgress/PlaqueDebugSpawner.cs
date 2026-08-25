using UnityEngine;
using Zenject;

public class PlaqueDebugSpawner : MonoBehaviour
{
    [Inject] private PlaqueHolder plaqueHolder;

    [Header("Настройки теста плаков")]
    [Tooltip("Сколько плаков первого типа добавить")]
    public int amountOfTypeA = 3;

    [Tooltip("Сколько плаков второго типа добавить")]
    public int amountOfTypeB = 2;

    private void Start()
    {
        AddTestPlaques();
    }

    private void AddTestPlaques()
    {
        PlaqueDependencies dummyDeps = null;

        for (int i = 0; i < amountOfTypeA; i++)
        {
            var data = ScriptableObject.CreateInstance<TestPlaqueDataA>();
            var plaque = data.CreatePlaque(dummyDeps);
            plaqueHolder.plaques.Add(plaque);
        }

        for (int i = 0; i < amountOfTypeB; i++)
        {
            var data = ScriptableObject.CreateInstance<TestPlaqueDataB>();
            var plaque = data.CreatePlaque(dummyDeps);
            plaqueHolder.plaques.Add(plaque);
        }

        Debug.Log($"<color=magenta>[Debug]</color> Добавлено тестовых плаков: {amountOfTypeA} типа A и {amountOfTypeB} типа B");
    }
}


public class TestPlaqueDataA : PlaqueData
{
    public override Plaque CreatePlaque(PlaqueDependencies dependencies) => new TestPlaqueA(this, dependencies);
}

public class TestPlaqueA : Plaque
{
    public TestPlaqueA(PlaqueData data, PlaqueDependencies deps) : base(data, deps) { }
    public override void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields) { }
    public override SelectionRequest[] SelectionRequests => null;
    public override void Apply(int[][] selectedFields) { }
}

public class TestPlaqueDataB : PlaqueData
{
    public override Plaque CreatePlaque(PlaqueDependencies dependencies) => new TestPlaqueB(this, dependencies);
}

public class TestPlaqueB : Plaque
{
    public TestPlaqueB(PlaqueData data, PlaqueDependencies deps) : base(data, deps) { }
    public override void OnSelectionPhaseCompleted(int phaseIndex, int[] selectedFields) { }
    public override SelectionRequest[] SelectionRequests => null;
    public override void Apply(int[][] selectedFields) { }
}