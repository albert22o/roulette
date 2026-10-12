using UnityEngine;

public class CursorManager
{
    private int cursorShowCount = 0;

    public void ShowCursorRequest()
    {
        cursorShowCount++;
        ApplyCursorState();
    }

    public void HideCursorRequest()
    {
        cursorShowCount--;
        ApplyCursorState();
    }

    public void ApplyCursorState()
    {
        if (cursorShowCount > 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (cursorShowCount == 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.LogError("Cursor lock count is negative. This should not happen.");
        }
    }
}
