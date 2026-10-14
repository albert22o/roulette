using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsLauncher : MonoBehaviour
{
    public event Action<string> Hover;
    public event Action<string> UnHover;
    public event Action<string> Clicked;
    public event Action<int, float> RollIntensityChanged; // ballId, 0..1
    public event Action<int, float> BallCollided;         // ballId, 0..1

    [SerializeField] private float launchVelocityMin;
    [SerializeField] private float launchVelocityMax;

    [SerializeField] private Ball ballPrefab;
    [SerializeField] private RouletteBall ballVisualPrefab;
    [SerializeField] private RouletteBallsManager ballsManager;

    [SerializeField] private Transform physicalRoulette, visualRoulette;
    [SerializeField] private float delayBetweenBalls;

    private List<BallData> ballsToSpawn = new List<BallData>();
    private Coroutine ballLaunching;

    private void Start()
    {
        ballsManager.BallInvalid += OnBallInvalid;
    }

    private void OnDestroy()
    {
        if (ballsManager != null)
            ballsManager.BallInvalid -= OnBallInvalid;
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
        if (ballLaunching != null) StopCoroutine(ballLaunching);
        ballLaunching = StartCoroutine(BallLaunching());
    }

    private IEnumerator BallLaunching()
    {
        int index = 0;
        while (ballsToSpawn.Count > index)
        {
            var ballData = ballsToSpawn[index];
            SpawnBall(ballData);
            index++;
            yield return new WaitForSeconds(delayBetweenBalls);
        }
        ballLaunching = null;
    }

    private void SpawnBall(BallData ballData)
    {
        var ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        float velocity = UnityEngine.Random.Range(launchVelocityMin, launchVelocityMax);
        ball.SetHash(ballData.BallHash);
        ball.Launch(transform.right * velocity, physicalRoulette);
        ballsManager.AddBall(ball);

        var rouletteBall = Instantiate(ballVisualPrefab, transform.position, Quaternion.identity);
        rouletteBall.Initialize(ball, visualRoulette.lossyScale.x / physicalRoulette.lossyScale.x, visualRoulette);
        rouletteBall.SetVisualComponent(ballData.VisualComponent);

        rouletteBall.Clicked += OnBallClicked;
        rouletteBall.Hover += OnBallHover;
        rouletteBall.UnHover += OnBallUnHover;
        rouletteBall.RollIntensityChanged += OnBallRollIntensityChanged;
        rouletteBall.Collided += OnBallCollided;
        rouletteBall.Destroyed += () => OnBallDestroyed(rouletteBall);
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
            if (ballData != null)
                newBallsToSpawn.Add(ballData);
        }
        ballsToSpawn = newBallsToSpawn;
    }

    private void OnBallInvalid(string ballHash)
    {
        Debug.Log($"Rerolling invalid ball: {ballHash}");

        var data = ballsToSpawn.Find(b => b.BallHash == ballHash);
        if (data == null)
        {
            Debug.LogError("BallData not found for invalid ball!");
            return;
        }
        ballsManager.Clear(ballHash);
        SpawnBall(data);
    }

    private void OnBallUnHover(string hash) => UnHover?.Invoke(hash);

    private void OnBallHover(string hash) => Hover?.Invoke(hash);

    private void OnBallClicked(string hash) => Clicked?.Invoke(hash);

    private void OnBallRollIntensityChanged(int ballId, float value) => RollIntensityChanged?.Invoke(ballId, value);

    private void OnBallCollided(int ballId, float impact) => BallCollided?.Invoke(ballId, impact);

    private void OnBallDestroyed(RouletteBall ball)
    {
        ball.Clicked -= OnBallClicked;
        ball.Hover -= OnBallHover;
        ball.UnHover -= OnBallUnHover;
        ball.RollIntensityChanged -= OnBallRollIntensityChanged;
        ball.Collided -= OnBallCollided;
    }
}

internal class BallData
{
    public string BallHash;
    public VisualComponent VisualComponent;
}