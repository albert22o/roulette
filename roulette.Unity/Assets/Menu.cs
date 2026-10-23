using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Zenject;

public class Menu : MonoBehaviour
{

    private TimeScaleManager timeScaleManager;

    [Inject]
    private void Construct(TimeScaleManager timeScaleManager)
    {
        this.timeScaleManager = timeScaleManager;
    }

    public void ChangeLanguage()
    {
        StartCoroutine(NextLanguage());
    }
    public IEnumerator NextLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;
        int current = locales.IndexOf(LocalizationSettings.SelectedLocale);
        LocalizationSettings.SelectedLocale = locales[(current + 1) % locales.Count];
    }

    public void Quit()
    {
        Application.Quit();
    }
}
