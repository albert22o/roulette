using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeadlyRoulette
{
    public class MainMenuUI : MonoBehaviour
    {
        public string gameSceneName = "SampleScene";

        public void StartGame()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
