using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ShopInventory : MonoBehaviour
{
    [SerializeField] private ShopInventoryItem ballPrefab;
    [SerializeField] private ShopInventoryItem chipPrefab;
    [SerializeField] private Transform[] ballSpawnPoints, chipSpawnPoints;

    private readonly List<ShopInventoryItem> balls = new List<ShopInventoryItem>();
    private readonly List<ShopInventoryItem> chips = new List<ShopInventoryItem>();

    private Inventory inventory;
    private DiContainer container;

    [Inject]
    public void Construct(Inventory inventory, DiContainer container)
    {
        this.inventory = inventory;
        this.container = container;
    }

    private void OnEnable()
    {
        inventory.ItemsChanged += OnItemsChanged;
    }

    private void OnDisable()
    {
        inventory.ItemsChanged -= OnItemsChanged;
    }

    private void OnItemsChanged()
    {
        UpdateBalls();
        UpdateChips();
    }

    private void UpdateBalls()
    {
        for (int i = balls.Count - 1; i >= 0; i--)
        {
            var visual = balls[i];

            bool stillExists = inventory.Balls.Any(invBall => invBall.Id == visual.Item.Id);

            if (!stillExists)
            {
                Destroy(visual.gameObject);
                balls.RemoveAt(i);
            }
        }

        foreach (var inventoryBall in inventory.Balls)
        {
            bool exists = balls.Any(visual => visual.Item.Id == inventoryBall.Id);

            if (!exists)
            {
                var newBall = container.InstantiatePrefabForComponent<ShopInventoryItem>(ballPrefab);
                newBall.transform.position = GetFreeBallSpawnPoint().position;
                newBall.SetItem(inventoryBall);
                newBall.SetVisualComponent((inventoryBall.Data as BallItemData).VisualComponent);

                balls.Add(newBall);
            }
        }
    }

    private Transform GetFreeBallSpawnPoint()
    {
        foreach (var point in ballSpawnPoints)
        {
            bool occupied = balls.Any(b => b.transform.position == point.position);
            if (!occupied)
                return point;
        }

        return ballSpawnPoints[0];
    }

    private void UpdateChips()
    {
        for (int i = chips.Count - 1; i >= 0; i--)
        {
            var visual = chips[i];

            bool isStillExists = inventory.Chips.Any(invChip => invChip.Id == visual.Item.Id);

            if (!isStillExists)
            {
                Destroy(visual.gameObject);
                chips.RemoveAt(i);
            }
        }

        foreach (var inventoryChip in inventory.Chips)
        {
            bool exists = chips.Any(visual => visual.Item.Id == inventoryChip.Id);

            if (!exists)
            {
                var newChip = container.InstantiatePrefabForComponent<ShopInventoryItem>(chipPrefab);
                newChip.transform.position = GetFreeChipSpawnPoint().position;
                newChip.SetItem(inventoryChip);
                newChip.SetVisualComponent((inventoryChip.Data as BallItemData).VisualComponent);

                chips.Add(newChip);
            }
        }
    }

    private Transform GetFreeChipSpawnPoint()
    {
        foreach (var point in chipSpawnPoints)
        {
            bool isOccupied = chips.Any(b => b.transform.position == point.position);
            if (!isOccupied)
                return point;
        }

        return chipSpawnPoints[0];
    }
}
