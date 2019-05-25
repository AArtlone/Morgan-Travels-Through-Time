using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DiaryDialogueLogManager : MonoBehaviour
{
    int CurrentPage;
    List<List<GameObject>> PagesOfNPCLogs = new List<List<GameObject>>();
    public GameObject ButtonPrefab;
    int MaxNumberOfButtons;

    void LoadNewPage(int page)
    {
        ClearPage();
        for (int i = 0; i < PagesOfNPCLogs[CurrentPage].Count; i++)
        {
            PagesOfNPCLogs[CurrentPage][i].SetActive(true);
        }
    }

    public void LoadPage(string direction)
    {
        if(direction == "left")
        {
            CurrentPage--;
        }
        else if(direction == "right")
        {
            CurrentPage++;
        }
        if(CurrentPage < 0)
        {
            CurrentPage = PagesOfNPCLogs.Count - 1;
        }
        if (PagesOfNPCLogs.Count > 0)
        {
            if (CurrentPage > PagesOfNPCLogs.Count - 1)
            {
                CurrentPage = 0;
            }
        }
        LoadNewPage(CurrentPage);
    }

    public void SetUpLog()
    {
        JsonData NPCLogs = JsonMapper.ToObject(File.ReadAllText(Application.persistentDataPath + "/DialogueLog.json"));
        List<GameObject> NewPage = new List<GameObject>();
        for (int i = 0; i < NPCLogs["NPC"].Count; i++)
        {
            GameObject Button = Instantiate(ButtonPrefab, InterfaceManager.Instance.LogsContainer.transform);
            Button.GetComponentInChildren<TextMeshProUGUI>().text = NPCLogs["NPC"][i]["Name"].ToString();
            NewPage.Add(Button);
            Button.SetActive(false);
            if (NewPage.Count -1 == MaxNumberOfButtons)
            {
                PagesOfNPCLogs.Add(NewPage);
                NewPage.Clear();
            }
        }
    }

    void ClearPage()
    {
        for (int i = 0; i < InterfaceManager.Instance.LogsContainer.transform.childCount; i++)
        {
            InterfaceManager.Instance.LogsContainer.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Start()
    {
        LoadNewPage(CurrentPage);
    }

}
