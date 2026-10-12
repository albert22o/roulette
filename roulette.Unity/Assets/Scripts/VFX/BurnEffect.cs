using UnityEngine;
using System.Collections;
using System;

public class BurnEffect : MonoBehaviour
{
    [SerializeField] private Material burnMaterial;
    [SerializeField] private float burnDuration = 1.5f;
    [SerializeField] private string dissolveProperty = "_Disolve";

    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private Material instanceMaterial;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>(true);
    }

    /// <summary>Задаёт настройки при динамическом добавлении компонента.</summary>
    public void Configure(Material material, float duration, string property)
    {
        burnMaterial = material;
        burnDuration = duration;
        dissolveProperty = property;
    }

    public void StartBurn(Action onComplete = null)
    {
        if (meshRenderer == null || burnMaterial == null)
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
            return;
        }

        originalMaterial = meshRenderer.sharedMaterial;
        instanceMaterial = new Material(burnMaterial);

        if (!instanceMaterial.HasProperty(dissolveProperty))
        {
            Destroy(instanceMaterial);
            instanceMaterial = null;
            gameObject.SetActive(false);
            onComplete?.Invoke();
            return;
        }

        meshRenderer.material = instanceMaterial;
        instanceMaterial.SetFloat(dissolveProperty, 0f);
        StartCoroutine(BurnCoroutine(onComplete));
    }

    private IEnumerator BurnCoroutine(Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < burnDuration)
        {
            elapsed += Time.deltaTime;
            instanceMaterial.SetFloat(dissolveProperty, Mathf.Clamp01(elapsed / burnDuration));
            yield return null;
        }

        instanceMaterial.SetFloat(dissolveProperty, 1f);
        RestoreMaterial();
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    private void RestoreMaterial()
    {
        if (meshRenderer != null && originalMaterial != null)
            meshRenderer.sharedMaterial = originalMaterial;

        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
            instanceMaterial = null;
        }
    }

    private void OnDestroy()
    {
        if (instanceMaterial != null)
            Destroy(instanceMaterial);
    }
}