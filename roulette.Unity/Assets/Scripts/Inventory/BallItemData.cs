using UnityEngine;

[CreateAssetMenu(fileName = "BallItemData", menuName = "Scriptable Objects/BallItemData")]
public class BallItemData : ItemData
{
    [field: SerializeField] public VisualComponent VisualComponent { get; private set; }
}
