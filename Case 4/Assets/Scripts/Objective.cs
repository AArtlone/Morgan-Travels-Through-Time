using System;

[Serializable]
public class Objective
{
    public string Name;
    public bool CompletedStatus;

    public Objective(string Name, bool CompletedStatus)
    {
        this.Name = Name;
        this.CompletedStatus = CompletedStatus;
    }
}
