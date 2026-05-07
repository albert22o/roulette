using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [field: SerializeField] public string NameKey { get; private set; }
    [field: SerializeField] public string DescriptionKey { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    
}
