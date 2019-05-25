using LitJson;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DiaryDialogueLogManager : MonoBehaviour
{
    public GameObject ButtonPrefab;
    private List<GameObject> _buttons = new List<GameObject>();
    private int _maxNumberOfButtons = 8;
    private int _currentPage;

    void LoadNewPage(int page)
    {
        ClearPage();

        for (int i = _currentPage * 6; i < _currentPage * 6 + 6; i++)
        {
            _buttons[i].SetActive(true);
        }
    }

    public void LoadPage(string direction)
    {
        if (direction == "Backward")
        {
            _currentPage--;
        }
        else if (direction == "Forward")
        {
            _currentPage++;
        }

        if (_currentPage < 0)
        {
            _currentPage = GetLengthOfButtonPages() - 1;
        }
        if (_currentPage > GetLengthOfButtonPages() - 1)
        {
            _currentPage = 0;
        }

        LoadNewPage(_currentPage);
    }

    private int GetLengthOfButtonPages()
    {
        int lengthOfPagesForButtons = 0;
        for (int i = 0; i < _buttons.Count; i++)
        {
            if (i % _maxNumberOfButtons == 0)
            {
                lengthOfPagesForButtons++;
            }
        }

        Debug.Log(lengthOfPagesForButtons);
        return lengthOfPagesForButtons > 0 ? lengthOfPagesForButtons : 1;
    }

    public void SetupLog()
    {
        JsonData NPCLogs = JsonMapper.ToObject(
            File.ReadAllText(Application.persistentDataPath + "/DialogueLog.json"));

        for (int i = 0; i < NPCLogs["NPC"].Count; i++)
        {
            GameObject button = Instantiate(
                ButtonPrefab,
                InterfaceManager.Instance.LogsContainer.transform);

            button.GetComponentInChildren<TextMeshProUGUI>().text = NPCLogs["NPC"][i]["Name"].ToString();
            button.SetActive(false);

            _buttons.Add(button);
        }

        LoadNewPage(_currentPage);
    }

    void ClearPage()
    {
        for (int i = 0; i < InterfaceManager.Instance.LogsContainer.transform.childCount; i++)
        {
            InterfaceManager.Instance.LogsContainer.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
