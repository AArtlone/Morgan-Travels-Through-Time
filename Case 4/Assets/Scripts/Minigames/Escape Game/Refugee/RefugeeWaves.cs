using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class RefugeeWaves
{
    public string Name;
    public int RewardInPoints;
    [Header("Number of NPCs should never", order = 1)]
    [Header("be less than 6!!!", order = 2)]
    public List<GameObject> Wave = new List<GameObject>();
}
