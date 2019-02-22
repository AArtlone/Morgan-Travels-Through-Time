using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case
{
    public string Name;
    public string Area;
    public string ProgressStatus;
    public string Description;
    public bool CompletionStatus;
    public List<Objective> Objectives;

    public Case(string Name, string Area, string ProgressStatus, string Description, bool CompletionStatus, List<Objective> Objectives)
    {
        this.Name = Name;
        this.Area = Area;
        this.ProgressStatus = ProgressStatus;
        this.Description = Description;
        this.CompletionStatus = CompletionStatus;
        this.Objectives = Objectives;

        /*
        Debug.Log("Case Name: " + Name);
        Debug.Log("Case Description: " + Description);

        foreach (Objective objective in Objectives)
        {
            Debug.Log("Objective: " + objective.Name);
        }
        */
    }
}
