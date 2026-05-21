using UnityEngine;

public class ItemVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }
}
