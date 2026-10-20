using Zenject;

public class ShopInstaller : Installer<ShopInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<ShopItemSoldSignal>();
        Container.DeclareSignal<ShopRerolledSignal>();
        Container.DeclareSignal<FoodBoughtSignal>();
        Container.DeclareSignal<BallMachineSpinSignal>();
        Container.DeclareSignal<ChipBoughtSignal>();
        Container.DeclareSignal<CounterValueChanged>();
        Container.DeclareSignal<ItemPucharedSignal>();
        Container.DeclareSignal<ItemSoldSignal>();
    }
}
