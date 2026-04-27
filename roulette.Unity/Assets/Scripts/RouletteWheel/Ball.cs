using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;

    public event Action Destroyed;
    public string Hash { get; private set; }

    public void Launch(Vector3 velocity)
    {
        rigidbody.linearVelocity = velocity;
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
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.None;
    }
}
