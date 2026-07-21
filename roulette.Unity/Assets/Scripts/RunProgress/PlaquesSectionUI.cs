using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlaquesSectionUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private ListItemUI listItemPrefab;
    [SerializeField] private Sprite defaultIcon; 

    private PlaqueHolder plaqueHolder;
    private List<ListItemUI> spawnedItems = new();

    [Inject]
    public void Construct(PlaqueHolder plaqueHolder)
    {
        this.plaqueHolder = plaqueHolder;
    }

    public void UpdateData()
    {
        ClearItems();

        if (plaqueHolder.plaques != null)
        {
            foreach (var plaque in plaqueHolder.plaques)
            {
                SpawnItem(defaultIcon, plaque.GetType().Name);
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