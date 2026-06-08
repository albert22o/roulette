using System;
using UnityEngine;
[Serializable]
public class BallInfo
{
    public string Name;
    public object Effect;

    public BallInfo(string name, object effect = null)
    {
        Name = name;
        Effect = effect;
    }
    
}
