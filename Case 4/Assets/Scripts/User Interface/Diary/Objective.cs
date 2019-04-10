using System;

[Serializable]
public class Objective
{
    public int ID;
    public string Name;
    public bool CompletedStatus;

    public Objective(int ID, string Name, bool CompletedStatus)
    {
        this.ID = ID;
        this.Name = Name;
        this.CompletedStatus = CompletedStatus;
    }
}
