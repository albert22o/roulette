using System;
using UnityEngine;

[Serializable]
public class RoundConfig
{
    public long Goal;

    [SerializeField] private bool isBoss;
    public bool IsBoss => isBoss;
}