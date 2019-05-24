using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressEntriesChecker : MonoBehaviour
{
    public List<ProgressEntry> RequiredMilestones;
    public enum Toggle { On, Off};
    public Toggle Switch;

    private void Start()
    {
        var progressLog = ProgressLog.Instance;
        int matches = 0;

        foreach (ProgressEntry progressEntry in RequiredMilestones)
        {
            for (int j = 0; j < progressLog.GetLogLength(); j++)
            {
                if (progressEntry.Milestone == progressLog.GetEntry(j).Milestone)
                {
                    matches++;
                }
            }
        }

        
        if (Switch == Toggle.On)
        {
            if (matches == RequiredMilestones.Count)
            {
                gameObject.SetActive(true);
            } else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (matches == RequiredMilestones.Count)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
