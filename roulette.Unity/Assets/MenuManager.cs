using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private GameObject startCam;

    private bool isPaused;
    private TimeScaleManager timeScaleManager;
    private CursorManager cursorManager;
    private PhysicsRaycaster physicsRaycaster;
    private MenuMusicPlayer menuMusic;
    private SFXManager SFXManager;

    [Inject]
    public void Construct(
        TimeScaleManager timeScaleManager,
        CursorManager cursorManager,
        PhysicsRaycaster physicsRaycaster,
        MenuMusicPlayer menuMusic,
        SFXManager SFXManager)
    {
        this.timeScaleManager = timeScaleManager;
        this.cursorManager = cursorManager;
        this.physicsRaycaster = physicsRaycaster;
        this.menuMusic = menuMusic;
        this.SFXManager = SFXManager;
    }

    public void ChangeLanguage()
    {
        SFXManager.Play("ButtonClick");
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
        SFXManager.Play("ButtonClick");
        Application.Quit();
    }

    public void TogglePause()
    {
        isPaused = true;
        timeScaleManager.SetPauseState(true);
        cursorManager.ShowCursorRequest();
        SetPhysicsRaycasterEnabled(false);
        menuMusic.Play();

        if (menuPanel != null)
            menuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (startCam.activeInHierarchy)
            StartCoroutine(DisableStartCamWithDelay(0.1f));
        SFXManager.Play("ButtonClick");
        isPaused = false;
        cursorManager.HideCursorRequest();
        timeScaleManager.SetPauseState(false);
        SetPhysicsRaycasterEnabled(true);

        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPauseButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPauseButtonPressed;
        }

        // Страховка: если менеджер отключили во время паузы, не оставляем 3D-ввод выключенным
        if (isPaused)
            SetPhysicsRaycasterEnabled(true);
    }

    private void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (isPaused)
            ResumeGame();
        else
            TogglePause();
    }

#if !UNITY_EDITOR
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !isPaused)
        {
            TogglePause();
        }
    }
#endif

    private void SetPhysicsRaycasterEnabled(bool value)
    {
        if (physicsRaycaster != null)
            physicsRaycaster.enabled = value;
    }

    private void Start()
    {
        TogglePause();
    }

    private IEnumerator DisableStartCamWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        startCam.SetActive(false);
    }
}