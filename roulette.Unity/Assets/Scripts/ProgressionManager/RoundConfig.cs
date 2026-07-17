using System;
using UnityEngine;

[Serializable]
public class RoundConfig
{
    public int Goal;

    [SerializeField] private bool isBoss;
    public bool IsBoss => isBoss;
}