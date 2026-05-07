using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<RoundManager>().AsSingle();

        Container.Bind<BetCalculator>().AsSingle();

        Container.Bind<BetTable>().AsSingle();
        Container.Bind<BallResults>().AsSingle();

        Container.Bind<Inventory>().AsSingle();

        Container.Bind<IRouletteWheel>().To<RouletteWheel>().FromComponentInHierarchy().AsSingle();

        Container.Bind<TableConfig>().AsSingle();

        Container.Bind<RoundStateManager>().AsSingle();
    }
}