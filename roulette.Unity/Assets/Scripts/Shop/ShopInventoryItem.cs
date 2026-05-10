using UnityEngine;

public class ShopInventoryItem : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;
    public Item Item { get; private set;  }


    public void SetVisualComponent(VisualComponent visualComponent)
    {
        meshRenderer.material = visualComponent.Material; 
    }

    public void SetItem(Item item)
    {
        Item = item;
    }
}
