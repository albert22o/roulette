using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] InventoryConfig inventoryConfig;
    [SerializeField] RoundsConfig roundsConfig;

   
   
    public override void InstallBindings()
    {
        // Менеджеры состояний
        Container.Bind<GameStateManager>().AsSingle();
        Container.Bind<RoundStateManager>().AsSingle();

        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
        // Конфиги
        Container.BindInstance(roundsConfig).AsSingle();
        Container.BindInstance(inventoryConfig).AsSingle();
        Container.Bind<TableConfig>().AsSingle();
        
        Container.Bind<IRouletteWheel>()
            .To<RouletteWheel>()
            .FromComponentInHierarchy()
            .AsSingle();
        
        Container.Bind<ProgressionManager>().AsSingle();
        Container.Bind<Inventory>().AsSingle();
        Container.Bind<BetTable>().AsSingle();

        Container.Bind<BallResults>().AsSingle();

        Container.Bind<BetCalculator>().AsSingle();

        Container.Bind<ChipFactory>().AsSingle();
        Container.Bind<ChipHolder>().AsSingle();

        Container.Bind<RoundManager>().AsSingle();

        Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();

        Container.Bind<GoldenChips>().AsSingle();
        Container.Bind<BossChips>().AsSingle();
        Container.Bind<Tickets>().AsSingle();
        Container.Bind<Money>().AsSingle();

        Container.Bind<ChipSpawner>().AsSingle();
        Container.Bind<ChipSpawnerManager>().FromComponentInHierarchy().AsSingle();

    }
}