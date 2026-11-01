using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class RoundStateManagerView : MonoBehaviour
{
    [SerializeField] CinemachineCamera roundCamera;
    [SerializeField] CinemachineCamera rouletteCamera;
    [SerializeField] CinemachineCamera betsCamera;

    private CancelInputHandler cancelInputHandler;
    private RoundStateManager roundStateManager;
    private WinningSequence winningSequence;

    private bool isCancelLockedInThisView = false;

    [Inject]
    public void Construct(RoundStateManager roundStateManager, WinningSequence winningSequence, CancelInputHandler cancelInputHandler)
    {
        this.roundStateManager = roundStateManager;
        this.winningSequence = winningSequence;
        this.cancelInputHandler = cancelInputHandler;
    }

    public void Awake()
    {
        roundStateManager.StateChanged += OnStateChanged;
        OnStateChanged(roundStateManager.CurrentState);
    }

    private void OnStateChanged(RoundStateManager.RoundState state)
    {
        UpdateCameras(state);

        bool shouldLock = DetermineIfShouldLock(state);
        ApplyCancelLock(shouldLock);
    }

    private void UpdateCameras(RoundStateManager.RoundState state)
    {
        Debug.Log(state);
        switch (state)
        {
            case RoundStateManager.RoundState.NotActive:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(false);
                break;
            case RoundStateManager.RoundState.WheelSpining:
            case RoundStateManager.RoundState.ChoosingReward:
                rouletteCamera.gameObject.SetActive(true);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(false);
                break;
            case RoundStateManager.RoundState.BetResults:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                betsCamera.gameObject.SetActive(true);
                break;
            case RoundStateManager.RoundState.CountMoney:
            default:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(true);
                betsCamera.gameObject.SetActive(false);
                break;
        }
    }

    private bool DetermineIfShouldLock(RoundStateManager.RoundState state)
    {
        return state == RoundStateManager.RoundState.NotActive ||
               state == RoundStateManager.RoundState.WheelSpining ||
               state == RoundStateManager.RoundState.ChoosingReward ||
               state == RoundStateManager.RoundState.BetResults ||
               state == RoundStateManager.RoundState.CountMoney;
    }

    private void ApplyCancelLock(bool shouldLock)
    {
        if (shouldLock && !isCancelLockedInThisView)
        {
            isCancelLockedInThisView = true;
            cancelInputHandler.LockCancel();
        }
        else if (!shouldLock && isCancelLockedInThisView)
        {
            isCancelLockedInThisView = false;
            cancelInputHandler.UnlockCancel();
        }
    }
}
