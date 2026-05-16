using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private InventoryConfig inventoryConfig;
    [SerializeField] private RoundsConfig roundsConfig;
    public override void InstallBindings()
    {
        Container.Bind<GameStateManager>().AsSingle();
        Container.Bind<RoundStateManager>().AsSingle();

        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();

        Container.BindInstance(roundsConfig).AsSingle();
        Container.BindInstance(inventoryConfig).AsSingle();
        Container.Bind<TableConfig>().AsSingle();

        Container.Bind<IRouletteWheel>().To<RouletteWheel>().FromComponentInHierarchy().AsSingle();

        Container.Bind<ProgressionManager>().AsSingle();
        Container.Bind<Inventory>().AsSingle();
        Container.Bind<BetTable>().AsSingle();

        Container.Bind<BallResults>().AsSingle();

        Container.Bind<BetCalculator>().AsSingle();

        Container.Bind<RoundManager>().AsSingle();

        Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();
    }
}