using UnityEngine;
using Zenject;

public class ApplicationFocusHandler : MonoBehaviour
{
    private CursorManager cursorManager;
    [Inject]
    private void Construct(CursorManager cursorManager)
    {
        this.cursorManager = cursorManager;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            cursorManager.ApplyCursorState();
        }
    }
}
