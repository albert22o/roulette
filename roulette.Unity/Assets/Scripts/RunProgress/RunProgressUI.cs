using System.Collections;
using UnityEngine;
using Zenject;

public class RunProgressUI : MonoBehaviour
{
    [SerializeField] private ChipsSectionUI chipsSection;
    [SerializeField] private BallsSectionUI ballsSection;
    [SerializeField] private PlaquesSectionUI plaquesSection;
    [SerializeField] private FoodSectionUI foodSection;
    [SerializeField] private QuotasSectionUI quotasSection;

    [SerializeField] private float displayDuration = 4f;
    private Coroutine hideCoroutine;
    private bool isPanelOpen = false;

    private void Start()
    {
        gameObject.SetActive(false);
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
        gameObject.SetActive(false);
        isPanelOpen = false;
        hideCoroutine = null;
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