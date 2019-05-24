using System;

[Serializable]
public class ProgressEntry
{
    public string Milestone;
    public bool Completed;

    public ProgressEntry(string milestone, bool completed)
    {
        Milestone = milestone;
        Completed = completed;
    }
}
