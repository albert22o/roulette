using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BottomSectionZone : MonoBehaviour
{
    public string Color => color;
    public DozenType Dozen => dozenType;

    [SerializeField] private DozenType dozenType;
    [SerializeField] private string color;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material highlightMaterial;
    private SpriteRenderer BG;

    private void Start()
    {
        BG = GetComponent<SpriteRenderer>();
        BG.material = baseMaterial;
    }

    public void Highlight(bool isEnabled)
    {
        if (isEnabled)
        {
            BG.material = highlightMaterial;
        }
        else
            BG.material = baseMaterial;
    }
}
