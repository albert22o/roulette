using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class RoundStateManagerView : MonoBehaviour
{
    [SerializeField] CinemachineCamera roundCamera;
    [SerializeField] CinemachineCamera rouletteCamera;

    private RoundStateManager roundStateManager;

    [Inject]
    public void Construct(RoundStateManager roundStateManager)
    {
        this.roundStateManager = roundStateManager;
    }

    public void Init()
    {
        roundStateManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(RoundStateManager.BetState state)
    {
        switch (state) 
        {
            case RoundStateManager.BetState.NotActive:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(false);
                break;
            case RoundStateManager.BetState.WheelSpining:
                rouletteCamera.gameObject.SetActive(true);
                roundCamera.gameObject.SetActive(false);
                break;
            default:
                rouletteCamera.gameObject.SetActive(false);
                roundCamera.gameObject.SetActive(true);
                break;
        }
    }
}
