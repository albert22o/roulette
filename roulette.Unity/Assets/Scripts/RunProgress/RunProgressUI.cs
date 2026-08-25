using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class RunProgressUI : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private ChipsSectionUI chipsSection;
    [SerializeField] private BallsSectionUI ballsSection;
    [SerializeField] private PlaquesSectionUI plaquesSection;
    [SerializeField] private FoodSectionUI foodSection;
    [SerializeField] private QuotasSectionUI quotasSection;
    [SerializeField] private CardsSectionUI cardsSection;

    [SerializeField] private float displayDuration = 4f;
    private FirstPersonController playerController;

    private InputAction toggleAction;
    private Coroutine hideCoroutine;
    private bool isPanelOpen = false;

    private CursorLockMode savedCursorLockState;
    private bool savedCursorVisible;

    [Inject]
    public void Construct(FirstPersonController firstPersonController)
    {
        playerController = firstPersonController;
    }

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
            Hide();
        else
            Show();
    }

    public void Show()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        savedCursorLockState = Cursor.lockState;
        savedCursorVisible = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameObject.SetActive(true);
        isPanelOpen = true;

        UpdateAllSections();

        playerController.Disable();
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
        playerController.Enable();

        Cursor.lockState = savedCursorLockState;
        Cursor.visible = savedCursorVisible;
    }

    private void UpdateAllSections()
    {
        chipsSection?.UpdateData();
        ballsSection?.UpdateData();
        plaquesSection?.UpdateData();
        foodSection?.UpdateData();
        quotasSection?.UpdateData();
        cardsSection?.UpdateData();
    }
}