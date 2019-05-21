using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogueLogManager : MonoBehaviour
{
    public static DialogueLogManager Instance;
    private string _pathToLog;
    public List<NPCLog> Log = new List<NPCLog>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            _pathToLog = Application.persistentDataPath + "/DialogueLog.json";

            if (File.Exists(_pathToLog) == false)
            {
                #region Creating the dialogue log file
                TextAsset logData = Resources.Load<TextAsset>("Default World Data/DialogueLog");
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

        for (int i = 0; i < logsJsonData["NPC"].Count; i++)
        {
            NPCLog newNPCLog = new NPCLog();
            newNPCLog.Name = logsJsonData["NPC"][i]["Name"].ToString();

            //Debug.Log(logsJsonData["NPC"][i]["Name"]);
            List<DialogueLogObject> listOfOfNewNPCLog = new List<DialogueLogObject>();

            for (int j = 0; j < logsJsonData["NPC"][i]["Log"].Count; j++)
            {
                DialogueLogObject newLogObj = new DialogueLogObject();
                newLogObj.Language = logsJsonData["NPC"][i]["Log"][j]["Language"].ToString();
                //Debug.Log("--->---> " + logsJsonData["NPC"][i]["Log"][j]["Language"].ToString());
                for (int k = 0; k < logsJsonData["NPC"][i]["Log"][j]["Messages"].Count; k++)
                {
                    newLogObj.Messages.Add(logsJsonData["NPC"][i]["Log"][j]["Messages"][k].ToString());
                    //Debug.Log("--->---> " + logsJsonData["NPC"][i]["Log"][j]["Messages"][k].ToString());
                }

                listOfOfNewNPCLog.Add(newLogObj);
            }

            newNPCLog.Log = listOfOfNewNPCLog;
            Log.Add(newNPCLog);
        }
    }

    public void RefreshLog()
    {
        string newJsonData = "{";
        newJsonData += InsertNewLineTabs(1);
        newJsonData += "\"NPC\": [";

        foreach (NPCLog npcLog in Log)
        {
            newJsonData += InsertNewLineTabs(2);
            newJsonData += "{";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Name\": \"" + npcLog.Name + "\",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Log\": [";
            foreach (DialogueLogObject logObj in npcLog.Log)
            {
                newJsonData += InsertNewLineTabs(4);
                newJsonData += "{";
                newJsonData += InsertNewLineTabs(5);
                newJsonData += "\"Language\": \"" + logObj.Language + "\", ";
                newJsonData += InsertNewLineTabs(5);
                newJsonData += "\"Messages\": [ "; // I added one extra space just in case the substring removed 
                foreach (string message in logObj.Messages)
                {
                    newJsonData += InsertNewLineTabs(6);
                    string messageConvertedForJson = JsonConvert.ToString(message);
                    messageConvertedForJson = messageConvertedForJson.Substring(1, messageConvertedForJson.Length - 2);
                    newJsonData += "\"" + messageConvertedForJson + "\",";
                }
                newJsonData = newJsonData.Substring(0, newJsonData.Length - 1);
                newJsonData += InsertNewLineTabs(5);
                newJsonData += "]";
                newJsonData += InsertNewLineTabs(4);
                newJsonData += "},";
            }
            newJsonData = newJsonData.Substring(0, newJsonData.Length - 1);
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "]";
            newJsonData += InsertNewLineTabs(2);
            newJsonData += "},";
        }

        newJsonData = newJsonData.Substring(0, newJsonData.Length - 1);
        newJsonData += InsertNewLineTabs(1) + "]" + Environment.NewLine + "}";
        File.WriteAllText(_pathToLog, newJsonData);
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

    public void AddNewDialogue(string NPCName, string language, string message)
    {
        foreach (NPCLog npcLog in Log)
        {
            if (npcLog.Name == NPCName)
            {
                foreach (DialogueLogObject logObj in npcLog.Log)
                {
                    if (logObj.Language == language)
                    {
                        bool isMessageInLogAlready = false;
                        foreach (string messageInLog in logObj.Messages)
                        {
                            if (message == messageInLog)
                            {
                                isMessageInLogAlready = true;
                            }
                        }

                        if (isMessageInLogAlready == false)
                        {
                            logObj.Messages.Add(message);

                            //Debug.LogWarning("The new message has been added to the log!");
                        } else
                        {
                            //Debug.LogWarning("The following message is already in the log: " + message + "!");
                        }
                    }
                }
            }
        }

        RefreshLog();
    }
}
