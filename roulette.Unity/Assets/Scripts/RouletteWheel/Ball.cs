using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;

    public event Action Destroyed;
    public string Hash { get; private set; }

    public void Launch(Vector3 velocity)
    {
        ballRigidbody.linearVelocity = velocity;
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
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.isKinematic = true;
        ballRigidbody.interpolation = RigidbodyInterpolation.None;
    }
}
