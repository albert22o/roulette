using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundsConfig", menuName = "Configs/RoundsConfig")]
public class RoundsConfig : ScriptableObject
{
    public List<RoundConfig> Rounds = new();
}