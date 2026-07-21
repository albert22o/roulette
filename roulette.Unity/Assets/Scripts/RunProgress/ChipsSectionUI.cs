using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ChipsSectionUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private ListItemUI listItemPrefab;
    [SerializeField] private Sprite defaultIcon; 

    private ChipHolder chipHolder;
    private List<ListItemUI> spawnedItems = new();

    [Inject]
    public void Construct(ChipHolder chipHolder)
    {
        this.chipHolder = chipHolder;
    }

    public void UpdateData()
    {
        ClearItems();

        foreach (var chip in chipHolder.Chips)
        {
            if (chip != null && chip.ChipData != null)
            {
                Sprite icon = chip.ChipData.Icon != null ? chip.ChipData.Icon : defaultIcon;
                SpawnItem(icon, chip.ChipData.Name);
            }
        }
    }

    private void SpawnItem(Sprite icon, string text)
    {
        var item = Instantiate(listItemPrefab, contentParent);
        item.Setup(icon, text);
        spawnedItems.Add(item);
    }

    private void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }
}