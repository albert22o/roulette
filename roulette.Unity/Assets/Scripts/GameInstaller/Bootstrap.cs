using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] RoundScopeView roundScopeView;
    [SerializeField] FirstPersonController firstPersonController;
    void Awake()
    {
        roundScopeView.Init();
        firstPersonController.Init();
    }
}