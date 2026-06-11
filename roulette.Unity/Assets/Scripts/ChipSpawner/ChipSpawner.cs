using System.Collections.Generic;
using UnityEngine;

public class ChipSpawner
{
    private List<GameObject> spawnerChips = new();

    public GameObject SpawnChip(GameObject chipPrefab, Transform spawnPoint)
    {
        GameObject chip = UnityEngine.Object.Instantiate(chipPrefab, spawnPoint.position, Quaternion.identity);
        spawnerChips.Add(chip);
        return chip;
    }

    public void ClearChips()
    {
        foreach (var chip in spawnerChips)
        {
            if (chip != null)
                UnityEngine.Object.Destroy(chip);
        }

        spawnerChips.Clear();
    }
}