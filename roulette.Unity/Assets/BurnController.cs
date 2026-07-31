using System.Collections;
using UnityEngine;

public class BurnController : MonoBehaviour
{
    [SerializeField] private Material burnMaterial;
    [SerializeField] private float burnDuration;
    private Material originalMaterial;
    private Renderer rend;
    private float dissolve;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
    }

    private void Start()
    {
        Burn();
    }

    public void Burn()
    {
        rend.material = burnMaterial;
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        dissolve = 0f;
        while (dissolve < 1f)
        {
            dissolve += Time.deltaTime / burnDuration;
            rend.material.SetFloat("_Dissolve", dissolve);
            yield return null;
        }
        Destroy(gameObject);
    }
}
