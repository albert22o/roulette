using System.Collections;
using UnityEngine;

public class RunProgressUIView : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private ChipsSectionUI chipsSection;
    [SerializeField] private BallsSectionUI ballsSection;
    [SerializeField] private PlaquesSectionUI plaquesSection;
    [SerializeField] private FoodSectionUI foodSection;
    [SerializeField] private QuotasSectionUI quotasSection;
    [SerializeField] private CardsSectionUI cardsSection;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;

    public float DisplayDuration => displayDuration;

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateAllSections();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
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

    public Coroutine StartCustomCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopCustomCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}