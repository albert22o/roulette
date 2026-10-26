using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private InputActionReference pauseAction;

    private bool isPaused;
    private TimeScaleManager timeScaleManager;
    private CursorManager cursorManager;
    private PhysicsRaycaster physicsRaycaster;
    private MenuMusicPlayer menuMusic;

    [Inject]
    private void Construct(
        TimeScaleManager timeScaleManager,
        CursorManager cursorManager,
        PhysicsRaycaster physicsRaycaster,
        MenuMusicPlayer menuMusic)
    {
        this.timeScaleManager = timeScaleManager;
        this.cursorManager = cursorManager;
        this.physicsRaycaster = physicsRaycaster;
        this.menuMusic = menuMusic;
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
        isPaused = false;
        cursorManager.HideCursorRequest();
        timeScaleManager.SetPauseState(false);
        SetPhysicsRaycasterEnabled(true);
        menuMusic.Pause();

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
}