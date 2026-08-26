using System.Collections.Generic;
using System.Linq;
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

        if (plaqueHolder.plaques != null && plaqueHolder.plaques.Count > 0)
        {
            var groupedPlaque = plaqueHolder.plaques.GroupBy(p => p.PlaqueData.GetType()).Select(g => new { PlaqueType = g.Key, Count = g.Count(), FirstPlaqueData = g.First().PlaqueData });

            foreach (var group in groupedPlaque)
            {
                Sprite icon = GetPlaqueIcon(group.FirstPlaqueData);
                string text = $"{group.PlaqueType.Name} x{group.Count}";
                SpawnItem(icon, text);
            }
        }
    }

    private Sprite GetPlaqueIcon(PlaqueData plaqueData)
    {
        if (plaqueData != null && plaqueData.Icon != null)
        {
            return plaqueData.Icon;
        }
        return defaultIcon;
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