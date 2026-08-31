using UnityEngine;
using Zenject;
using System.Collections;

public class GrayFieldLoseView : MonoBehaviour
{
    [Header("Эффект взрыва")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("Настройки")]
    [SerializeField] private float bounceForce = 5f;
    [SerializeField] private float timeBeforeExplosion = 0.4f;

    private BallResults ballResults;
    private RouletteBallsManager ballsManager;

    [Inject]
    public void Construct(BallResults ballResults, RouletteBallsManager ballsManager)
    {
        this.ballResults = ballResults;
        this.ballsManager = ballsManager;
    }

    private void OnEnable()
    {
        ballResults.GrayBallHit += OnGrayBallHit;
    }

    private void OnDisable()
    {
        ballResults.GrayBallHit -= OnGrayBallHit;
    }

    private void OnGrayBallHit(HitInfo hitInfo)
    {
        StartCoroutine(PlayGrayLoseEffect(hitInfo));
    }

    private IEnumerator PlayGrayLoseEffect(HitInfo hitInfo)
    {
        Ball grayBall = ballsManager.GetBallByHash(hitInfo.BallHash);
        if (grayBall == null)
        {
            Debug.LogWarning($"[GrayFieldLose] Физический шарик с хешом {hitInfo.BallHash} не найден");
            yield break;
        }

        RouletteBall visualBall = FindVisualBallByHash(hitInfo.BallHash);
        if (visualBall == null)
        {
            Debug.LogWarning($"[GrayFieldLose] Визуальный шарик с хешом {hitInfo.BallHash} не найден");
            yield break;
        }

        Vector3 explosionPosition = visualBall.transform.position;

        grayBall.Bounce(bounceForce);

        yield return new WaitForSeconds(timeBeforeExplosion);

        if (explosionPrefab != null)
        {
            var explosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
            Destroy(explosion, 2f);
        }
        else
        {
            Debug.LogWarning("[GrayFieldLose] Префаб взрыва не назначен!");
        }

        ballsManager.Clear(grayBall.Hash);
    }

    private RouletteBall FindVisualBallByHash(string hash)
    {
        var allVisualBalls = FindObjectsOfType<RouletteBall>();
        foreach (var visualBall in allVisualBalls)
        {
            if (visualBall != null && visualBall.GetBallHash() == hash)
            {
                return visualBall;
            }
        }
        return null;
    }
}