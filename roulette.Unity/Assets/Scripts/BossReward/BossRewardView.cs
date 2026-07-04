using UnityEngine;
using Zenject;

public class BossRewardView : MonoBehaviour
{
    [SerializeField] private PlaqueItem[] itemPlaces;

    private BossReward bossReward;
    private ItemData[] itemsData;

    [Inject]
    public void Construct(BossReward bossReward)
    {
        this.bossReward = bossReward;
    }

    private void Start()
    {
        foreach (var item in itemPlaces)
        {
            item.HoverEntered += OnItemHoverEnter;
            item.HoverExited += OnItemHoverExit;
            item.Clicked += OnItemClick;
        }
    }

    private void OnEnable()
    {
        bossReward.RewardCreated += OnRewardReceived;
    }

    private void OnDisable()
    {
        bossReward.RewardCreated -= OnRewardReceived;
    }

    private void OnRewardReceived(ItemData[] items)
    {
        itemsData = items;
        UpdateItemPlaces();
    }

    private void UpdateItemPlaces()
    {
        for (int i = 0; i < itemPlaces.Length; i++)
        {
            itemPlaces[i].SetData(itemsData[i]);
        }
    }

    private void OnItemHoverEnter(PlaqueItem item)
    {

    }

    private void OnItemHoverExit(PlaqueItem item)
    {

    }

    private void OnItemClick(PlaqueItem item)
    {

    }
}
