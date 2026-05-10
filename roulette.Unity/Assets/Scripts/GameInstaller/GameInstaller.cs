using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private InventoryConfig inventoryConfig;
    public override void InstallBindings()
    {
        Container.Bind<RoundManager>().AsSingle();

        Container.Bind<BetCalculator>().AsSingle();

        Container.Bind<BetTable>().AsSingle();
        Container.Bind<BallResults>().AsSingle();

        Container.Bind<IRouletteWheel>().To<RouletteWheel>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();

        Container.Bind<TableConfig>().AsSingle();

        Container.Bind<RoundStateManager>().AsSingle();

        Container.BindInstance(inventoryConfig).AsSingle();
        Container.Bind<Inventory>().AsSingle();

        Container.Bind<GameStateManager>().AsSingle();

        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
    }
}