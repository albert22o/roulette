using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraEventHandler
{
    public event Action TransitionStarted;
    public event Action TransitionFinished;

    public CameraEventHandler()
    {
        CinemachineCore.BlendCreatedEvent.AddListener(OnBlendCreated);
        CinemachineCore.BlendFinishedEvent.AddListener(OnBlendFinished);
    }

    private void OnBlendFinished(ICinemachineMixer arg0, ICinemachineCamera arg1) => TransitionFinished?.Invoke();
    private void OnBlendCreated(CinemachineCore.BlendEventParams arg0) => TransitionStarted?.Invoke();
}