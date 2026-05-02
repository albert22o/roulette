using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsLauncher : MonoBehaviour
{
    [SerializeField] private float launchVelocityMin;
    [SerializeField] private float launchVelocityMax;

    [SerializeField] private Ball ballPrefab;
    [SerializeField] private BallVisual ballVisualPrefab;
    [SerializeField] private RouletteBallsManager ballsManager;

    [SerializeField] private Transform physicalRoulette, visualRoulette;
    [SerializeField] private float delayBetweenBalls;

    private List<BallData> ballsToSpawn = new List<BallData>();
    private Coroutine ballLaunching;

    private void Start()
    {
        ballsManager.BallInvalid += OnBallInvalid;
    }


    public void AddBall(string ballHash, VisualComponent visualComponent)
    {
        ballsToSpawn.Add(new BallData()
        {
            BallHash = ballHash,
            VisualComponent = visualComponent
        });
    }
    public void StartLaunch()
    {
        ballLaunching = StartCoroutine(BallLaunching(ballsToSpawn));
    }

    private IEnumerator BallLaunching(List<BallData> balls)
    {
        int index = 0;
        while(ballsToSpawn.Count > index)
        {
            var ballData = ballsToSpawn[index];
            SpawnBall(ballData);
            index++;
            yield return new WaitForSeconds(delayBetweenBalls);
        }
    }

    private void SpawnBall(BallData ballData)
    {
        var ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        float velocity = Random.Range(launchVelocityMin, launchVelocityMax);
        ball.SetHash(ballData.BallHash);
        ball.Launch(transform.right * velocity);
        ballsManager.AddBall(ball);

        var ballVisual = Instantiate(ballVisualPrefab, transform.position, Quaternion.identity);
        {
            ballVisual.Initialize(ball, visualRoulette.lossyScale.x / physicalRoulette.lossyScale.x);
            ballVisual.SetVisualComponent(ballData.VisualComponent);
        }
    }

    public void Clear()
    {
        ballsToSpawn.Clear();
    }

    public void Reroll(params string[] ballNames)
    {
        List<BallData> newBallsToSpawn = new List<BallData>();
        foreach (var ballName in ballNames)
        {
            var ballData = ballsToSpawn.Find(ball => ball.BallHash == ballName);
            newBallsToSpawn.Add(ballData);
        }
        ballsToSpawn = newBallsToSpawn;
    }

    private void OnBallInvalid(string ballHash)
    {
        Debug.Log($"Rerolling invalid ball: {ballHash}");

        var data = ballsToSpawn.Find(b => b.BallHash == ballHash);
        if (data.BallHash == null)
        {
            Debug.LogError("BallData not found for invalid ball!");
            return;
        }
        ballsManager.Clear(ballHash);
        SpawnBall(data);
    }

}

internal class BallData
{
    public string BallHash;
    public VisualComponent VisualComponent;
}
