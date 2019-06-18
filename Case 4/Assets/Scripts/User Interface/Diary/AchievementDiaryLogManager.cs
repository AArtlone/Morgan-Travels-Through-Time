using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDiaryLogManager : MonoBehaviour
{
    public GameObject LogEntryPrefab;
    public GameObject LeftPageSection;
    private RectTransform _leftPageSectionRect;
    public GameObject RightPageSection;
    private RectTransform _rightPageSectionRect;

    private int _currentPage;
    private int _currentLogEntryIndex;
    private float _currentPositionOfLatestButton;
    private int _currentLogEntriesPage;
    private string _lastEntryInLog;
    private string _currentPageInLogEntries = "Left";
    private List<string> _exploredEntries = new List<string>();
    private List<List<string>> _pagesOfEntries = new List<List<string>>();

    private void Start()
    {
        _leftPageSectionRect = LeftPageSection.GetComponent<RectTransform>();
        _rightPageSectionRect = RightPageSection.GetComponent<RectTransform>();
    }

    public void LoadPage(string direction)
    {
        ClearPage();

        if (direction == "Backward")
        {
            _currentPage--;
        }
        else if (direction == "Forward")
        {
            _currentPage++;
        }
    }

    public void SetupLog()
    {
        ClearSection(LeftPageSection);
        ClearSection(RightPageSection);

        _currentPageInLogEntries = "Left";
        _currentPositionOfLatestButton = 0f;

        List<string> newListOfEntries = new List<string>();
        bool isDuplicate = false;
        if (_currentLogEntryIndex < AchievementManager.Instance.AllAchievements.Count)
        {
            for (int i = _currentLogEntryIndex; i < AchievementManager.Instance.AllAchievements.Count; i++)
            {
                if (AchievementManager.Instance.AllAchievements[i].Status == true)
                {
                    if (newListOfEntries.Contains(AchievementManager.Instance.AllAchievements[i].Name) == false)
                    {
                        newListOfEntries.Add(AchievementManager.Instance.AllAchievements[i].Name);
                    }

                    switch (SettingsManager.Instance.Language)
                    {
                        case "English":
                            _lastEntryInLog = AchievementManager.Instance.AllAchievements[AchievementManager.Instance.AllAchievements.Count - 1].Name;
                            break;
                        case "Dutch":
                            _lastEntryInLog = AchievementManager.Instance.AllAchievements[AchievementManager.Instance.AllAchievements.Count - 1].NameDutch;
                            break;
                    }
                    GameObject achievementLogEntry;
                    if (_currentPageInLogEntries == "Left")
                    {
                        achievementLogEntry = Instantiate(LogEntryPrefab);
                        RectTransform entryParent = achievementLogEntry.GetComponent<RectTransform>();
                        RectTransform entryRect = achievementLogEntry.GetComponentsInChildren<RectTransform>()[1];
                        if (SettingsManager.Instance.Language == "English")
                        {
                            achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = AchievementManager.Instance.AllAchievements[i].Name;
                        }
                        else if (SettingsManager.Instance.Language == "Dutch")
                        {
                            achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = AchievementManager.Instance.AllAchievements[i].NameDutch;
                        }
                        achievementLogEntry.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Achievements/" + AchievementManager.Instance.AllAchievements[i].Icon);
                        achievementLogEntry.transform.SetParent(LeftPageSection.transform);
                        achievementLogEntry.transform.localScale = new Vector3(1, 1, 1);
                        _currentPositionOfLatestButton += entryRect.sizeDelta.y + Mathf.Abs(entryRect.rect.y);
                        if (AchievementManager.Instance.AllAchievements[0] != AchievementManager.Instance.AllAchievements[i])
                        {
                            entryParent.sizeDelta = new Vector2(entryRect.sizeDelta.x, entryRect.sizeDelta.y + 15);
                        }
                        else
                        {
                            entryParent.sizeDelta = new Vector2(entryRect.sizeDelta.x, entryRect.sizeDelta.y);
                        }
                        if (_currentPositionOfLatestButton > _leftPageSectionRect.sizeDelta.y)
                        {
                            achievementLogEntry.transform.SetParent(RightPageSection.transform);
                            _currentPageInLogEntries = "Right";
                            LayoutRebuilder.ForceRebuildLayoutImmediate(_rightPageSectionRect);

                            _currentPositionOfLatestButton = entryRect.sizeDelta.y;
                        }
                        _exploredEntries.Add(achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text);

                        LayoutRebuilder.ForceRebuildLayoutImmediate(_leftPageSectionRect);
                    }
                    else if (_currentPageInLogEntries == "Right")
                    {
                        achievementLogEntry = Instantiate(LogEntryPrefab);
                        RectTransform entryRect = achievementLogEntry.GetComponentsInChildren<RectTransform>()[1];
                        RectTransform entryParent = achievementLogEntry.GetComponent<RectTransform>();
                        if (SettingsManager.Instance.Language == "English")
                        {
                            achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = AchievementManager.Instance.AllAchievements[i].Name;
                        }
                        else if (SettingsManager.Instance.Language == "Dutch")
                        {
                            achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = AchievementManager.Instance.AllAchievements[i].NameDutch;
                        }
                        achievementLogEntry.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Achievements/" + AchievementManager.Instance.AllAchievements[i].Icon);
                        achievementLogEntry.transform.SetParent(RightPageSection.transform);

                        achievementLogEntry.transform.localScale = new Vector3(1, 1, 1);
                        _currentPositionOfLatestButton += entryRect.sizeDelta.y + Mathf.Abs(entryRect.rect.y);
                        entryParent.sizeDelta = new Vector2(entryRect.sizeDelta.x, entryRect.sizeDelta.y + 15);
                        if (_currentPositionOfLatestButton > _rightPageSectionRect.sizeDelta.y)
                        {
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

                            if (isDuplicate == false)
                            {
                                _pagesOfEntries.Add(newListOfEntries);
                            }

                            _currentPageInLogEntries = "Left";
                            return;
                        }
                        LayoutRebuilder.ForceRebuildLayoutImmediate(_rightPageSectionRect);
                    }
                    if (AchievementManager.Instance.AllAchievements[i].Name == AchievementManager.Instance.AllAchievements[AchievementManager.Instance.AllAchievements.Count - 1].Name)
                    {
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

                        if (isDuplicate == false)
                        {
                            _pagesOfEntries.Add(newListOfEntries);
                        }

                        _currentPageInLogEntries = "Left";
                        return;
                    }
                }
                
                _currentLogEntryIndex++;
            }
        }
    }

    public void LoadNewLogEntriesPage(string direction)
    {
        _currentPageInLogEntries = "Left";
        if (direction == "Backwards" || direction == "Backward")
        {
            if (_currentLogEntriesPage > 0)
            {
                _currentLogEntriesPage--;

                _currentLogEntryIndex = 0;
                if (_currentLogEntriesPage > 0)
                {
                    for (int i = 0; i < _currentLogEntriesPage; i++)
                    {
                        _currentLogEntryIndex += _pagesOfEntries[i].Count;
                    }
                }

                SetupLog();
            }
        }
        else if (direction == "Forwards" || direction == "Forward")
        {
            if (_currentLogEntryIndex < AchievementManager.Instance.AllAchievements.Count - 1)
            {
                _currentLogEntriesPage++;
                _currentLogEntryIndex++;
                SetupLog();
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

    private void ClearSection(GameObject section)
    {
        for (int i = 0; i < section.transform.childCount; i++)
        {
            Destroy(section.transform.GetChild(i).gameObject);
        }
    }
}
