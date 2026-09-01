using System;
using UnityEngine.InputSystem;
using Zenject;

public class RunProgress
{
    public event Action Shown;
    public event Action Hidden;

    public bool IsPanelOpen { get; private set; }

    private readonly FirstPersonController playerController;
    private InputAction holdAction;

    [Inject]
    public RunProgress(FirstPersonController playerController)
    {
        this.playerController = playerController;
        SetupInput();
    }

    private void SetupInput()
    {
        holdAction = new InputAction("HoldProgress", binding: "<Keyboard>/tab");
        holdAction.started += _ => Show();    
        holdAction.canceled += _ => Hide();  
        holdAction.Enable();
    }

    private void Show()
    {
        if (IsPanelOpen) return;
        IsPanelOpen = true;

        playerController.Disable();
        Shown?.Invoke();
    }

    private void Hide()
    {
        if (!IsPanelOpen) return;
        IsPanelOpen = false;

        playerController.Enable();
        Hidden?.Invoke();
    }
}