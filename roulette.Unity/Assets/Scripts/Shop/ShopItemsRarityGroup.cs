using System.Linq;
using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ShopItemsRarityGroup", menuName = "Scriptable Objects/ShopItemsRarityGroup")]
public class ShopItemsRarityGroup : ScriptableObject
{
    [SerializeField] private RarityType rarityType;

#if UNITY_EDITOR
    [SerializeField] private DefaultAsset folder;
#endif

    [SerializeField] private ItemData[] items;

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

        EditorUtility.SetDirty(this);
    }
#endif

    public ItemData GetRandomItem()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning($"ShopItemsRarityGroup ({name}) has no items.");
            return null;
        }
        int randomIndex = Random.Range(0, items.Length);
        return items[randomIndex];
    }

    public IReadOnlyCollection<ItemData> Items => items;
}
