using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BallsSectionUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private ListItemUI listItemPrefab;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private bool spawnTestItems = false;

    private BallHolder ballHolder;
    private List<ListItemUI> spawnedItems = new();

    [Inject]
    public void Construct(BallHolder ballHolder)
    {
        this.ballHolder = ballHolder;
    }

    public void UpdateData()
    {
        ClearItems();

        if (spawnTestItems)
        {
            SpawnItem(defaultIcon, "Тестовый шарик 1");
            SpawnItem(defaultIcon, "Тестовый шарик 2");
            return;
        }

        if (ballHolder.Balls != null)
        {
            foreach (var ball in ballHolder.Balls)
            {
                if (ball?.BallInfoData != null)
                {
                    Sprite icon = ball.BallInfoData.Icon != null ? ball.BallInfoData.Icon : defaultIcon;
                    SpawnItem(icon, ball.BallInfoData.Name);
                }
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