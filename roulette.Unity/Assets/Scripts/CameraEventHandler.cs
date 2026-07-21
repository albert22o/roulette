using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraEventHandler
{
    public event Action TransitionStarted;
    public event Action TransitionFinished;

    public bool IsTransitioning { get; private set; } = false;

    public CameraEventHandler()
    {
        CinemachineCore.BlendCreatedEvent.AddListener(OnBlendCreated);
        CinemachineCore.BlendFinishedEvent.AddListener(OnBlendFinished);
    }

    private void OnBlendFinished(ICinemachineMixer arg0, ICinemachineCamera arg1)
    {
        IsTransitioning = false;
        TransitionFinished?.Invoke();
    }

    private void OnBlendCreated(CinemachineCore.BlendEventParams arg0)
    {
        IsTransitioning = true;
        TransitionStarted?.Invoke();
    }
}