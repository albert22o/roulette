using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] RoundsConfig roundsConfig;
    [SerializeField] BallHolderConfig ballHolderConfig;
    [SerializeField] ChipHolderConfig chipHolderConfig;

    public override void InstallBindings()
    {
        // Менеджеры состояний
        Container.Bind<GameStateManager>().AsSingle();
        Container.Bind<RoundStateManager>().AsSingle();
        // Приколюхи Юнити
        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
        // Конфиги
        Container.BindInstance(roundsConfig).AsSingle();
        Container.Bind<TableConfig>().AsSingle();
        Container.BindInstance(ballHolderConfig).AsSingle();
        Container.BindInstance(chipHolderConfig).AsSingle();
        // Валюты
        Container.Bind<GoldenChips>().AsSingle();
        Container.Bind<BossChips>().AsSingle();
        Container.Bind<Tickets>().AsSingle();
        Container.Bind<Money>().AsSingle();

        Container.Bind<IRouletteWheel>()
            .To<RouletteWheel>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container.Bind<IRouletteBallsManager>()
            .To<RouletteBallsManager>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container.Bind<ProgressionManager>().AsSingle();
        Container.Bind<BetTable>().AsSingle();

        Container.Bind<BallResults>().AsSingle();

        Container.Bind<BetCalculator>().AsSingle();

        Container.Bind<ChipFactory>().AsSingle();
        Container.Bind<ChipHolder>().AsSingle();

        Container.Bind<BallFactory>().AsSingle();
        Container.Bind<BallHolder>().AsSingle();

        Container.Bind<RoundManager>().AsSingle();

        Container.Bind<RewardCalculator>().AsSingle().NonLazy();

        Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();

        Container.Bind<ChipSpawner>().AsSingle();

        Container.Bind<WinningSequence>().AsSingle();
    }
}