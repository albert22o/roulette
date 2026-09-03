using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

public class BossRewardView : MonoBehaviour
{
    [SerializeField] private PlaqueItem[] itemPlaces;
    [SerializeField] private ShopTooltip tooltip;
    [SerializeField] private CinemachineCamera bossRewardCamera;
    [SerializeField] private MouseInteraction mouseInteraction;
    [SerializeField] private float tooltipHeight;
    [SerializeField] private float delayBetweenChipPickupAndItemsSpawn = 1f;
    [SerializeField] private float delayBetweenItemsSpawn = 0.2f;

    private BossReward bossReward;
    private ItemData[] itemsData;
    private ItemReceiver itemReceiver;
    private GameStateManager gameStateManager;
    private CameraEventHandler cameraEventHandler;
    private bool isInteractable;
    private RarityData rarityData;

    public event Action RewardReceived;

    [Inject]
    public void Construct(BossReward bossReward, ItemReceiver itemReceiver,
        GameStateManager gameStateManager, CameraEventHandler cameraEventHandler, RarityData rarityData)
    {
        this.bossReward = bossReward;
        this.itemReceiver = itemReceiver;
        this.gameStateManager = gameStateManager;
        this.cameraEventHandler = cameraEventHandler;
        this.rarityData = rarityData;

        bossReward.RewardCreated += OnRewardReceived;
        gameStateManager.GameStateChanged += OnGameStateChanged;
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

    private void OnRewardReceived(ItemData[] items)
    {
        itemsData = items;
        isInteractable = false;
        cameraEventHandler.TransitionFinished += OnCameraTransitionFinished;
    }


    private void OnItemHoverEnter(PlaqueItem item)
    {
        string description = item.Data.Description;
        if (item.Data.RarityType != RarityType.None)
        {
            var rarity = rarityData.GetRarityDisplayData(item.Data.RarityType);
            description += "\nРедкость: <color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(rarity.Color) + ">" + rarity.Name + "</color>";
        }
        tooltip.Show(item.Data.Name, description, item.transform.position);
    }

    private void OnItemHoverExit(PlaqueItem item)
    {
        tooltip.Hide();
    }

    private void OnItemClick(PlaqueItem item)
    {
        if (isInteractable == false) return;
        itemReceiver.ReceiveItem(item.Data);
        ClearAllItems();
        gameStateManager.ChangeGameState(GameStateManager.GameState.FreeRoam);
    }

    private void ClearAllItems()
    {
        foreach (var item in itemPlaces)
        {
            item.gameObject.SetActive(false);
            item.SetData(null);
        }
    }

    private void OnGameStateChanged(GameStateManager.GameState newState)
    {
        if (newState == GameStateManager.GameState.BossReward)
        {
            bossRewardCamera.gameObject.SetActive(true);
            mouseInteraction.enabled = true;
        }
        else
        {
            bossRewardCamera.gameObject.SetActive(false);
            mouseInteraction.enabled = false;
        }
    }

    public void OnBossChipPickedUp()
    {
        StartCoroutine(SpawnItems());
        bossReward.GiveBossChips();
    }

    private IEnumerator SpawnItems()
    {
        yield return new WaitForSeconds(delayBetweenChipPickupAndItemsSpawn);
        for (int i = 0; i < itemPlaces.Length; i++)
        {
            itemPlaces[i].SetData(itemsData[i]);
            itemPlaces[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(delayBetweenItemsSpawn);
        }
        isInteractable = true;
    }

    private void OnCameraTransitionFinished()
    {
        RewardReceived?.Invoke();
        cameraEventHandler.TransitionFinished -= OnCameraTransitionFinished;
    }
}
