using System;
using UnityEngine;

public abstract class BallInfo
{
    public string BallHash { get; }
    public BallInfoData BallInfoData { get; }

    public BallInfo(BallInfoData ballInfoData)
    {
        BallHash = Guid.NewGuid().ToString();
        BallInfoData = ballInfoData;
    }

    public abstract void OnLanding();
}

public class SimpleBallInfo : BallInfo
{
    public SimpleBallInfo(BallInfoData ballInfoData) : base(ballInfoData)
    {
    }

    public override void OnLanding()
    {
        
    }
}
