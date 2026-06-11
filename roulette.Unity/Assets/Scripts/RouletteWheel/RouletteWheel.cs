using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class RouletteWheel : MonoBehaviour, IRouletteWheel
{
    [SerializeField] private SpinController spinController;
    [SerializeField] private BallsLauncher ballsLauncher;
    [SerializeField] private Transform physicsModel;
    [SerializeField] private RouletteFieldsGenerator fieldsGenerator;
    [SerializeField] private RouletteBallsManager ballsManager;

    private HitInfo[] hitsInfo;
    private TableConfig tableConfig;

    public RouletteWheelField[] Fields { get; set; } = new RouletteWheelField[37];

    public event Action FieldsUpdated;

    public Transform InnerWheel => spinController.transform;
    public Vector2 PhysicsModelPosition => physicsModel.position;

    public event Action<HitInfo[]> AllBallsStopped;

    public int[] FieldsOrder = new int[]
{
    0,
    32, 15, 19, 4, 21, 2, 25, 17, 34, 6,
    27, 13, 36, 11, 30, 8, 23, 10, 5, 24,
    16, 33, 1, 20, 14, 31, 9, 22, 18, 29,
    7, 28, 12, 35, 3, 26
};

    [Inject]
    public void Construct(TableConfig tableConfig)
    {
        this.tableConfig = tableConfig;
    }

    private void Awake()
    {
        for (int i = 0; i < 37; i++)
        {
            Fields[i] = new RouletteWheelField();
        }
        fieldsGenerator.GenerateFields(Fields);
        for (int i = 0; i < tableConfig.PocketMapping.Count; i++)
        {
            int tableIndex = FieldsOrder[i];

            Fields[i].Color = tableConfig.PocketMapping[tableIndex].FieldColor;
            Fields[i].Number = tableConfig.PocketMapping[tableIndex].FieldNumber;
        }
        FieldsUpdated?.Invoke();
    }

    private void OnEnable()
    {
        spinController.TargetSpeedReached += LaunchBalls;
        ballsManager.AllBallsStopped += OnAllBallsStopped;
        tableConfig.PocketColorChanged += OnPocketColorChanged;
        tableConfig.PocketNumberChanged += OnPocketNumberChanged;
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

    public void OnPocketColorChanged(int index, string color)
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
