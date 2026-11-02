using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class RouletteWheel : MonoBehaviour, IRouletteWheel, IAnimateBalls
{
    public event Action<string> BallHover;
    public event Action<string> BallUnHover;
    public event Action<string> BallClicked;
    public event Action<int, float> BallRollIntensityChanged; // ballId, 0..1
    public event Action<int, float> BallCollided;             // ballId, 0..1
    public event Action FieldsUpdated;
    public bool IsCurrentlyHovered => !string.IsNullOrEmpty(CurrentHoveredBallHash);
    public string CurrentHoveredBallHash { get; private set; }

    [SerializeField] private SpinController spinController;
    [SerializeField] private BallsLauncher ballsLauncher;
    [SerializeField] private Transform physicsModel;
    [SerializeField] private RouletteFieldsGenerator fieldsGenerator;
    [SerializeField] private RouletteBallsManager ballsManager;
    [SerializeField] private float rerollDelay = 0.5f;

    private HitInfo[] hitsInfo;
    private TableInfo tableInfo;
    private List<string> ballsToAutoReroll = new();
    private Coroutine rerollCoroutine;

    public RouletteWheelField[] Fields { get; set; } = new RouletteWheelField[37];
    public FieldHighlightState[] HighlightStates { get; private set; } = new FieldHighlightState[37];

    public Transform InnerWheel => spinController.transform;
    public Vector2 PhysicsModelPosition => physicsModel.position; // Vector3 -> Vector2 (x, y), как и раньше

    public event Action<HitInfo[]> AllBallsStopped;
    public event Action Rerolled;

    public int[] FieldsOrder = new int[]
    {
        0,
        32, 15, 19, 4, 21, 2, 25, 17, 34, 6,
        27, 13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18, 29,
        7, 28, 12, 35, 3, 26
    };

    [Inject]
    public void Construct(TableInfo tableInfo)
    {
        this.tableInfo = tableInfo;
    }

    private void Awake()
    {
        for (int i = 0; i < 37; i++)
        {
            Fields[i] = new RouletteWheelField();
            HighlightStates[i] = new FieldHighlightState();
        }
        fieldsGenerator.GenerateFields(Fields, HighlightStates);

        for (int i = 0; i < tableInfo.PocketMapping.Count; i++)
        {
            int tableIndex = FieldsOrder[i];

            Fields[i].Color = tableInfo.PocketMapping[tableIndex].FieldColor;
            Fields[i].Number = tableInfo.PocketMapping[tableIndex].FieldNumber;
            Fields[i].Dozen = tableInfo.PocketMapping[tableIndex].Dozen;
        }
        FieldsUpdated?.Invoke();
    }

    private void OnEnable()
    {
        spinController.TargetSpeedReached += LaunchBalls;
        ballsManager.AllBallsStopped += OnAllBallsStopped;
        tableInfo.PocketColorChanged += OnPocketColorChanged;
        tableInfo.PocketNumberChanged += OnPocketNumberChanged;

        ballsLauncher.Clicked += OnBallClicked;
        ballsLauncher.Hover += OnBallHover;
        ballsLauncher.UnHover += OnBallUnHover;
        ballsLauncher.RollIntensityChanged += OnBallRollIntensityChanged;
        ballsLauncher.BallCollided += OnBallCollided;
    }

    private void OnDisable()
    {
        spinController.TargetSpeedReached -= LaunchBalls;
        ballsManager.AllBallsStopped -= OnAllBallsStopped;
        tableInfo.PocketColorChanged -= OnPocketColorChanged;
        tableInfo.PocketNumberChanged -= OnPocketNumberChanged;

        ballsLauncher.Clicked -= OnBallClicked;
        ballsLauncher.Hover -= OnBallHover;
        ballsLauncher.UnHover -= OnBallUnHover;
        ballsLauncher.RollIntensityChanged -= OnBallRollIntensityChanged;
        ballsLauncher.BallCollided -= OnBallCollided;
    }

    private void OnBallUnHover(string hash)
    {
        if (CurrentHoveredBallHash == hash)
        {
            CurrentHoveredBallHash = null;
        }
        BallUnHover?.Invoke(hash);
    }

    private void OnBallHover(string hash)
    {
        CurrentHoveredBallHash = hash;
        if (ballsManager.IsSpinning == false)
            BallHover?.Invoke(hash);
    }

    private void OnBallClicked(string hash)
    {
        if (ballsManager.IsSpinning == false)
            BallClicked?.Invoke(hash);
    }

    private void OnBallRollIntensityChanged(int ballId, float value)
    {
        BallRollIntensityChanged?.Invoke(ballId, value);
    }

    private void OnBallCollided(int ballId, float impact)
    {
        BallCollided?.Invoke(ballId, impact);
    }

    private void OnAllBallsStopped(HitInfo[] hitsInfo)
    {
        this.hitsInfo = hitsInfo.ToArray();
        AllBallsStopped?.Invoke(hitsInfo);

        if (ballsToAutoReroll.Count > 0)
        {
            if (rerollCoroutine != null) StopCoroutine(rerollCoroutine);
            rerollCoroutine = StartCoroutine(RerollWithDelayRoutine(ballsToAutoReroll.ToArray()));
            ballsToAutoReroll.Clear();
        }
        else
        {
            if (IsCurrentlyHovered)
            {
                BallHover?.Invoke(CurrentHoveredBallHash);
            }
        }
    }

    private IEnumerator RerollWithDelayRoutine(string[] ballNames)
    {
        yield return new WaitForSeconds(rerollDelay);
        Reroll(ballNames);
        rerollCoroutine = null;
    }

    public void OnPocketColorChanged(int index, ColorType color)
    {
        int pos = Array.IndexOf(FieldsOrder, index);
        Fields[pos].Color = color;
        FieldsUpdated?.Invoke();
    }

    public void OnPocketNumberChanged(int index, int number)
    {
        int pos = Array.IndexOf(FieldsOrder, index);
        Fields[pos].Number = number;
        FieldsUpdated?.Invoke();
    }

    public void AddBall(string ballHash, VisualComponent visualComponent)
    {
        ballsLauncher.AddBall(ballHash, visualComponent);
    }

    public void ClearBalls()
    {
        ballsManager.ClearAll();
        ballsLauncher.Clear();
    }

    public HitInfo[] GetHitInfo()
    {
        return hitsInfo;
    }

    public void Reroll(params string[] ballNames)
    {
        ballsManager.FreezeBallsExcept(ballNames);
        ballsManager.Clear(ballNames);
        ballsLauncher.Reroll(ballNames);
        spinController.Spin();
        Rerolled?.Invoke();
    }

    public void SetFieldColor(int fieldPosition, ColorType color)
    {
        Fields[fieldPosition].Color = color;
        FieldsUpdated?.Invoke();
    }

    public void SetFieldNumber(int fieldPosition, int number)
    {
        Fields[fieldPosition].Number = number;
        FieldsUpdated?.Invoke();
    }

    public void Spin()
    {
        spinController.Spin();
    }

    public void LaunchBalls()
    {
        ballsLauncher.StartLaunch();
    }

    public void AnimateBall(string ballHash)
    {
        ballsManager.AnimateBall(ballHash);
    }

    public void AddBallsToAutoReroll(params string[] ballNames)
    {
        foreach (string ballName in ballNames)
        {
            if (!ballsToAutoReroll.Contains(ballName))
            {
                ballsToAutoReroll.Add(ballName);
            }
        }
    }
}

