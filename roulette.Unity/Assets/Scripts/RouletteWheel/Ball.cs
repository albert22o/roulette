using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;

    private Transform rouletteOrigin;

    public event Action Destroyed;
    public event Action Animated;
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
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.isKinematic = true;
        ballRigidbody.interpolation = RigidbodyInterpolation.None;
    }

    public void PlayAnimation()
    {
        Animated?.Invoke();
    }
}
