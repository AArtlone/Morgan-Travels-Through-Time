using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    public string Name;
    public string Description;
    public bool CompletionStatus = false;
    public List<Objective> Objectives;

    public Case(string Name, string Description, List<Objective> Objectives)
    {
        this.Name = Name;
        this.Description = Description;
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
