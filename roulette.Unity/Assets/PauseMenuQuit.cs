using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PauseMenuQuit : MonoBehaviour
{

    private TimeScaleManager timeScaleManager;

    [Inject]
    private void Construct(TimeScaleManager timeScaleManager)
    {
        this.timeScaleManager = timeScaleManager;
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
        timeScaleManager.ResetTimeScale();
    }
}
