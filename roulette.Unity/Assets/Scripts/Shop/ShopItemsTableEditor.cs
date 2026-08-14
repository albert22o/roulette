#if UNITY_EDITOR

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ShopItemsTable))]
public class ShopItemsTableEditor : Editor
{
    private SerializedProperty tablesProp;
    private ReorderableList tablesList;

    private void OnEnable()
    {
        tablesProp = serializedObject.FindProperty("itemsTables");

        tablesList = new ReorderableList(serializedObject, tablesProp, true, true, true, true);

        tablesList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Shop Loot Groups");
        };

        tablesList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 12;

        tablesList.drawElementCallback = (rect, index, active, focused) =>
        {
            var element = tablesProp.GetArrayElementAtIndex(index);

            var rarityGroupProp = element.FindPropertyRelative("RarityGroup");
            var rarityProp = element.FindPropertyRelative("Rarity");

            rect.y += 4;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                rarityGroupProp,
                new GUIContent("Rarity Group")
            );

            rect.y += EditorGUIUtility.singleLineHeight + 4;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                rarityProp,
                new GUIContent("Chance (%)")
            );
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        float sum = 0;
        for (int i = 0; i < tablesProp.arraySize; i++)
        {
            var element = tablesProp.GetArrayElementAtIndex(i);
            sum += element.FindPropertyRelative("Rarity").floatValue;
        }

        if (Mathf.Abs(sum - 100f) > 0.01f)
        {
            GUI.color = new Color(1f, 0.6f, 0.3f);
            EditorGUILayout.HelpBox(
                $"Total chance = {sum}. Must be exactly 100.",
                MessageType.Warning
            );
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = new Color(0.6f, 1f, 0.6f); 
            EditorGUILayout.HelpBox(
                "Chances sum is 100",
                MessageType.Info
            );
            GUI.color = Color.white;
        }

        tablesList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif