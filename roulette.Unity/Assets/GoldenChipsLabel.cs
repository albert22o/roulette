using TMPro;
using UnityEngine;
using Zenject;

public class GoldenChipsLabel : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    private Inventory inventory;

    [Inject]
    public void Construct(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private void OnEnable()
    {
        inventory.GoldenChipsChanged += UpdateText;
    }

    private void OnDisable()
    {
        inventory.GoldenChipsChanged -= UpdateText;
    }

    private void UpdateText(int count)
    {
        text.text = count.ToString();
    }
}
