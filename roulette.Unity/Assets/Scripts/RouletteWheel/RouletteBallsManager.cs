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
    [SerializeField] private float stableTimeRequired = 1f;
    [SerializeField] private float maxDistance;
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private Vector3 upAxis = Vector3.up;

    private List<Ball> balls = new List<Ball>();

    private List<int> prevSectors = new List<int>();
    private List<float> stableTimes = new List<float>();
    private HashSet<string> invalidBalls = new HashSet<string>();
    private TableInfo tableInfo;

    private Dictionary<string, HitInfo> hitsInfo = new Dictionary<string, HitInfo>();

    public event Action<HitInfo[]> AllBallsStopped;

    private bool isSpinning;

    public event Action<string> BallInvalid;

    public bool IsSpinning => isSpinning;

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
            // Отрисовка окружностей радиусов
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(wheel.position, sectorsRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wheel.position, maxDistance);

            // Отрисовка разделительных линий секторов
            if (rouletteWheel != null && rouletteWheel.Fields != null && rouletteWheel.Fields.Length > 0)
            {
                Gizmos.color = Color.yellow;
                int totalSectors = rouletteWheel.Fields.Length;
                float angleStep = 360f / totalSectors;

                // Вычисляем половину угла одного сектора
                float halfSectorOffset = angleStep / 2f;

                // Нормализуем ось вращения
                Vector3 up = upAxis.normalized;

                // Создаем два ортогональных вектора на плоскости колеса
                Vector3 refAxis = Vector3.Normalize(Vector3.Cross(up, Vector3.right) + Vector3.Cross(up, Vector3.forward));
                Vector3 ortho = Vector3.Cross(up, refAxis);

                for (int i = 0; i < totalSectors; i++)
                {
                    // Вычитаем половину сектора, чтобы повернуть линии по часовой стрелке
                    float currentAngleDeg = (i * angleStep) - halfSectorOffset;
                    float currentAngleRad = currentAngleDeg * Mathf.Deg2Rad;

                    // Направление линии на плоскости колеса
                    Vector3 dir = refAxis * Mathf.Cos(currentAngleRad) + ortho * Mathf.Sin(currentAngleRad);

                    // Поворачиваем линию вместе с колесом
                    Vector3 worldDir = wheel.TransformDirection(dir);

                    // Рисуем линию из центра колеса до максимального радиуса
                    Gizmos.DrawLine(wheel.position, wheel.position + worldDir * maxDistance);
                }
            }
        }
    }



    public void AddBall(Ball ball)
    {
        balls.Add(ball);
        prevSectors.Add(-1);
        stableTimes.Add(0);
        isSpinning = true;

    }

    void Update()
    {
        if (isSpinning == false) return;

        invalidBalls.Clear();

        for (int i = 0; i < balls.Count; i++)
        {
            CheckBallStopped(balls[i]);
        }

        if (hitsInfo.Count == balls.Count)
        {
            foreach (Ball ball in balls)
            {
                ball.Stop();
            }
            AllBallsStopped?.Invoke(hitsInfo.Values.ToArray());
            isSpinning = false;

            foreach (var hit in hitsInfo.Values)
            {
                Debug.Log($"Ball {hit.BallHash} stopped at field with number {hit.FieldNumber}, color {hit.FieldColor}, dozen {hit.Dozen}");
            }
        }
    }

    private void CheckBallStopped(Ball ball)
    {
        float sqrDistance = Vector3.SqrMagnitude(ball.transform.position - wheel.position);
        if (sqrDistance > maxDistance * maxDistance)
        {
            MarkInvalid(ball);
            return;
        }

        int ballIndex = balls.IndexOf(ball);

        if (sqrDistance <= sectorsRadius * sectorsRadius)
        {
            Vector3 local = wheel.InverseTransformPoint(ball.transform.position);
            float deg = GetAngle(local);

            int currentSectorIndex = Mathf.FloorToInt(deg / (360f / rouletteWheel.Fields.Length));

            if (currentSectorIndex == prevSectors[ballIndex])
            {
                stableTimes[ballIndex] += Time.deltaTime;
            }
            else
            {
                stableTimes[ballIndex] = 0f;
                prevSectors[ballIndex] = currentSectorIndex;

                if (hitsInfo.ContainsKey(ball.Hash))
                {
                    hitsInfo.Remove(ball.Hash);
                }
            }

            if (stableTimes[ballIndex] >= stableTimeRequired)
            {
                if (hitsInfo.ContainsKey(ball.Hash) == false)
                {
                    int number = rouletteWheel.FieldsOrder[currentSectorIndex];
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
                if (hitsInfo.ContainsKey(ball.Hash) == true)
                {
                    hitsInfo.Remove(ball.Hash);
                }
            }
        }
        else
        {
            stableTimes[ballIndex] = 0f;
            prevSectors[ballIndex] = -1;

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

        float halfSectorOffset = (360f / rouletteWheel.Fields.Length) / 2f;

        return (angle + halfSectorOffset + 360f) % 360f;
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
