using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;

    private Transform rouletteOrigin;
    private float lastContactTime = float.NegativeInfinity;

    public event Action Destroyed;
    public event Action Animated;
    public event Action Stopped;
    public event Action<float> Collided; // сила удара (относительная скорость, м/с)

    public string Hash { get; private set; }

    public Vector3 RelativeToRoulettePosition => transform.position - rouletteOrigin.position;

    /// <summary>Шарик касается поверхности (катится, а не летит).</summary>
    public bool IsInContact => Time.time - lastContactTime < 0.1f;

    public void Launch(Vector3 velocity, Transform rouletteOrigin)
    {
        ballRigidbody.linearVelocity = velocity;
        this.rouletteOrigin = rouletteOrigin;
    }

    public void SetHash(string hash)
    {
        Hash = hash;
    }

    private void OnCollisionEnter(Collision collision)
    {
        lastContactTime = Time.time;
        Collided?.Invoke(collision.relativeVelocity.magnitude);
    }

    private void OnCollisionStay(Collision collision)
    {
        lastContactTime = Time.time;
    }

    public void OnDestroy()
    {
        Destroyed?.Invoke();
    }

    public void Freeze()
    {
        if (ballRigidbody.isKinematic)
            return;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.isKinematic = true;
        ballRigidbody.interpolation = RigidbodyInterpolation.None;
    }

    public void Stop()
    {
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.linearVelocity = Vector3.zero;
        Stopped?.Invoke();
    }

    public void PlayAnimation()
    {
        Animated?.Invoke();
    }
}