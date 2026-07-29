using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class RouletteBallsManager : MonoBehaviour
{
    [SerializeField] private BallsLauncher ballLauncher;
    [SerializeField] private float sectorsRadius;
    [SerializeField] private Transform wheel;
    [SerializeField] private float stopThreshold = 5f;
    [SerializeField] private float stableTimeRequired = 1f;
    [SerializeField] private float maxDistance;
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private Vector3 upAxis = Vector3.up;

    private List<Ball> balls = new List<Ball>();

    private List<float> prevAngles = new List<float>();
    private List<float> stableTimes = new List<float>();
    private HashSet<string> invalidBalls = new HashSet<string>();
    private TableInfo tableInfo;

    private Dictionary<string, HitInfo> hitsInfo = new Dictionary<string, HitInfo>();

    public event Action<HitInfo[]> AllBallsStopped;

    public event Action SimulationStarted;

    private bool isSimulating;

    public event Action<string> BallInvalid;

    [Inject]
    public void Construct(TableInfo tableInfo)
    {
        this.tableInfo = tableInfo;
    }

    private void MarkInvalid(Ball ball)
    {
        if (!invalidBalls.Contains(ball.Hash))
        {
            Debug.LogWarning($"Ball {ball.Hash} marked INVALID");
            invalidBalls.Add(ball.Hash);
            BallInvalid?.Invoke(ball.Hash);
        }

        if (hitsInfo.ContainsKey(ball.Hash))
        {
            hitsInfo.Remove(ball.Hash);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (wheel != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(wheel.position, sectorsRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wheel.position, maxDistance);
        }
    }

    public void AddBall(Ball ball)
    {
        balls.Add(ball);
        prevAngles.Add(0);
        stableTimes.Add(0);
        isSimulating = true;
        SimulationStarted?.Invoke();
        
    }

    void Update()
    {
        if (isSimulating == false) return;
        for (int i = 0; i < balls.Count; i++)
        {
            CheckBallStopped(balls[i]);
        }
        invalidBalls.Clear();

        if (hitsInfo.Count == balls.Count)
        {
            foreach (Ball ball in balls)
            {
                ball.Stop();
            }
            AllBallsStopped?.Invoke(hitsInfo.Values.ToArray());
            isSimulating = false;

            foreach (var hit in hitsInfo.Values)
            {
                Debug.Log($"Ball {hit.BallHash} stopped at field with number {hit.FieldNumber}, color {hit.FieldColor}, dozen {hit.Dozen}");
            }
        }
    }

    private void CheckBallStopped(Ball ball)
    {
        if (Vector2.SqrMagnitude(ball.transform.position - wheel.position) > maxDistance * maxDistance)
        {
            MarkInvalid(ball);
            return;
        }

        Vector3 local = wheel.InverseTransformPoint(ball.transform.position);

        float deg = GetAngle(local);

        int ballIndex = balls.IndexOf(ball);
        float delta = Mathf.Abs(Mathf.DeltaAngle(prevAngles[ballIndex], deg));
        prevAngles[ballIndex] = deg;

        if (delta < stopThreshold)
            stableTimes[ballIndex] += Time.deltaTime;
        else
            stableTimes[ballIndex] = 0f;

        if (stableTimes[ballIndex] >= stableTimeRequired)
        {
            if (Vector2.SqrMagnitude(ball.transform.position - wheel.position) <= sectorsRadius * sectorsRadius)
            {
                int sectorIndex = Mathf.FloorToInt(deg / (360f / rouletteWheel.Fields.Length));

                var field = rouletteWheel.Fields[sectorIndex];
                if (hitsInfo.ContainsKey(ball.Hash) == false)
                {
                    int number = rouletteWheel.FieldsOrder[sectorIndex];
                    var bet = tableInfo.PocketMapping[number];
                    hitsInfo.Add(ball.Hash, new HitInfo()
                    {
                        BallHash = ball.Hash,
                        FieldColor = bet.FieldColor,
                        FieldNumber = bet.FieldNumber,
                        Dozen = bet.Dozen,
                        FieldIndex = number
                    });
                }
            }
            else
            {
                MarkInvalid(ball);
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

    public void ClearAll()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            Destroy(balls[i].gameObject);
        }
        hitsInfo.Clear();
        balls.Clear();
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

    public void AnimateBall(string ballHash)
    {
        foreach (var ball in balls)
        {
            if (ball.Hash == ballHash)
            {
                ball.PlayAnimation();
            }
        }
    }
}
