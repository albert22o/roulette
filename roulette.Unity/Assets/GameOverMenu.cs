using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameOverMenu : MonoBehaviour
{
    private TimeScaleManager timeScaleManager;
    private SFXManager SFXManager;

    [Inject] private void Construct(TimeScaleManager timeScaleManager, SFXManager SFXManager)
    {
        this.timeScaleManager = timeScaleManager;
        this.SFXManager = SFXManager;
    }
    public void Restart()
    {
        SFXManager.Play("ButtonClick");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        timeScaleManager.ResetTimeScale();
    }

    public void Quit()
    {
        SFXManager.Play("ButtonClick");
        Application.Quit();
    }
}
