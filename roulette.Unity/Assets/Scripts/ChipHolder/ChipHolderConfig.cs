using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChipHolderConfig", menuName = "Configs/ChipHolderConfig")]
public class ChipHolderConfig : ScriptableObject
{
    [field : SerializeField] public int Capacity { get; private set; }
    public List<ChipData> StartChips; 
}
