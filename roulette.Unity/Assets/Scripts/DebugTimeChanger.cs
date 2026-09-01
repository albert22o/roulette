#if UNITY_EDITOR
using UnityEngine;

public class DebugTimeChanger : MonoBehaviour
{
    [SerializeField][Range(0, 10)] private float timeScale = 1;

    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
}
#endif