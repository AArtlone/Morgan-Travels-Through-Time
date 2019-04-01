using LitJson;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    [Header("Only populate if current scene requires the diary!")]
    [Space(10)]
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

    [Header("Only populate if current scene requires the diary!")]
    [Space(10)]
    #region Item details for bottom UI inventory
    [Space(10)]
    public ItemsLoader ItemsLoader;
    public GameObject BottomUIInventory;
    public GameObject ItemActionsWindow;
    public Item ItemSelected;
    public GameObject ItemDetailsWindow;
    public Image ItemDetailsPortrait;
    public TextMeshProUGUI ItemDetailsName;
    public TextMeshProUGUI ItemDetailsDescription;
    public TextMeshProUGUI ItemDetailsActives;
    #endregion

    [Header("Only populate if current scene is the main map!")]
    #region Area references
    [Space(10)]
    public GameObject AreaLockedErrorPopup;
    #endregion

    [Header("Only populate if current scene requires the diary!")]
    [Space(10)]
    #region Blueprint pieces references
    [Space(10)]
    public Image Blueprint1;
    public Image Blueprint2;
    public Image Blueprint3;
    public Image Blueprint4;
    public Image Blueprint5;
    #endregion
    
    [Header("Only populate if current scene has an interface of icons!")]
    [Space(10)]
    #region Icons display variables
    [Space(10)]
    public List<GameObject> IconDisplays = new List<GameObject>();

    public Image _bodyIcon;
    public Image _faceIcon;
    public Image _hairIcon;
    public Image _topIcon;
    #endregion

    [Header("Only populate if current scene requires NPCs for dialogue!")]
    [Space(10)]
    #region Dialogue body parts display variables
    public Image _bodyPart;
    public Image _facePart;
    public Image _hairPart;
    public Image _topPart;
    public Image _botPart;
    public Image _shoesPart;
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
        LoadCharacterAppearance();

        //if(Character.Instance.TutorialCompleted == false)
        //{
        //    foreach(GameObject mapArea in MapAreas)
        //    {
        //        if(mapArea.name != "Map Area 1")
        //        {
        //            mapArea.SetActive(false);
        //        }
        //    }
        //}
    }

    /// <summary>
    /// This function must run whenever a scene containing the inventory backpack
    /// is initiated OR the player's inventory has to be updated.
    /// </summary>
    public void RefreshInvetory()
    {
        Character.Instance.LoadInventory();
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
                    Blueprint1.color = new Color(255, 255, 255, 1);
                }
                else if (i == 2)
                {
                    Blueprint2.sprite = blueprintSprite;
                    Blueprint2.color = new Color(255, 255, 255, 1);
                }
                else if (i == 3)
                {
                    Blueprint3.sprite = blueprintSprite;
                    Blueprint3.color = new Color(255, 255, 255, 1);
                }
                else if (i == 4)
                {
                    Blueprint4.sprite = blueprintSprite;
                    Blueprint4.color = new Color(255, 255, 255, 1);
                }
                else if (i == 5)
                {
                    Blueprint5.sprite = blueprintSprite;
                    Blueprint5.color = new Color(255, 255, 255, 1);
                }
            }
            else if (characterData["Blueprint" + i].ToString() == "False")
            {
                Sprite blueprintSprite = Resources.Load<Sprite>("Blueprints/Blueprint" + i + "Locked");
                if (i == 1)
                {
                    Blueprint1.sprite = blueprintSprite;
                    Blueprint1.color = new Color(255, 255, 255, 0);
                } else if (i == 2)
                {
                    Blueprint2.sprite = blueprintSprite;
                    Blueprint2.color = new Color(255, 255, 255, 0);
                } else if (i == 3)
                {
                    Blueprint3.sprite = blueprintSprite;
                    Blueprint3.color = new Color(255, 255, 255, 0);
                } else if (i == 4)
                {
                    Blueprint4.sprite = blueprintSprite;
                    Blueprint4.color = new Color(255, 255, 255, 0);
                } else if (i == 5)
                {
                    Blueprint5.sprite = blueprintSprite;
                    Blueprint5.color = new Color(255, 255, 255, 0);
                }
            }
        }
    }

    public void SetupQuestsInDiary()
    {
        if (SceneManager.GetActiveScene().name == "Main Map" || SceneManager.GetActiveScene().name == "Tutorial Map Area")
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
            foreach (Quest loadedQuest in Character.Instance.AllQuests)
            {
                if (loadedQuest.ProgressStatus == "Ongoing")
                {
                    GameObject newQuestButton = Instantiate(QuestButtonPrefab, CurrentQuestsDisplay.transform.GetChild(0).transform);
                    newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
                }
            }

            foreach (Quest loadedQuest in Character.Instance.AllQuests)
            {
                GameObject newQuestButton = Instantiate(QuestButtonPrefab, AvailableQuestsDisplay.transform.GetChild(0).transform);
                newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
            }

            foreach (Quest loadedQuest in Character.Instance.AllQuests)
            {
                if (loadedQuest.ProgressStatus == "Completed")
                {
                    GameObject newQuestButton = Instantiate(QuestButtonPrefab, CompletedQuestsDisplay.transform.GetChild(0).transform);
                    newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
                }
            }
        }
    }

    public void ToggleUI(Object ui)
    {
        GameObject uiObj = (GameObject)ui;

        foreach (GameObject obj in IconDisplays)
        {
            if (obj == uiObj && obj.activeSelf == false)
            {
                obj.SetActive(true);
            }
            else if (obj == uiObj && obj.activeSelf == true)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(false);
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
    
    #region Quests, diary and character functions
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
                SelectedQuestStatus.text = "Status: " + loadedQuest.ProgressStatus;

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
                        newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Done: Yes";
                    }
                    else
                    {
                        newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Done: No";
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

    public void LoadCharacterAppearance()
    {
        Sprite[] _spritesFromStorage = Resources.LoadAll<Sprite>("Clothing/New Clothing");
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == "Body" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _bodyIcon.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Face" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _faceIcon.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Hair" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _hairIcon.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Top" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _topIcon.sprite = sprite;
                    }
                }
            }
        }
    }

    public void LoadCharacterDialogueAppearance()
    {
        Sprite[] _spritesFromStorage = Resources.LoadAll<Sprite>("Clothing/New Clothing");
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == "Body" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _bodyPart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Face" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _facePart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Hair" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _hairPart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Top" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _topPart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Bot" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _botPart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Shoes" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _shoesPart.sprite = sprite;
                    }
                }
            }
        }
    }
    #endregion

    #region Item drops functions
    // Display the menu that gives the player the options of different actions to
    // do on that item he tapped.
    public void DisplayActionsMenu()
    {
        if (ItemActionsWindow.activeSelf == true)
        {
            ClosePopup(ItemActionsWindow);
        } else
        {
            OpenPopup(ItemActionsWindow);
        }
    }

    public void CollectItem()
    {
        Character.Instance.AddItem(ItemSelected);
        Destroy(ItemSelected.gameObject);
    }

    // Displays the selected item on the item details window if the action "view"
    // is pressed by the player.
    public void ViewItem()
    {
        Sprite sprite = Resources.Load<Sprite>("Items/Inventory/" + ItemSelected.AssetsImageName);
        ItemDetailsPortrait.sprite = sprite;
        ItemDetailsName.text = ItemSelected.Name;
        ItemDetailsDescription.text = ItemSelected.Description;
        ItemDetailsActives.text = ItemSelected.Active;

        ItemSelected.DisplayItemDetails();
    }

    public void UseItem()
    {
        Debug.Log("Used " + ItemSelected.Name);
    }
    #endregion
}
