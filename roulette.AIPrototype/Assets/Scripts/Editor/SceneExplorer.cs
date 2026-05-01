using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneExplorer
{
    [MenuItem("Tools/Print Scene Hierarchy")]
    static void PrintHierarchy()
    {
        var scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();
        string r = "Scene: " + scene.name + "\n";
        foreach (var go in roots)
        {
            r += go.name + " (" + go.GetInstanceID() + ")\n";
            PrintChildren(go.transform, 1, ref r);
        }
        Debug.Log(r);
    }

    static void PrintChildren(Transform t, int depth, ref string r)
    {
        foreach (Transform child in t)
        {
            r += new string(' ', depth * 2) + child.name + " (" + child.gameObject.GetInstanceID() + ")\n";
            PrintChildren(child, depth + 1, ref r);
        }
    }
}
