using UnityEngine;

[CreateAssetMenu(fileName = "BallShopItemData", menuName = "Scriptable Objects/BallShopItemData")]
public class BallShopItemData : ItemData
{
    [SerializeField] private VisualComponent visualComponent;
    public VisualComponent VisualComponent => visualComponent;

}
