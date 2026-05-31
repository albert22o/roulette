using UnityEngine;

public class ChipScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem chipEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ParticleSystem ChipEffect()
    {
        return chipEffect;
    }
}
