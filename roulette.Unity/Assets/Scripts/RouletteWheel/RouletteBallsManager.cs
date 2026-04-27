using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouletteBallsManager : MonoBehaviour
{
    [SerializeField] private BallsLauncher ballLauncher;
    [SerializeField] private float sectorsRadius;
    [SerializeField] private Transform wheel;
    [SerializeField] private float stopThreshold = 5f;
    [SerializeField] private float stableTimeRequired = 1f;
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private Vector3 upAxis = Vector3.up;

    private List<Ball> balls = new List<Ball>();

    private List<float> prevAngles = new List<float>();
    private List<float> stableTimes = new List<float>();

    private Dictionary<string, HitInfo> hitsInfo = new Dictionary<string, HitInfo>();
    public event Action<HitInfo[]> AllBallsStopped;

    private bool isSimulating;

    private void OnDrawGizmosSelected()
    {
        if (wheel != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wheel.position, sectorsRadius);
        }
    }

    public void AddBall(Ball ball)
    {
        balls.Add(ball);
        prevAngles.Add(0);
        stableTimes.Add(0);
        isSimulating = true;
    }


    void Update()
    {
        if (isSimulating == false) return;
        foreach (Ball ball in balls)
        {
            CheckBallStopped(ball);
        }
        if (hitsInfo.Count == balls.Count)
        {
            AllBallsStopped?.Invoke(hitsInfo.Values.ToArray());
            isSimulating = false;
        }
    }

    private void CheckBallStopped(Ball ball)
    {
        Vector3 local = wheel.InverseTransformPoint(ball.transform.position);

        float deg = GetAngle(local);

        int ballIndex = balls.IndexOf(ball);
        float delta = Mathf.Abs(Mathf.DeltaAngle(prevAngles[ballIndex], deg));
        prevAngles[ballIndex] = deg;

        if (delta < stopThreshold)
            stableTimes[ballIndex] += Time.deltaTime;
        else
            stableTimes[ballIndex] = 0f;

        if (stableTimes[ballIndex] >= stableTimeRequired
            && Vector2.SqrMagnitude(ball.transform.position - wheel.position) <= sectorsRadius * sectorsRadius)
        {
            int sectorIndex = Mathf.FloorToInt(deg / (360f / rouletteWheel.Fields.Length));

            var field = rouletteWheel.Fields[sectorIndex];
            if (hitsInfo.ContainsKey(ball.Hash) == false)
            {
                hitsInfo.Add(ball.Hash, new HitInfo()
                {
                    BallHash = ball.Hash,
                    FieldColor = field.Color,
                    FieldNumber = field.Number,
                    Dozen = GetDozenByIndex(sectorIndex)
                });
            }
        }
        else
        {
            if (hitsInfo.ContainsKey(ball.Hash) == true)
            {
                hitsInfo.Remove(ball.Hash);
            }
        }
    }

    private DozenType GetDozenByIndex(int index)
    {
        if (index == 0) return DozenType.Zero;
        else if (index <= 13) return DozenType.First;
        else if (index <= 25) return DozenType.Second;
        else return DozenType.Third;
    }

    private float GetAngle(Vector3 local)
    {
        Vector3 up = upAxis.normalized;

        Vector3 refAxis = Vector3.Normalize(Vector3.Cross(up, Vector3.right) + Vector3.Cross(up, Vector3.forward));
        Vector3 ortho = Vector3.Cross(up, refAxis);

        float x = Vector3.Dot(local, refAxis);
        float y = Vector3.Dot(local, ortho);

        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        return (angle + 360f) % 360f;
    }

    public void Clear()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            hitsInfo.Remove(balls[i].Hash);
            Destroy(balls[i].gameObject);
        }
    }

    public void Clear(params string[] ballHash)
    {
        if (ballHash == null || ballHash.Length == 0) return;

        balls.RemoveAll(ball =>
        {
            if (ballHash.Contains(ball.Hash))
            {
                if (ball != null)
                {
                    Destroy(ball.gameObject);
                }
                return true;
            }
            return false;
        });
    }

    public void FreezeBallsExcept(params string[] ballHash)
    {
        if (balls.Count == 0) return;

        foreach (var ball in balls)
        {
            if (ballHash.Contains(ball.Hash) == false)
            {
                ball.Freeze();
                ball.transform.parent = wheel.transform;
            }
        }
    }
}
