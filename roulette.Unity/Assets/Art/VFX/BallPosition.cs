using UnityEngine;
using UnityEngine.VFX;

public class BallPosition : MonoBehaviour
{
    [SerializeField] private VisualEffect trailVFX;
    private const string POSITION_PROPERTY_NAME = "BallPosition";

    private void Awake()
    {
        if (trailVFX == null)
            trailVFX = GetComponent<VisualEffect>();
    }

    private void Update()
    {
        //if (trailVFX != null && trailVFX.isActiveAndEnabled && transform != null)
        //    trailVFX.SetVector3(POSITION_PROPERTY_NAME, transform.position);
    }
}