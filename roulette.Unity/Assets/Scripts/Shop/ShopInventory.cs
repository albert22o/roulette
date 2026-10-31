using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class ShopInventory : MonoBehaviour
{
    [SerializeField] private PlaqueItem itemPrefab;
    [SerializeField] private CustomWorldGridLayout ballsParent, chipsParent;

    private List<PlaqueItem> balls = new();
    private List<PlaqueItem> chips = new();

    private BallHolder ballHolder;
    private ChipHolder chipHolder;
    private Shop shop;
    private RarityData rarityData;
    private SFXManager sfxManager;

    [Inject]
    public void Construct(BallHolder ballHolder, ChipHolder chipHolder, Shop shop,
        RarityData rarityData, SFXManager sfxManager)
    {
        this.ballHolder = ballHolder;
        this.chipHolder = chipHolder;
        this.shop = shop;
        this.rarityData = rarityData;
        this.sfxManager = sfxManager;
    }

    private void OnEnable()
    {
        ballHolder.BallAdded += OnBallAdded;
        ballHolder.BallRemoved += OnBallRemoved;
        chipHolder.ChipAdded += OnChipAdded;
        chipHolder.ChipRemoved += OnChipRemoved;
        UpdateBalls();
        UpdateChips();
    }

    private void OnDisable()
    {
        ballHolder.BallAdded -= OnBallAdded;
        ballHolder.BallRemoved -= OnBallRemoved;
        chipHolder.ChipAdded -= OnChipAdded;
        chipHolder.ChipRemoved -= OnChipRemoved;
    }

    private void UpdateBalls()
    {
        for (int i = balls.Count - 1; i >= 0; i--)
            Destroy(balls[i].gameObject);
        balls.Clear();
        for (int i = 0; i < ballHolder.Balls.Count; i++)
        {
            var newBall = Instantiate(itemPrefab, ballsParent.transform);
            newBall.SetData(ballHolder.Balls[i].BallInfoData);
            newBall.HoverEntered += OnHoverEnter;
            newBall.HoverExited += OnHoverExit;
            newBall.Clicked += OnClick;
            balls.Add(newBall);
        }
    }

    private void OnClick(PlaqueItem item)
    {
        if (item.Data is BallInfoData)
        {
            if (shop.TrySell(item.Data))
            {
                sfxManager.Play("Sell");
            }
        }
        else
        {
            if (shop.TrySell(item.Data))
            {
                sfxManager.Play("Sell");
            }
        }
    }

    private void OnHoverExit(PlaqueItem item)
    {
        shop.HideTooltip();
    }

    private void OnHoverEnter(PlaqueItem item)
    {
        string description = item.Data.Description;
        string cost = shop.GetSellPrice(item.Data).ToString();
        shop.ShowTooltip(item.Data.Name, description, cost, item.transform.position, item.Data.RarityType);
    }

    private void UpdateChips()
    {
        for (int i = chips.Count - 1; i >= 0; i--)
            Destroy(chips[i].gameObject);
        chips.Clear();
        for (int i = 0; i < chipHolder.Chips.Count; i++)
        {
            if (chipHolder.Chips[i] != null)
            {
                var newChip = Instantiate(itemPrefab, chipsParent.transform);
                newChip.HoverEntered += OnHoverEnter;
                newChip.HoverExited += OnHoverExit;
                newChip.Clicked += OnClick;
                newChip.SetData(chipHolder.Chips[i].ChipData);
                chips.Add(newChip);
            }
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
        UpdateChips();
    }

    private void OnChipRemoved(int index)
    {
        UpdateChips();
    }
}