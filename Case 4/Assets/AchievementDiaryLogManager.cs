using LitJson;
using System.Collections;
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
    private string _currentPageInLogEntries = "Left";
    private List<string> _exploredEntries = new List<string>();
    private List<List<string>> _pagesOfEntries = new List<List<string>>();


    private void Start()
    {
        _leftPageSectionRect = LeftPageSection.GetComponent<RectTransform>();
        _rightPageSectionRect = RightPageSection.GetComponent<RectTransform>();
        Debug.Log(AchievementManager.Instance.AllAchievements.Count);
        SetupLog();
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

        //if (_currentPage < 0)
        //{
        //    _currentPage = GetLengthOfButtonPages() - 1;
        //}
        //if (_currentPage > GetLengthOfButtonPages() - 1)
        //{
        //    _currentPage = 0;
        //}

        //LoadNewPage(_currentPage);
    }

    private void SetupLog()
    {
        ClearSection(LeftPageSection);
        ClearSection(RightPageSection);

        Debug.Log(AchievementManager.Instance.AllAchievements.Count);
        foreach (Achievement achievement in AchievementManager.Instance.AllAchievements)
        {
            List<string> newListOfEntries = new List<string>();
            bool isDuplicate = false;
            _currentPositionOfLatestButton = 0;
            GameObject achievementLogEntry;
            if (_currentPageInLogEntries == "Left")
            {
                achievementLogEntry = Instantiate(LogEntryPrefab);
                RectTransform entryRect = achievementLogEntry.GetComponent<RectTransform>();
                if (SettingsManager.Instance.Language == "English")
                {
                    achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = achievement.Name;
                }
                else if (SettingsManager.Instance.Language == "Dutch")
                {
                    achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = achievement.NameDutch;
                }
                achievementLogEntry.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Achievements/" + achievement.Icon);
                achievementLogEntry.transform.SetParent(LeftPageSection.transform);
                _currentPositionOfLatestButton += entryRect.sizeDelta.y + Mathf.Abs(entryRect.rect.y);
                if(_currentPositionOfLatestButton > _leftPageSectionRect.sizeDelta.y)
                                {
                    achievementLogEntry.transform.SetParent(RightPageSection.transform);
                    _currentPageInLogEntries = "Right";
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_rightPageSectionRect);

                    _currentPositionOfLatestButton = entryRect.sizeDelta.y;
                    newListOfEntries.Add(achievementLogEntry.name);
                    _exploredEntries.Add(achievementLogEntry.GetComponent<TextMeshProUGUI>().text);

                    LayoutRebuilder.ForceRebuildLayoutImmediate(_leftPageSectionRect);
                }
            } else if (_currentPageInLogEntries == "Right")
            {
                achievementLogEntry = Instantiate(LogEntryPrefab);
                RectTransform entryRect = achievementLogEntry.GetComponent<RectTransform>();
                if (SettingsManager.Instance.Language == "English")
                {
                    achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = achievement.Name;
                }
                else if (SettingsManager.Instance.Language == "Dutch")
                {
                    achievementLogEntry.GetComponentInChildren<TextMeshProUGUI>().text = achievement.NameDutch;
                }
                achievementLogEntry.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Achievements/" + achievement.Icon);
                achievementLogEntry.transform.SetParent(RightPageSection.transform);
                _currentPositionOfLatestButton += entryRect.sizeDelta.y + Mathf.Abs(entryRect.rect.y);

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

                    if (isDuplicate == false)
                    {
                        _pagesOfEntries.Add(newListOfEntries);
                    }
                    return;
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(_rightPageSectionRect);
            }
            _currentLogEntryIndex++;

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
