using LitJson;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DiaryDialogueLogManager : MonoBehaviour
{
    public GameObject LeftPageSection;
    private RectTransform _leftPageSectionRect;
    public GameObject RightPageSection;
    private RectTransform _rightPageSectionRect;
    public GameObject ButtonPrefab;
    public GameObject LogEntryPrefab;

    private DiaryManager _diaryManager;
    private List<GameObject> _buttons = new List<GameObject>();
    private int _maxNumberOfButtons = 8;
    private int _currentPage;

    private int _currentLogEntryIndex;
    private string _currentPageInLogEntries = "Left";
    private string _selectedNPC;
    private float _currentPositionOfLatestButton;

    private void Start()
    {
        _leftPageSectionRect = LeftPageSection.GetComponent<RectTransform>();
        _rightPageSectionRect = RightPageSection.GetComponent<RectTransform>();
        _diaryManager = FindObjectOfType<DiaryManager>();
    }

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

    public void SetupLogEntries(string NPCName)
    {
        _diaryManager.CloseAllPages();

        if (NPCName != _selectedNPC)
        {
            _currentLogEntryIndex = 0;
            _selectedNPC = NPCName;
        }

        JsonData NPCLogs = JsonMapper.ToObject(
            File.ReadAllText(Application.persistentDataPath + "/DialogueLog.json"));

        for (int i = 0; i < NPCLogs["NPC"].Count; i++)
        {
            if (NPCLogs["NPC"][i]["Name"].ToString() == _selectedNPC)
            {
                for (int j = 0; j < NPCLogs["NPC"][i]["Log"].Count; j++)
                {
                    if (NPCLogs["NPC"][i]["Log"][j]["Language"].ToString() == SettingsManager.Instance.Language)
                    {
                        for (int k = 0; k < NPCLogs["NPC"][i]["Log"][j]["Messages"].Count; k++)
                        {
                            if (_currentPageInLogEntries == "Left")
                            {
                                GameObject newEntry = Instantiate(LogEntryPrefab);
                                RectTransform entryRect = newEntry.GetComponent<RectTransform>();
                                newEntry.GetComponent<TextMeshProUGUI>().text = NPCLogs["NPC"][i]["Log"][j]["Messages"][k].ToString();
                                newEntry.transform.SetParent(LeftPageSection.transform);

                                _currentPositionOfLatestButton += entryRect.sizeDelta.y;

                                if (_currentPositionOfLatestButton + entryRect.sizeDelta.y > _leftPageSectionRect.sizeDelta.y)
                                {
                                    newEntry.transform.SetParent(RightPageSection.transform);
                                    _currentPageInLogEntries = "Right";
                                }
                            } else if (_currentPageInLogEntries == "Right")
                            {
                                GameObject newEntry = Instantiate(LogEntryPrefab);
                                RectTransform entryRect = newEntry.GetComponent<RectTransform>();
                                newEntry.GetComponent<TextMeshProUGUI>().text = NPCLogs["NPC"][i]["Log"][j]["Messages"][k].ToString();
                                newEntry.transform.SetParent(RightPageSection.transform);

                                if (_currentPositionOfLatestButton + entryRect.sizeDelta.y > _rightPageSectionRect.sizeDelta.y)
                                {
                                    Debug.Log("Done printing.");
                                    return;
                                }
                            }

                            _currentLogEntryIndex++;
                        }

                        break;
                    }
                }
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
}
