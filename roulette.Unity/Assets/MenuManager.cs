using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel; 
    [SerializeField] private InputActionReference pauseAction;

    private bool isPaused;
    private TimeScaleManager timeScaleManager;

    private CursorManager cursorManager;
    [Inject]private void Construct(TimeScaleManager timeScaleManager, CursorManager cursorManager)
    {
        this.timeScaleManager = timeScaleManager;
        this.cursorManager = cursorManager;
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
    }

    private void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            TogglePause();
        }
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
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        cursorManager.HideCursorRequest();
        timeScaleManager.SetPauseState(false);
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }
}
