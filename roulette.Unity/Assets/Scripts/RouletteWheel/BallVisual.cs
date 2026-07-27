using UnityEngine;

public class BallVisual : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;

    private VisualComponent visualComponent;

    public void SetVisualComponent(VisualComponent visualComponent)
    {
        if (visualComponent == this.visualComponent) return;
        this.visualComponent = visualComponent;
        meshRenderer.material = visualComponent.Material;
    }
}
