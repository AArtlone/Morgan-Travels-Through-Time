using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProgressLog : MonoBehaviour
{
    public static ProgressLog Instance;
    public List<ProgressEntry> Log = new List<ProgressEntry>();
    private string _pathToLog;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            _pathToLog = Application.persistentDataPath + "/ProgressLog.json";

            if (File.Exists(_pathToLog) == false)
            {
                #region Creating the progress log file
                TextAsset logData = Resources.Load<TextAsset>("Default World Data/ProgressLog");
                File.WriteAllText(_pathToLog, logData.text);
                #endregion
            }
        }
    }

    private void Start()
    {
        SetupLog();
    }

    private void SetupLog()
    {
        JsonData logsJsonData = JsonMapper.ToObject(File.ReadAllText(_pathToLog));

        Log.Clear();
        for (int i = 0; i < logsJsonData["Log"].Count; i++)
        {
            Log.Add(
                new ProgressEntry(
                    logsJsonData["Log"][i]["Milestone"].ToString(),
                    logsJsonData["Log"][i]["Completed"].ToString() == "True" ? true : false
                    ));
        }

        RefreshLog();
    }

    private void RefreshLog()
    {
        string newJsonData = "{";
        newJsonData += InsertNewLineTabs(1);
        newJsonData += "\"Log\": [";

        foreach (ProgressEntry entry in Log)
        {
            newJsonData += InsertNewLineTabs(2);
            newJsonData += "{";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Milestone\": \"" + entry.Milestone + "\",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Completed\": " + (entry.Completed == true ? "true" : "false");
            newJsonData += InsertNewLineTabs(2);
            newJsonData += "},";
        }
        newJsonData = newJsonData.Substring(0, newJsonData.Length - 1);
        newJsonData += InsertNewLineTabs(1);
        newJsonData += "]" + Environment.NewLine + "}";

        File.WriteAllText(_pathToLog, newJsonData);
    }

    public void SetEntry(string milestone, bool newStatus)
    {
        foreach (ProgressEntry entry in Log)
        {
            if (entry.Milestone == milestone)
            {
                entry.Completed = newStatus;
                RefreshLog();
                return;
            }
        }
    }

    public ProgressEntry GetEntry(int index)
    {
        return Log[index];
    }

    public bool IsMilestoneReached(string milestone)
    {
        foreach (ProgressEntry entry in Log)
        {
            if (entry.Milestone == milestone)
            {
                return true;
            } else
            {
                return false;
            }
        }

        return false;
    }

    public int GetLogLength()
    {
        return Log.Count;
    }

    private string InsertNewLineTabs(int numberOfTabs)
    {
        string whiteSpace = Environment.NewLine;
        for (int i = 0; i < numberOfTabs; i++)
        {
            whiteSpace += "\t";
        }

        return whiteSpace;
    }
}
