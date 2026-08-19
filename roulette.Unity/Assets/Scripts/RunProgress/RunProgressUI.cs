using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class RunProgress
{
    private readonly RunProgressUIView view;
    private readonly IInputFreezer inputFreezer;

    private InputAction toggleAction;
    private Coroutine hideCoroutine;
    private bool isPanelOpen = false;

    [Inject]
    public RunProgress(RunProgressUIView view, IInputFreezer inputFreezer)
    {
        this.view = view;
        this.inputFreezer = inputFreezer;

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

        inputFreezer.FreezeInput();

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

        inputFreezer.UnfreezeInput();
    }
}