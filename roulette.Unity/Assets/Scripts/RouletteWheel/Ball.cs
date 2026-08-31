using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;

    private Transform rouletteOrigin;

    public event Action Destroyed;
    public event Action Animated;
    public event Action Stopped;
    public string Hash { get; private set; }

    public Vector3 RelativeToRoulettePosition => transform.position - rouletteOrigin.position;


    public void Launch(Vector3 velocity, Transform rouletteOrigin)
    {
        ballRigidbody.linearVelocity = velocity;
        this.rouletteOrigin = rouletteOrigin;
    }

    public void SetHash(string hash)
    {
        Hash = hash;
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
    public void Bounce(float force)
    {
        if (ballRigidbody.isKinematic)
        {
            ballRigidbody.isKinematic = false;
        }

        ballRigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
    }
}
