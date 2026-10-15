using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] Settings settings;
    public override void InstallBindings()
    {
        // Менеджеры состояний
        Container.Bind<GameStateManager>().AsSingle();
        Container.Bind<RoundStateManager>().AsSingle();
        // Приколюхи Юнити
        Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CinemachineBrainEvents>().FromComponentInHierarchy().AsSingle();
        // Конфиги
        Container.BindInstance(settings.RoundsConfig).AsSingle();
        Container.BindInstance(settings.TableConfig).AsSingle();
        Container.BindInstance(settings.CurrenciesConfig).AsSingle();
        Container.BindInstance(settings.BallHolderConfig).AsSingle();
        Container.BindInstance(settings.ChipHolderConfig).AsSingle();
        Container.BindInstance(settings.PlaqueShopConfig).AsSingle();
        Container.BindInstance(settings.BossRewardConfig).AsSingle();
        Container.BindInstance(settings.RarityData).AsSingle();
        Container.BindInstance(settings.ShopFontConfig).AsSingle();
        Container.BindInstance(settings.ColorsDescription).AsSingle();
        // Валюты
        Container.Bind<GoldenChips>().AsSingle();
        Container.Bind<BossChips>().AsSingle();
        Container.Bind<Tickets>().AsSingle();

        Container.Bind<TableInfo>().AsSingle();

        Container.Bind<RouletteWheel>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container.Bind<IRouletteWheel>()
            .To<RouletteWheel>()
            .FromResolve();

        Container.Bind<IAnimateBalls>()
            .To<RouletteWheel>()
            .FromResolve();

        Container.Bind<ProgressionManager>().AsSingle();
        Container.Bind<BetTable>().AsSingle();

        Container.Bind<BallResults>().AsSingle();

        Container.Bind<BetCalculator>().AsSingle();

        Container.Bind<ChipFactory>().AsSingle();
        Container.Bind<ChipHolder>().AsSingle();

        Container.Bind<BallFactory>().AsSingle();
        Container.Bind<BallHolder>().AsSingle();

        Container.Bind<FoodFactory>().AsSingle();
        Container.Bind<FoodHolder>().AsSingle();

        Container.Bind<RoundManager>().AsSingle();

        Container.Bind<LoseManager>().AsSingle();

        Container.Bind<RewardCalculator>().AsSingle().NonLazy();

        Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();

        Container.Bind<ChipSpawner>().AsSingle();

        Container.Bind<PlaqueShop>().AsSingle();
        Container.Bind<PlaqueFieldsSelector>().AsSingle();
        Container.Bind<PlaqueHolder>().AsSingle();
        Container.Bind<PlaqueUseController>().AsSingle();
        Container.Bind<PlaqueFactory>().AsSingle();

        Container.Bind<WinningSequence>().AsSingle();

        Container.Bind<CameraEventHandler>().AsSingle();
        Container.Bind<FirstPersonController>().FromComponentInHierarchy().AsSingle();

        Container.Bind<BossReward>().AsSingle();
        Container.Bind<ItemReceiver>().AsSingle();
        Container.Bind<CardHolder>().AsSingle();
        Container.Bind<RoundWinManager>().AsSingle().NonLazy();
        Container.Bind<CardFactory>().AsSingle();

        Container.BindInterfacesTo<RouletteBetFieldsHighlight>().AsSingle();
        Container.Bind<RunProgress>().AsSingle();

        Container.Bind<CursorManager>().AsSingle();
        Container.Bind<FreeroamCursorHandler>().AsSingle().NonLazy();
        Container.Bind<ChipSoundPlayer>().FromComponentInHierarchy().AsSingle();
    }
}