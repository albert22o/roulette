using UnityEngine;
using Zenject;

public class CurrencyView<T> : MonoBehaviour where T : CurrencyResource
{
    [SerializeField] private DrumNumberDisplay display;

    private T currency;

    [Inject]
    public void Construct(T currency)
    {
        this.currency = currency;
    }

    private void OnEnable()
    {
        currency.AmountChanged += UpdateDisplay;
        UpdateDisplay(currency.Amount);
    }

    private void OnDisable()
    {
        currency.AmountChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int amount)
    {
        display.SetNumber(amount);
    }
}