using UnityEngine;
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

        Container.BindInstance(inventoryConfig).AsSingle();
        Container.Bind<Inventory>().AsSingle();
        Container.Bind<ItemFactory>().AsSingle();
    }
}