using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] RoundScopeView roundScopeView;
    [SerializeField] FirstPersonController firstPersonController;
    void Awake()
    {
        roundScopeView.Init(mainCamera);
        firstPersonController.Init();
    }
}