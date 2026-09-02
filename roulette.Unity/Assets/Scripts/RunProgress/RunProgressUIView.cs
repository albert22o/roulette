using UnityEngine;
using Zenject;

public class RunProgressUIView : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private ChipsSectionUI chipsSection;
    [SerializeField] private BallsSectionUI ballsSection;
    [SerializeField] private PlaquesSectionUI plaquesSection;
    [SerializeField] private FoodSectionUI foodSection;
    [SerializeField] private QuotasSectionUI quotasSection;
    [SerializeField] private CardsSectionUI cardsSection;

    private RunProgress runProgress;

    private CursorLockMode savedCursorLockState;
    private bool savedCursorVisible;

    [Inject]
    public void Construct(RunProgress runProgress)
    {
        this.runProgress = runProgress;

        runProgress.Shown += OnShown;
        runProgress.Hidden += OnHidden;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (runProgress != null)
        {
            runProgress.Shown -= OnShown;
            runProgress.Hidden -= OnHidden;
        }
    }

    private void OnShown()
    {
        savedCursorLockState = Cursor.lockState;
        savedCursorVisible = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameObject.SetActive(true);
        UpdateAllSections();
    }

    private void OnHidden()
    {
        gameObject.SetActive(false);

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