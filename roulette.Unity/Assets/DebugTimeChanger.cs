#if UNITY_EDITOR
using UnityEngine;

public class DebugTimeChanger : MonoBehaviour
{
    [SerializeField][Range(0, 5)] private float timeScale = 1;

    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
}
#endif