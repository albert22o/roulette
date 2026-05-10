using UnityEngine;
using Zenject;

public class GoldenChipsTest : MonoBehaviour
{
    [SerializeField] private int initMoney;
    private Inventory inventory;

    [Inject]
    public void Construct(Inventory inventory)
    {
        this.inventory = inventory;
    }

    void Start()
    {
        inventory.AddCurrency(CurrencyType.GoldenChips, initMoney);
    }
}
