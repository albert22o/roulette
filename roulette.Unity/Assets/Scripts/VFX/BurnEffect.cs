using UnityEngine;
using System.Collections;
using System;

public class BurnEffect : MonoBehaviour
{
    [SerializeField] private Material burnMaterial;
    [SerializeField] private float burnDuration = 1.5f;
    [SerializeField] private string dissolveProperty = "_Disolve";

    private MeshRenderer meshRenderer;
    private Material instanceMaterial;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>(true);
    }

    public void StartBurn(Action onComplete = null)
    {
        if (meshRenderer == null || burnMaterial == null)
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
            return;
        }

        instanceMaterial = new Material(burnMaterial);
        meshRenderer.material = instanceMaterial;

        if (!instanceMaterial.HasProperty(dissolveProperty))
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
            return;
        }

        instanceMaterial.SetFloat(dissolveProperty, 0f);
        StartCoroutine(BurnCoroutine(onComplete));
    }

    private IEnumerator BurnCoroutine(Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < burnDuration)
        {
            elapsed += Time.deltaTime;
            float dissolveValue = Mathf.Clamp01(elapsed / burnDuration);
            instanceMaterial.SetFloat(dissolveProperty, dissolveValue);
            yield return null;
        }

        instanceMaterial.SetFloat(dissolveProperty, 1f);
        gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}