using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CardsSectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private ListItemUI listItemPrefab;
    [SerializeField] private Sprite defaultIcon;

    private CardHolder cardHolder;
    private readonly List<ListItemUI> spawnedItems = new();

    [Inject]
    public void Construct(CardHolder cardCollection)
    {
        this.cardHolder = cardCollection;
    }

    public void UpdateData()
    {
        ClearItems();

        var cards = cardHolder.GetCollectedCards();

        foreach (var item in cards)
        {
            if (item.Data is CardData cardData)
            {
                Sprite icon = cardData.Icon != null ? cardData.Icon : defaultIcon;
                string displayName = string.IsNullOrEmpty(cardData.Name)
                    ? $"Карта x{cardData.Multiplier}"
                    : $"{cardData.Name} (x{cardData.Multiplier})";

                SpawnItem(icon, displayName);
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
            if (item != null)
                Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }
}