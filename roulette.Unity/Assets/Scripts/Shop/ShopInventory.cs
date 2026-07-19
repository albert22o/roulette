using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ShopInventory : MonoBehaviour
{
    [SerializeField] private ShopInventoryItem itemPrefab;
    [SerializeField] private Transform[] ballSpawnPoints, chipSpawnPoints;

    private ShopInventoryItem[] balls;
    private ShopInventoryItem[] chips;

    private BallHolder ballHolder;
    private ChipHolder chipHolder;
    private Shop shop;

    [Inject]
    public void Construct(BallHolder ballHolder, ChipHolder chipHolder, Shop shop)
    {
        this.ballHolder = ballHolder;
        this.chipHolder = chipHolder;
        this.shop = shop;
    }

    private void OnEnable()
    {
        ballHolder.BallAdded += OnBallAdded;
        ballHolder.BallRemoved += OnBallRemoved;
        chipHolder.ChipAdded += OnChipAdded;
        chipHolder.ChipRemoved += OnChipRemoved;

        chipHolder.SlotAdded += OnChipSlotAdded;
        chipHolder.SlotRemoved += OnChipSlotRemoved;

        for (int index = 0; index < chipHolder.Chips.Count; index++)
        {
            if (chipHolder.Chips[index] == null)
                continue;

            OnChipAdded(index, chipHolder.Chips[index]);
        }
    }
    private void OnChipSlotAdded(int index)
    {
        UpdateChips();
    }

    private void OnChipSlotRemoved(int index)
    {
        UpdateChips();
    }

    private void UpdateChips()
    {
        for (int i = 0; i < chips.Length; i++)
        {
            if (i < chipHolder.Chips.Count && chipHolder.Chips[i] != null)
                chips[i].SetItem(chipHolder.Chips[i].ChipData);
            else
                chips[i].SetItem(null);
        }
    }

    private void UpdateBalls()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            if (i < ballHolder.Balls.Count)
                balls[i].SetItem(ballHolder.Balls[i].BallData);
            else
                balls[i].SetItem(null);
        }
    }

    private void OnBallAdded(int index, BallInfo info)
    {
        UpdateBalls();
    }

    private void OnBallRemoved(int index)
    {
        UpdateBalls();
    }

    private void OnChipAdded(int index, Chip chip)
    {
        chips[index].SetItem(chip.ChipData);
    }

    private void OnChipRemoved(int index)
    {
        chips[index].SetItem(null);
    }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        balls = new ShopInventoryItem[ballSpawnPoints.Length];
        chips = new ShopInventoryItem[chipSpawnPoints.Length];
        for (int i = 0; i < ballSpawnPoints.Length; i++)
        {
            balls[i] = Instantiate(itemPrefab, ballSpawnPoints[i]);
            balls[i].transform.position = ballSpawnPoints[i].position;
            balls[i].Index = i;
            balls[i].SetShop(shop);
        }
        for (int i = 0; i < chipSpawnPoints.Length; i++)
        {
            chips[i] = Instantiate(itemPrefab, chipSpawnPoints[i]);
            chips[i].transform.position = chipSpawnPoints[i].position;
            chips[i].Index = i;
            chips[i].SetShop(shop);
        }
        UpdateBalls();
    }
}
