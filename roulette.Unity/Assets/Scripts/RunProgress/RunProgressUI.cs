using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Zenject.Asteroids;
using static GameStateManager;

public class RunProgressUI : MonoBehaviour
{
    [SerializeField] private ChipsSectionUI chipsSection;
    [SerializeField] private BallsSectionUI ballsSection;
    [SerializeField] private PlaquesSectionUI plaquesSection;
    [SerializeField] private FoodSectionUI foodSection;
    [SerializeField] private QuotasSectionUI quotasSection;

    [SerializeField] private float displayDuration = 4f;
    [Inject] private GameStateManager gameStateManager;

    private InputAction toggleAction;
    private Coroutine hideCoroutine;
    private bool isPanelOpen = false;
    private GameState previousState;

    private void Start()
    {
        gameObject.SetActive(false);
        SetupInput();
    }

    private void SetupInput()
    {
        toggleAction = new InputAction("ToggleProgress", binding: "<Keyboard>/tab");
        toggleAction.started += _ => TogglePanel();
        toggleAction.Enable();
    }

    private void OnDestroy()
    {
        toggleAction?.Disable();
        toggleAction?.Dispose();
    }

    public void TogglePanel()
    {
        if (isPanelOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        gameObject.SetActive(true);
        isPanelOpen = true;

        UpdateAllSections();

        previousState = gameStateManager.CurrentState ?? GameStateManager.GameState.FreeRoam;
        gameStateManager.ChangeGameState(GameStateManager.GameState.ViewingProgress);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        Hide();
    }

    public void Hide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
        gameObject.SetActive(false);
        isPanelOpen = false;

        gameStateManager.ChangeGameState(previousState);
    }

    private void UpdateAllSections()
    {
        chipsSection?.UpdateData();
        ballsSection?.UpdateData();
        plaquesSection?.UpdateData();
        foodSection?.UpdateData();
        quotasSection?.UpdateData();
    }
}