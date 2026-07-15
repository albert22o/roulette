using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemsRarityGroup", menuName = "Scriptable Objects/ShopItemsRarityGroup")]
public class ShopItemsRarityGroup : ScriptableObject
{
    [SerializeField] private RarityType rarityType;
    [SerializeField] private DefaultAsset folder;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (folder == null)
            return;

        string folderPath = AssetDatabase.GetAssetPath(folder);

        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { folderPath });

        items = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToArray();
    }
#endif

    private ItemData[] items;

    public ItemData GetRandomItem()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning("ShopItemsRarityGroup has no items.");
            return null;
        }
        int randomIndex = Random.Range(0, items.Length);
        var item = items[randomIndex];
        return item;
    }
}
