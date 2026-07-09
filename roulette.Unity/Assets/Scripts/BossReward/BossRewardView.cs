using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

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
    private BossChips bossChips;

    public event Action RewardReceived;

    [Inject]
    public void Construct(BossReward bossReward, ItemReceiver itemReceiver, GameStateManager gameStateManager, CameraEventHandler cameraEventHandler,
        BossChips bossChips)
    {
        this.bossReward = bossReward;
        this.itemReceiver = itemReceiver;
        this.gameStateManager = gameStateManager;
        this.cameraEventHandler = cameraEventHandler;
        this.bossChips = bossChips;
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
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        bossReward.RewardCreated -= OnRewardReceived;
        gameStateManager.GameStateChanged -= OnGameStateChanged;
    }

    private void OnRewardReceived(ItemData[] items)
    {
        itemsData = items;
        isInteractable = false;
        cameraEventHandler.TransitionFinished += OnCameraTransitionFinished;
    }


    private void OnItemHoverEnter(PlaqueItem item)
    {
        tooltip.SetData(item.Data.Name, item.Data.Description);
        tooltip.transform.position = item.transform.position + Vector3.up * tooltipHeight;
        tooltip.gameObject.SetActive(true);
    }

    private void OnItemHoverExit(PlaqueItem item)
    {
        tooltip.gameObject.SetActive(false);
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
        bossChips.Add(1);
    }

    private IEnumerator SpawnItems()
    {
        yield return new WaitForSeconds(delayBetweenChipPickupAndItemsSpawn);
        for (int i = 0; i < itemPlaces.Length; i++)
        {
            itemPlaces[i].SetData(itemsData[i]);
            yield return new WaitForSeconds(delayBetweenItemsSpawn);
        }
        isInteractable = true;
    }

    private void OnCameraTransitionFinished()
    {
        Debug.Log("cameraTransitinoFinished");
        RewardReceived?.Invoke();
        cameraEventHandler.TransitionFinished -= OnCameraTransitionFinished;
    }
}
