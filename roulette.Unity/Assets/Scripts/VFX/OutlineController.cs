using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;

    [SerializeField] private bool autoFindRenderers = true;

    [SerializeField] private List<Renderer> _renderers = new List<Renderer>();
    [SerializeField] private bool findRenderersEveryToggle = false;
    private bool isMaterialActive = false;


    private void Awake()
    {
        if (autoFindRenderers) FindRenderers();
    }

    private void FindRenderers()
    {
        _renderers.Clear();
        _renderers.AddRange(GetComponentsInChildren<Renderer>(true));
    }

    public void ToggleMaterial(bool state)
    {
        if (targetMaterial == null) return;
        if (isMaterialActive == state) return;
        if (state && findRenderersEveryToggle) FindRenderers();

        for (int i = 0; i < _renderers.Count; i++)
        {
            Renderer rent = _renderers[i];
            if (rent == null) continue;

            List<Material> currentMaterials = new List<Material>(rent.sharedMaterials);
            bool hasMaterial = currentMaterials.Contains(targetMaterial);

            if (state && !hasMaterial)
            {
                currentMaterials.Add(targetMaterial);
                rent.sharedMaterials = currentMaterials.ToArray();
            }
            else if (!state && hasMaterial)
            {
                currentMaterials.Remove(targetMaterial);
                rent.sharedMaterials = currentMaterials.ToArray();
            }
        }

        isMaterialActive = state;
    }
}
