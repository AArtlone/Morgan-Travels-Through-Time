using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    public enum AreaStatus { Locked, Unlocked }
    public string Name;
    public AreaStatus Status;

    public Area(string Name, AreaStatus Status)
    {
        this.Name = Name;
        this.Status = Status;
    }
}
