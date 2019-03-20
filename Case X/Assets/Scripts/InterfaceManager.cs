using LitJson;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    #region Diary and Quests interface references
    [Space(10)]
    public GameObject QuestButtonPrefab;
    public GameObject CurrentQuestsDisplay;
    public GameObject AvailableQuestsDisplay;
    public GameObject CompletedQuestsDisplay;
    [Space(10)]
    public GameObject QuestObjectivePrefab;
    public TextMeshProUGUI SelectedQuestTitle;
    public TextMeshProUGUI SelectedQuestDescription;
    public TextMeshProUGUI SelectedQuestStatus;
    public GameObject SelectedQuestObjectivesDisplay;
    private Quest _currentlySelectedQuest;
    #endregion

    #region Gameplay start references
    [Space(10)]
    public GameObject StartMenu;
    #endregion

    #region Item details for bottom UI inventory
    [Space(10)]
    public GameObject BottomUIInventory;
    public GameObject ItemDetailsWindow;
    public Image ItemDetailsPortrait;
    public TextMeshProUGUI ItemDetailsName;
    public TextMeshProUGUI ItemDetailsDescription;
    public TextMeshProUGUI ItemDetailsActives;
    #endregion

    #region Area references
    [Space(10)]
    public GameObject AreaLockedErrorPopup;
    #endregion

    #region Blueprint pieces references
    public Image Blueprint1;
    public Image Blueprint2;
    public Image Blueprint3;
    public Image Blueprint4;
    public Image Blueprint5;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetupQuestsInDiary();
    }

    public void LoadBlueprints()
    {
        Character.Instance.RefreshJsonData();
        string dataToJson = File.ReadAllText(Character.Instance.PlayerStatsFilePath);
        JsonData characterData = JsonMapper.ToObject(dataToJson);
        
        for (int i = 1; i < 6; i++)
        {
            if (characterData["Blueprint" + i].ToString() == "True")
            {
                Sprite blueprintSprite = Resources.Load<Sprite>("Blueprints/Blueprint" + i + "Unlocked");
                if (i == 1)
                {
                    Blueprint1.sprite = blueprintSprite;
                }
                else if (i == 2)
                {
                    Blueprint2.sprite = blueprintSprite;
                }
                else if (i == 3)
                {
                    Blueprint3.sprite = blueprintSprite;
                }
                else if (i == 4)
                {
                    Blueprint4.sprite = blueprintSprite;
                }
                else if (i == 5)
                {
                    Blueprint5.sprite = blueprintSprite;
                }
            }
            else if (characterData["Blueprint" + i].ToString() == "False")
            {
                Sprite blueprintSprite = Resources.Load<Sprite>("Blueprints/Blueprint" + i + "Locked");
                if (i == 1)
                {
                    Blueprint1.sprite = blueprintSprite;
                } else if (i == 2)
                {
                    Blueprint2.sprite = blueprintSprite;
                } else if (i == 3)
                {
                    Blueprint3.sprite = blueprintSprite;
                } else if (i == 4)
                {
                    Blueprint4.sprite = blueprintSprite;
                } else if (i == 5)
                {
                    Blueprint5.sprite = blueprintSprite;
                }
            }
        }
    }

    public void SetupQuestsInDiary()
    {
        if (SceneManager.GetActiveScene().name == "Main Map")
        {
            for (int i = 0; i < CurrentQuestsDisplay.transform.GetChild(0).transform.childCount; i++)
            {
                Destroy(CurrentQuestsDisplay.transform.GetChild(0).transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < AvailableQuestsDisplay.transform.GetChild(0).transform.childCount; i++)
            {
                Destroy(AvailableQuestsDisplay.transform.GetChild(0).transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < CompletedQuestsDisplay.transform.GetChild(0).transform.childCount; i++)
            {
                Destroy(CompletedQuestsDisplay.transform.GetChild(0).transform.GetChild(i).gameObject);
            }

            //Character.Instance.AllQuests.Clear();
            foreach (Quest loadedQuest in Character.Instance.CurrentQuests)
            {
                GameObject newQuestButton = Instantiate(QuestButtonPrefab, CurrentQuestsDisplay.transform.GetChild(0).transform);
                newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
            }

            foreach (Quest loadedQuest in Character.Instance.AvailableQuests)
            {
                GameObject newQuestButton = Instantiate(QuestButtonPrefab, AvailableQuestsDisplay.transform.GetChild(0).transform);
                newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
            }

            foreach (Quest loadedQuest in Character.Instance.CompletedQuests)
            {
                GameObject newQuestButton = Instantiate(QuestButtonPrefab, CompletedQuestsDisplay.transform.GetChild(0).transform);
                newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
            }
        }
    }

    public void ClosePopup(Object obj)
    {
        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(false);
    }

    public void OpenPopup(Object obj)
    {
        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(true);
    }

    // ****************************
    #region Quests and diary functions
    public void DisplayQuestDetails(Object obj)
    {
        GameObject button = obj as GameObject;

        _currentlySelectedQuest = null;
        foreach (Quest loadedQuest in Character.Instance.AllQuests)
        {
            if (loadedQuest.Name == button.GetComponentInChildren<TextMeshProUGUI>().text)
            {
                _currentlySelectedQuest = loadedQuest;

                SelectedQuestTitle.text = "Title: " + loadedQuest.Name;
                SelectedQuestDescription.text = "Description: " + loadedQuest.Description;
                SelectedQuestStatus.text = "Completion Status: " + loadedQuest.ProgressStatus;

                for (int i = 0; i < SelectedQuestObjectivesDisplay.transform.childCount; i++)
                {
                    Destroy(SelectedQuestObjectivesDisplay.transform.GetChild(i).gameObject);
                }

                foreach (Objective objective in loadedQuest.Objectives)
                {
                    GameObject newObjective = Instantiate(QuestObjectivePrefab,
                    SelectedQuestObjectivesDisplay.transform);
                    newObjective.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objective.Name;

                    if (objective.CompletedStatus)
                    {
                        newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Done: No";
                    }
                    else
                    {
                        newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Done: Yes";
                    }
                }
            }
        }
    }

    public void TakeOnQuest()
    {
        if (_currentlySelectedQuest.ProgressStatus == "Available")
        {
            foreach (Quest completedQuest in Character.Instance.CompletedQuests)
            {
                if (completedQuest.Name == _currentlySelectedQuest.Name)
                {
                    Character.Instance.MoveQuestStageStatus(_currentlySelectedQuest, "Completed Quests", "CurrentQuests");
                    completedQuest.ProgressStatus = "Ongoing";
                    _currentlySelectedQuest.CompletionStatus = false;
                    break;
                }
            }
            foreach (Quest availableQuest in Character.Instance.AvailableQuests)
            {
                if (availableQuest.Name == _currentlySelectedQuest.Name)
                {
                    Character.Instance.MoveQuestStageStatus(_currentlySelectedQuest, "Available Quests", "Current Quests");
                    availableQuest.ProgressStatus = "Ongoing";
                    _currentlySelectedQuest.CompletionStatus = false;
                    break;
                }
            }

            _currentlySelectedQuest = null;

            Character.Instance.RefreshAllQuests();
            Character.Instance.SetupQuests();
            SetupQuestsInDiary();

            ResetFields();
        }
    }

    public void AbandonQuest()
    {
        if (_currentlySelectedQuest.ProgressStatus == "Ongoing")
        {
            Character.Instance.MoveQuestStageStatus(_currentlySelectedQuest, "Current Quests", "Available Quests");
            _currentlySelectedQuest.ProgressStatus = "Available";
            _currentlySelectedQuest.CompletionStatus = false;

            _currentlySelectedQuest = null;

            Character.Instance.RefreshAllQuests();
            Character.Instance.SetupQuests();
            SetupQuestsInDiary();

            ResetFields();
        }
    }

    public void ResetFields()
    {
        SelectedQuestTitle.text = string.Empty;
        SelectedQuestDescription.text = string.Empty;
        SelectedQuestStatus.text = string.Empty;

        for (int i = 0; i < SelectedQuestObjectivesDisplay.transform.childCount; i++)
        {
            Destroy(SelectedQuestObjectivesDisplay.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < SelectedQuestObjectivesDisplay.transform.childCount; i++)
        {
            Destroy(SelectedQuestObjectivesDisplay.transform.GetChild(i));
        }
    }
    #endregion
}
