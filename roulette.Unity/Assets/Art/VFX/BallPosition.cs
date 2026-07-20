using UnityEngine;
using UnityEngine.VFX;

public class BallTrailBinder : MonoBehaviour
{
    [SerializeField] private VisualEffect trailVFX;
    [SerializeField] private Transform ball;

    private void Update()
    {
        trailVFX.SetVector3("BallPosition", ball.position);
    }
}