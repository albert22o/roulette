using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BottomSectionZone : MonoBehaviour
{
    public string Color { get{ return color; } }

    public DozenType Dozen { get { return dozenType; } }

    [SerializeField] private BottomSectionType sectionType;
    [SerializeField] private DozenType dozenType;
    [SerializeField] private string color;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material colorHighlight;
    [SerializeField] private Material dozenHighlight;
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
            if (sectionType == BottomSectionType.Color)
                BG.material = colorHighlight;
            else
                BG.material = dozenHighlight;
        }
        else
            BG.material = baseMaterial;
    }

    public enum BottomSectionType { Color, Dozen}
}
