using System;
using UnityEngine;

public class CameraEventHandler : MonoBehaviour
{

    public event Action TransitionStarted;
    public event Action TransitionCompleted;

    public void OnTransitionStart()
    {
        TransitionStarted?.Invoke();
        Debug.Log("transitionStarted");
    }

    public void OnTransitionFinish()
    {
        TransitionCompleted?.Invoke();
        Debug.Log("transitionCompleted");
    }

}