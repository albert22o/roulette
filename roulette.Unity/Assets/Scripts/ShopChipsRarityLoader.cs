using TMPro;
using UnityEngine;
using Zenject;

public class ShopChipsRarityLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI common, rare, epic;
    private Shop shop;

    [Inject]
    private void Construct(Shop shop)
    {
        this.shop = shop;
    }

    private void Start()
    {
        var chipsTable = shop.Config.ChipsTable;
        common.text = $"{chipsTable.GetProbability(RarityType.Common)}%";
        rare.text = $"{chipsTable.GetProbability(RarityType.Rare)}%";
        epic.text = $"{chipsTable.GetProbability(RarityType.Epic)}%";
    }
}
