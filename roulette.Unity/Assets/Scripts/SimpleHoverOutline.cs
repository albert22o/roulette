using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(OutlineController))]
public class SimpleHoverOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private OutlineController outline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.ToggleMaterial(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.ToggleMaterial(false);
    }

    private void Start()
    {
        outline = GetComponent<OutlineController>();
    }
}
