using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInteraction : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask mask;

    private IMouseInteractable hitObject;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask))
        {
            if (hit.collider.gameObject.TryGetComponent(out IMouseInteractable interactable))
            {
                if (hitObject != interactable)
                {
                    hitObject = interactable;
                    interactable.OnMouseEnter();
                }
                
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    interactable.OnMouseDown();
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    interactable.OnMouseUp();
                }
            }
            else
            {
                hitObject?.OnMouseExit();
                hitObject = null;
            }
        }
        else
        {
            hitObject?.OnMouseExit();
            hitObject = null;
        }
    }
}
