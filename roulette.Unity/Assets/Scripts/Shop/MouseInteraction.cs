using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInteraction : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask mask;

    private GameObject hitObject;
    private GameObject clickedObject;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask))
        {
            if (hit.collider.gameObject.TryGetComponent(out IMouseInteractable _))
            {
                if (hitObject != hit.collider.gameObject)
                {
                    HandleNoIntersection();
                    hitObject = hit.collider.gameObject;

                    if (hitObject.TryGetComponent(out IHoverable hoverable))
                    {
                        hoverable.HoverEnter();
                    }
                }
                
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (hitObject.TryGetComponent(out IPressable pressable))
                    {
                        pressable.Pressed();
                    }
                    clickedObject = hitObject;
                }

                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    if (hitObject.TryGetComponent(out IPressable pressable))
                    {
                        pressable.Released();
                    }
                    if (hitObject.TryGetComponent(out IClickable clickable)
                        && clickedObject == hitObject)
                    {
                        clickable.Clicked();
                    }
                }
            }
            else
            {
                HandleNoIntersection();
            }
        }
        else
        {
            HandleNoIntersection();
        }
    }

    private void HandleNoIntersection()
    {
        if (hitObject != null)
        {
            if(hitObject.TryGetComponent(out IPressable pressable))
            {
                pressable.Released();
            }
            if (hitObject.TryGetComponent(out IHoverable hoverable))
            {
                hoverable.HoverExit();
            }
        }
        hitObject = null;
        clickedObject = null;
    }
}
