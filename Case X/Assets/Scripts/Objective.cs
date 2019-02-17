using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public string Name;
    public bool CompletedStatus;

    public Objective(string Name, bool CompletedStatus)
    {
        this.Name = Name;
        this.CompletedStatus = CompletedStatus;
    }
}
