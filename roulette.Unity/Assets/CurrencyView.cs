using TMPro;
using UnityEngine;
using Zenject;

public class CurrencyView : MonoBehaviour
{
    [SerializeField] private DrumNumberDisplay display;
    [SerializeField] private CurrencyType currencyType;
    private Inventory inventory;

    [Inject]
    public void Construct(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private void OnEnable()
    {
        inventory.CurrencyChanged += UpdateText;
        UpdateText(currencyType, inventory.GetCurrency(currencyType));
    }

    private void OnDisable()
    {
        inventory.CurrencyChanged -= UpdateText;
    }

    private void UpdateText(CurrencyType type, int amount)
    {
        if (currencyType == type)
            display.SetNumber(amount);
    }
}
