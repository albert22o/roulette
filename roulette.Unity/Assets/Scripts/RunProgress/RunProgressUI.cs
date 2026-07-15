using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class RunProgressUI : MonoBehaviour
{
    [Header("Sections (Non-Currency)")]
    [SerializeField] private ChipsSectionUI chipsSection;
    [SerializeField] private BallsSectionUI ballsSection;
    [SerializeField] private PlaquesSectionUI plaquesSection;
    [SerializeField] private FoodSectionUI foodSection;
    [SerializeField] private QuotasSectionUI quotasSection;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;

    private InputAction toggleAction;
    private Coroutine hideCoroutine;
    private bool isPanelOpen = false;

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