using TMPro;
using UnityEngine;
using Zenject;

public class CurrencyLabel : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
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
    }

    private void OnDisable()
    {
        inventory.CurrencyChanged -= UpdateText;
    }

    private void UpdateText(CurrencyType type, int count)
    {
        if (currencyType == type)
            text.text = count.ToString();
    }
}
