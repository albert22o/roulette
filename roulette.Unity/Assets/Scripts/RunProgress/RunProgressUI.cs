using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class RunProgress
{
    private readonly RunProgressUIView view;
    private readonly GameStateManager gameStateManager;

    private InputAction toggleAction;
    private Coroutine hideCoroutine;
    private bool isPanelOpen = false;
    private GameStateManager.GameState? previousState = null;

    [Inject]
    public RunProgress(RunProgressUIView view, GameStateManager gameStateManager)
    {
        this.view = view;
        this.gameStateManager = gameStateManager;

        view.gameObject.SetActive(false);
        SetupInput();
    }

    private void SetupInput()
    {
        toggleAction = new InputAction("ToggleProgress", binding: "<Keyboard>/tab");
        toggleAction.started += _ => TogglePanel();
        toggleAction.Enable();
    }

    public void TogglePanel()
    {
        if (isPanelOpen)
            Hide();
        else
            Show();
    }

    public void Show()
    {
        if (hideCoroutine != null)
            view.StopCustomCoroutine(hideCoroutine);

        previousState = gameStateManager.CurrentState;

        gameStateManager.ChangeGameState(GameStateManager.GameState.RunProgress);

        view.Show();
        isPanelOpen = true;

        hideCoroutine = view.StartCustomCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(view.DisplayDuration);
        Hide();
    }

    public void Hide()
    {
        if (hideCoroutine != null)
        {
            view.StopCustomCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        view.Hide();
        isPanelOpen = false;

        if (previousState.HasValue)
        {
            gameStateManager.ChangeGameState(previousState.Value);
            previousState = null;
        }
    }
}