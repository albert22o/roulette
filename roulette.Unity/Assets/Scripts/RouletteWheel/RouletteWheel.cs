using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouletteWheel : MonoBehaviour, IRouletteWheel
{
    [SerializeField] private SpinController spinController;
    [SerializeField] private BallsLauncher ballsLauncher;
    [SerializeField] private Transform physicsModel;
    [SerializeField] private RouletteFieldsGenerator fieldsGenerator;
    [SerializeField] private RouletteBallsManager ballsManager;

    private HitInfo[] hitsInfo;

    public RouletteWheelField[] Fields { get; set; } = new RouletteWheelField[37];

    public event Action FieldsUpdated;

    public Transform InnerWheel => spinController.transform;
    public Vector2 PhysicsModelPosition => physicsModel.position;

    public event Action<HitInfo[]> AllBallsStopped;


    private void Awake()
    {
        for (int i = 0; i < 37; i++)
        {
            Fields[i] = new RouletteWheelField();
        }
        fieldsGenerator.GenerateFields(Fields);
    }

    private void OnEnable()
    {
        spinController.TargetSpeedReached += LaunchBalls;
        ballsManager.AllBallsStopped += OnAllBallsStopped;
    }

    private void OnDisable()
    {
        spinController.TargetSpeedReached -= LaunchBalls;
        ballsManager.AllBallsStopped -= OnAllBallsStopped;
    }

    private void OnAllBallsStopped(HitInfo[] hitsInfo)
    {
        this.hitsInfo = hitsInfo.ToArray();
        AllBallsStopped?.Invoke(hitsInfo);
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
    }

    public void SetFieldColor(int fieldPosition, string color)
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

}
