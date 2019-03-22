using System.Collections.Generic;
using System;

[Serializable]
public class RefugeeWaves
{
    public string Name;
    public int RewardInPoints;
    public List<Refugee> Wave = new List<Refugee>();
}
