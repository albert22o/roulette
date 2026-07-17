using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FoodSectionUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private ListItemUI listItemPrefab;
    [SerializeField] private Sprite defaultIcon; 
    [SerializeField] private bool spawnTestItems = false;

    private FoodHolder foodHolder;
    private List<ListItemUI> spawnedItems = new();

    [Inject]
    public void Construct(FoodHolder foodHolder)
    {
        this.foodHolder = foodHolder;
    }

    public void UpdateData()
    {
        ClearItems();

        if (spawnTestItems)
        {
            SpawnItem(defaultIcon, "Тестовая еда 1");
            SpawnItem(defaultIcon, "Тестовая еда 2");
            SpawnItem(defaultIcon, "Тестовая еда 3");
            return;
        }

        if (foodHolder.AppliedHistory != null)
        {
            foreach (var food in foodHolder.AppliedHistory)
            {
                SpawnItem(defaultIcon, food.GetType().Name);
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