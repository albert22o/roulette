using UnityEngine;
using System.Collections;
using System;

public class BurnEffect : MonoBehaviour
{
    [Header("Настройки сгорания")]
    [SerializeField] private Material burnMaterial;
    [SerializeField] private float burnDuration = 1.5f;
    [SerializeField] private string dissolveProperty = "_Dissolve"; // Имя переменной в шейдере

    private MeshRenderer meshRenderer;
    private Material instanceMaterial;

    private void Awake()
    {
        // Ищем MeshRenderer в детях
        meshRenderer = GetComponentInChildren<MeshRenderer>(true);
    }

    public void StartBurn(Action onComplete = null)
    {
        if (meshRenderer == null || burnMaterial == null)
        {
            Debug.LogWarning($"[BurnEffect] Ошибка настроек на {gameObject.name}.");
            gameObject.SetActive(false);
            onComplete?.Invoke();
            return;
        }

        // Создаем копию материала
        instanceMaterial = new Material(burnMaterial);
        meshRenderer.material = instanceMaterial;

        // ПРОВЕРКА: Есть ли такая переменная в шейдере?
        if (!instanceMaterial.HasProperty(dissolveProperty))
        {
            Debug.LogError($"[BurnEffect] ❌ В шейдере НЕТ переменной '{dissolveProperty}'! " +
                           $"Проверь точное имя в инспекторе материала (например, _DissolveAmount, _Clip, _Fade).");
            // Не выключаем объект, чтобы ты могла увидеть ошибку
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