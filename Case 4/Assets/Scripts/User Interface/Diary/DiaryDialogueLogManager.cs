using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private List<List<string>> _pagesOfEntries = new List<List<string>>();
    private int _maxNumberOfButtons = 8;
    private int _currentPage;

    private int _currentLogEntryIndex;
    private string _currentPageInLogEntries = "Left";
    private string _selectedNPC;
    private float _currentPositionOfLatestButton;
    private int _currentLogEntriesPage;
    private string _lastEntryInLog;
    private List<string> _exploredEntries = new List<string>();

    public TextMeshProUGUI SelectedNPCName;

    private void Start()
    {
        _leftPageSectionRect = LeftPageSection.GetComponent<RectTransform>();
        _rightPageSectionRect = RightPageSection.GetComponent<RectTransform>();
        _diaryManager = FindObjectOfType<DiaryManager>();
    }

    void LoadNewPage(int page)
    {
        ClearPage();

        /*for (int i = _currentPage * 6; i < _currentPage * 6 + 6; i++)
        {
            _buttons[i].SetActive(true);
        }*/
        for (int i = 0; i < _buttons.Count; i++)
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
        for (int i = 0; i < _buttons.Count; i++)
        {
            Destroy(_buttons[i].gameObject);
        }
        _buttons.Clear();

        JsonData NPCLogs = JsonMapper.ToObject(
            File.ReadAllText(Application.persistentDataPath + "/DialogueLog.json"));

        for (int i = 0; i < NPCLogs["NPC"].Count; i++)
        {
            bool isNPCUnlocked = false;
            for (int j = 0; j < NPCLogs["NPC"][i]["Log"].Count; j++)
            {
                if (NPCLogs["NPC"][i]["Log"][j]["Messages"].Count > 0)
                {
                    isNPCUnlocked = true;
                    break;
                }
            }

            if (isNPCUnlocked)
            {
                GameObject button = Instantiate(
                    ButtonPrefab,
                    InterfaceManager.Instance.LogsContainer.transform);

                button.GetComponentInChildren<TextMeshProUGUI>().text = NPCLogs["NPC"][i]["Name"].ToString();
                button.SetActive(false);

                _buttons.Add(button);
            }
        }

        LoadNewPage(_currentPage);
    }

    public void SetupLogEntries(string NPCName)
    {
        _diaryManager.CloseAllPages();

        ClearSection(LeftPageSection);
        ClearSection(RightPageSection);


        if (NPCName != _selectedNPC)
        {
            _currentLogEntryIndex = 0;
            _selectedNPC = NPCName;
        }

        JsonData NPCLogs = JsonMapper.ToObject(
            File.ReadAllText(Application.persistentDataPath + "/DialogueLog.json"));

        SelectedNPCName.text = _selectedNPC;

        for (int i = 0; i < NPCLogs["NPC"].Count; i++)
        {
            if (NPCLogs["NPC"][i]["Name"].ToString() == _selectedNPC)
            {
                for (int j = 0; j < NPCLogs["NPC"][i]["Log"].Count; j++)
                {
                    if (NPCLogs["NPC"][i]["Log"][j]["Language"].ToString() == SettingsManager.Instance.Language)
                    {
                        if (_currentLogEntryIndex < NPCLogs["NPC"][i]["Log"][j]["Messages"].Count)
                        {
                            _lastEntryInLog = NPCLogs["NPC"][i]["Log"][j]["Messages"][NPCLogs["NPC"][i]["Log"][j]["Messages"].Count - 1].ToString();
                            _currentPositionOfLatestButton = 0;
                            List<string> newListOfEntries = new List<string>();
                            bool isDuplicate = false;
                            for (int k = _currentLogEntryIndex; k < NPCLogs["NPC"][i]["Log"][j]["Messages"].Count; k++)
                            {
                                GameObject newEntry;
                                if (_currentPageInLogEntries == "Left")
                                {
                                    newEntry = Instantiate(LogEntryPrefab);
                                    newEntry.name += _currentLogEntryIndex;
                                    RectTransform entryRect = newEntry.GetComponent<RectTransform>();
                                    newEntry.GetComponent<TextMeshProUGUI>().text = NPCLogs["NPC"][i]["Log"][j]["Messages"][k].ToString();
                                    newEntry.transform.SetParent(LeftPageSection.transform);

                                    entryRect.transform.localScale = new Vector3(1, 1, 1);
                                    _currentPositionOfLatestButton += entryRect.sizeDelta.y + Mathf.Abs(entryRect.rect.y);

                                    if (_currentPositionOfLatestButton > _leftPageSectionRect.sizeDelta.y)
                                    {
                                        newEntry.transform.SetParent(RightPageSection.transform);
                                        _currentPageInLogEntries = "Right";

                                        LayoutRebuilder.ForceRebuildLayoutImmediate(_rightPageSectionRect);

                                        _currentPositionOfLatestButton = entryRect.sizeDelta.y;
                                    }
                                    newListOfEntries.Add(newEntry.name);
                                    _exploredEntries.Add(newEntry.GetComponent<TextMeshProUGUI>().text);

                                    LayoutRebuilder.ForceRebuildLayoutImmediate(_leftPageSectionRect);
                                }
                                else if (_currentPageInLogEntries == "Right")
                                {
                                    newEntry = Instantiate(LogEntryPrefab);
                                    newEntry.name += _currentLogEntryIndex;
                                    RectTransform entryRect = newEntry.GetComponent<RectTransform>();
                                    newEntry.GetComponent<TextMeshProUGUI>().text = NPCLogs["NPC"][i]["Log"][j]["Messages"][k].ToString();
                                    newEntry.transform.SetParent(RightPageSection.transform);

                                    entryRect.transform.localScale = new Vector3(1, 1, 1);

                                    _currentPositionOfLatestButton += entryRect.sizeDelta.y + Mathf.Abs(entryRect.rect.y);
                                    newListOfEntries.Add(newEntry.name);
                                    _exploredEntries.Add(newEntry.GetComponent<TextMeshProUGUI>().text);

                                    if (_currentPositionOfLatestButton > _rightPageSectionRect.sizeDelta.y)
                                    {
                                        _currentPageInLogEntries = "Left";

                                        isDuplicate = false;
                                        foreach (List<string> list in _pagesOfEntries)
                                        {
                                            foreach (string obj in list)
                                            {
                                                foreach (string entry in newListOfEntries)
                                                {
                                                    if (entry == obj)
                                                    {
                                                        isDuplicate = true;
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                        }

                                        if (isDuplicate == false && _exploredEntries.Contains(_lastEntryInLog) == false)
                                        {
                                            _pagesOfEntries.Add(newListOfEntries);
                                        }
                                        return;
                                    }

                                    LayoutRebuilder.ForceRebuildLayoutImmediate(_rightPageSectionRect);
                                }

                                _currentLogEntryIndex++;
                            }

                            //isDuplicate = false;
                            //foreach (List<string> list in _pagesOfEntries)
                            //{
                            //    foreach (string obj in list)
                            //    {
                            //        foreach (string entry in newListOfEntries)
                            //        {
                            //            if (entry == obj)
                            //            {
                            //                isDuplicate = true;
                            //                break;
                            //            }
                            //        }
                            //        break;
                            //    }
                            //}

                            //if (isDuplicate == false && _exploredEntries.Contains(_lastEntryInLog) == false)
                            //{
                            //    _pagesOfEntries.Add(newListOfEntries);
                            //    Debug.Log("aaaaaaaaa");
                            //}
                            //break;
                        }
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

    public void LoadNewLogEntriesPage(string direction)
    {
        _currentPageInLogEntries = "Left";
        if (direction == "Backwards" || direction == "Backward")
        {
            if (_pagesOfEntries.Count > 0  && 
                _currentLogEntriesPage > 0)
            {
                _currentLogEntriesPage--;

                if (_currentLogEntriesPage < 0)
                {
                    _currentLogEntriesPage = 0;
                }

                _currentLogEntryIndex = 0;
                if (_currentLogEntriesPage > 0)
                {
                    for (int i = 0; i < _currentLogEntriesPage; i++)
                    {
                        _currentLogEntryIndex += _pagesOfEntries[i].Count;
                    }
                }

                SetupLogEntries(_selectedNPC);
            }
        } else if (direction == "Forwards" || direction == "Forward")
        {
            if (_pagesOfEntries.Count > 0)
            {
                if (_currentLogEntriesPage == 0)
                {
                    _currentLogEntriesPage++;

                    _currentLogEntryIndex = 0;
                    for (int i = 0; i < _currentLogEntriesPage; i++)
                    {
                        _currentLogEntryIndex += _pagesOfEntries[i].Count;
                    }
                    SetupLogEntries(_selectedNPC);
                }
                else
                {
                    if (_currentLogEntriesPage < _pagesOfEntries.Count)
                    {
                        if (_exploredEntries.Contains(_lastEntryInLog) == false)
                        {
                            _currentLogEntriesPage++;

                            _currentLogEntryIndex = 0;
                            for (int i = 0; i < _currentLogEntriesPage; i++)
                            {
                                _currentLogEntryIndex += _pagesOfEntries[i].Count;
                            }

                            SetupLogEntries(_selectedNPC);
                        }

                        _exploredEntries.Clear();
                    }
                }
            }
        }
    }

    private void ClearSection(GameObject section)
    {
        for (int i = 0; i < section.transform.childCount; i++)
        {
            Destroy(section.transform.GetChild(i).gameObject);
        }
    }
}
