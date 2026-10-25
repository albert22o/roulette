using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class BossRewardView : MonoBehaviour
{
    [SerializeField] private PlaqueItem[] itemPlaces;
    [SerializeField] private BossRewardTooltip tooltip;
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

    [Header("Default Burn Settings")]
    [SerializeField] private Material defaultBurnMaterial;
    [SerializeField] private float defaultBurnDuration = 1.5f;
    [SerializeField] private string defaultDissolveProperty = "_Disolve";

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
            description += Loc.Get("tooltip.rarity", UnityEngine.ColorUtility.ToHtmlStringRGB(rarity.Color), rarity.Name);
        }
        tooltip.Show(item.Data.Name, description, item.Data.RarityType, item.transform.position);
    }

    private void OnItemHoverExit(PlaqueItem item)
    {
        tooltip.Hide();
    }

    private void OnItemClick(PlaqueItem item)
    {
        if (isInteractable == false) return;

        isInteractable = false;
        itemReceiver.ReceiveItem(item.Data);

        foreach (var place in itemPlaces)
        {
            if (!place.gameObject.activeSelf) continue;

            if (place == item)
            {
                StartCoroutine(DelayedHide(place.gameObject, 1.5f));
                continue;
            }

            var burnEffect = place.GetComponentInChildren<BurnEffect>();
            Action onComplete = null;

            if (burnEffect == null)
            {
                // Эффекта нет — вешаем временный с настройками по умолчанию
                var temp = place.gameObject.AddComponent<BurnEffect>();
                temp.Configure(defaultBurnMaterial, defaultBurnDuration, defaultDissolveProperty);
                burnEffect = temp;
                onComplete = () => Destroy(temp); // убираем после горения
            }

            burnEffect.StartBurn(onComplete);
        }

        StartCoroutine(WaitForBurnAndChangeState(2.5f));
    }

    private System.Collections.IEnumerator DelayedHide(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private System.Collections.IEnumerator WaitForBurnAndChangeState(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameStateManager.ChangeGameState(GameStateManager.GameState.FreeRoam);
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
