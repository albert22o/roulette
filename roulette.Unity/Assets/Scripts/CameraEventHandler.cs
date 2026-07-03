using System;
using UnityEngine;

public class CameraEventHandler : MonoBehaviour
{

    public event Action TransitionStarted;
    public event Action TransitionCompleted;

    public void OnTransitionStart()
    {
        TransitionStarted?.Invoke();
    }

    public void OnTransitionFinish()
    {
        TransitionCompleted?.Invoke();
    }
}