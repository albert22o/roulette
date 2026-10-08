using UnityEngine;
using UnityEngine.InputSystem;

public class CheatMenuToggle : MonoBehaviour
{
    [SerializeField] private GameObject cheatMenu;

    [SerializeField] private bool startHidden = true;

    [SerializeField]
    private InputAction toggleAction =
        new("ToggleCheatMenu", InputActionType.Button, "<Keyboard>/backquote");

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void Awake()
    {
        if (cheatMenu != null && startHidden)
            cheatMenu.SetActive(false);
    }

    private void OnEnable()
    {
        toggleAction.performed += OnTogglePerformed;
        toggleAction.Enable();
    }

    private void OnDisable()
    {
        toggleAction.performed -= OnTogglePerformed;
        toggleAction.Disable();
    }

    private void OnTogglePerformed(InputAction.CallbackContext context)
    {
        Toggle();
    }

    public void Toggle()
    {
        if (cheatMenu == null)
            return;

        cheatMenu.SetActive(!cheatMenu.activeSelf);
    }
#else
    private void Awake()
    {
        if (cheatMenu != null)
            cheatMenu.SetActive(false);
 
        enabled = false;
    }
#endif
}