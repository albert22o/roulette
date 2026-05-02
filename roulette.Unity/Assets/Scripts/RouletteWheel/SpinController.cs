using UnityEngine;
using System;

public class SpinController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [SerializeField] private float targetSpeed = 360f;

    [SerializeField] private float accelerationTime = 1f;

    [SerializeField] private float decelerationTime = 1f;

    [SerializeField] private float constantSpinTime = 2f;

    [SerializeField] private Rigidbody rb;

    public event Action TargetSpeedReached;

    private float currentSpeed = 0f;
    private bool isSpinning = false;
    private bool isTargetSpeedReached = false;

    private enum SpinState { Idle, Accelerating, Constant, Decelerating }
    private SpinState state = SpinState.Idle;

    private float constantTimer = 0f;

    private float accelerationRate;
    private float decelerationRate;

    private void Start()
    {
        accelerationRate = Mathf.Abs(targetSpeed) / Mathf.Max(accelerationTime, 0.0001f);
        decelerationRate = Mathf.Abs(targetSpeed) / Mathf.Max(decelerationTime, 0.0001f);
    }

    private void FixedUpdate()
    {
        if (!isSpinning) return;

        switch (state)
        {
            case SpinState.Accelerating:
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    targetSpeed,
                    accelerationRate * Time.fixedDeltaTime
                );

                if (Mathf.Approximately(currentSpeed, targetSpeed))
                {
                    state = SpinState.Constant;

                    if (!isTargetSpeedReached)
                    {
                        isTargetSpeedReached = true;
                        TargetSpeedReached?.Invoke();
                    }
                }
                break;

            case SpinState.Constant:
                constantTimer += Time.fixedDeltaTime;
                if (constantTimer >= constantSpinTime)
                {
                    state = SpinState.Decelerating;
                }
                break;

            case SpinState.Decelerating:
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    0f,
                    decelerationRate * Time.fixedDeltaTime
                );

                if (Mathf.Approximately(currentSpeed, 0f))
                {
                    currentSpeed = 0f;
                    isSpinning = false;
                    state = SpinState.Idle;
                }
                break;
        }

        Quaternion delta = Quaternion.AngleAxis(currentSpeed * Time.fixedDeltaTime, rotationAxis.normalized);
        rb.MoveRotation(rb.rotation * delta);
    }

    public void Spin()
    {
        isSpinning = true;
        state = SpinState.Accelerating;
        currentSpeed = 0f;
        constantTimer = 0f;
        isTargetSpeedReached = false;
    }
}