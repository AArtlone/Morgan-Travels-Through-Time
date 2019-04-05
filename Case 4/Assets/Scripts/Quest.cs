using System.Collections.Generic;
using System;

[Serializable]
public class Quest
{
    public int ID;
    public string Name;
    public string Area;
    public string ProgressStatus;
    public string Description;
    public bool CompletionStatus;
    public List<Objective> Objectives;

    public Quest(int ID, string Name, string Area, string ProgressStatus, string Description, bool CompletionStatus, List<Objective> Objectives)
    {
        this.ID = ID;
        this.Name = Name;
        this.Area = Area;
        this.ProgressStatus = ProgressStatus;
        this.Description = Description;
        this.CompletionStatus = CompletionStatus;
        this.Objectives = Objectives;

        /*
        Debug.Log("Quest Name: " + Name);
        Debug.Log("Quest Description: " + Description);

        foreach (Objective objective in Objectives)
        {
            Debug.Log("Objective: " + objective.Name);
        }
        */
    }
}
