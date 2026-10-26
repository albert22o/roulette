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

    [Inject]
    private void Construct(
        TimeScaleManager timeScaleManager,
        CursorManager cursorManager,
        PhysicsRaycaster physicsRaycaster)
    {
        this.timeScaleManager = timeScaleManager;
        this.cursorManager = cursorManager;
        this.physicsRaycaster = physicsRaycaster;
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

    public void TogglePause()
    {
        isPaused = true;
        timeScaleManager.SetPauseState(true);
        cursorManager.ShowCursorRequest();
        SetPhysicsRaycasterEnabled(false);

        if (menuPanel != null)
            menuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        cursorManager.HideCursorRequest();
        timeScaleManager.SetPauseState(false);
        SetPhysicsRaycasterEnabled(true);

        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

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