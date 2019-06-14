using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;
    public List<KeyValuePair<int, Item>> Items = new List<KeyValuePair<int, Item>>();
    public GameObject InventoryPanel;
    public GameObject ItemPrefab;
    public GameObject LogsContainer;

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

    [Header("Only populate if current scene requires the inventory!")]
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
    public Image _darkBodyPart;
    [Space(5)]
    public Image _facePart;
    public Image _darkFacePart;
    [Space(5)]
    public Image _hairPart;
    public Image _darkHairPart;
    [Space(5)]
    public Image _topPart;
    public Image _darkTopPart;
    [Space(5)]
    public Image _botPart;
    public Image _darkBotPart;
    [Space(5)]
    public Image _shoesPart;
    public Image _darkShoesPart;
    #endregion

    [Space(10)]
    public Canvas InventoryUICanvas;
    public Animator AnimatorOfFade;
    public CameraBehavior CameraBehavior;
    public GameObject LoadingScreen;

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

    public void FadeEndTutorial()
    {
        AnimatorOfFade.gameObject.SetActive(true);
        AnimatorOfFade.SetBool("IsTransitionBlack", true);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Escape Game")
        {
            SetupQuestsInDiary();
            LoadCharacterAppearance();
        }

        // Once the player enteres a new scene, his previous tap must be halted
        // from overflowing from the previous scene to this one. Because once you
        // change a scene, sometimes it takes me back to the previous one because the
        // button that takes me to one scene is on the same place in the other and
        // they sort of loop or overflow relative to my tap.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touch.phase = TouchPhase.Ended;
        }
    }

    /// <summary>
    /// This function must run whenever a scene containing the inventory backpack
    /// is initiated OR the player's inventory has to be updated.
    /// </summary>
    public void RefreshInvetory()
    {
        Character.Instance.ReloadInventory();
    }

    /// <summary>
    /// This function displays the blueprint parts the player has collected whenever
    /// he enters the blueprints page.
    /// </summary>
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
                }
                else if (i == 2)
                {
                    Blueprint2.sprite = blueprintSprite;
                    Blueprint2.color = new Color(255, 255, 255, 0);
                }
                else if (i == 3)
                {
                    Blueprint3.sprite = blueprintSprite;
                    Blueprint3.color = new Color(255, 255, 255, 0);
                }
                else if (i == 4)
                {
                    Blueprint4.sprite = blueprintSprite;
                    Blueprint4.color = new Color(255, 255, 255, 0);
                }
                else if (i == 5)
                {
                    Blueprint5.sprite = blueprintSprite;
                    Blueprint5.color = new Color(255, 255, 255, 0);
                }
            }
        }
    }

    /// <summary>
    /// This function loads the quest/s in the diary of the player.
    /// </summary>
    public void SetupQuestsInDiary()
    {
        if (SceneManager.GetActiveScene().name != "Escape Game")
        {
            if (CurrentQuestsDisplay != null)
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
            }

            List<Quest> questsToLoad = new List<Quest>();

            if (SettingsManager.Instance != null && Character.Instance != null)
            {
                switch (SettingsManager.Instance.Language)
                {
                    case "English":
                        questsToLoad = Character.Instance.AllQuests;
                        break;
                    case "Dutch":
                        questsToLoad = Character.Instance.AllQuestsDutch;
                        break;
                }
            }

            //Character.Instance.AllQuests.Clear();
            if (CurrentQuestsDisplay != null)
            {
                foreach (Quest loadedQuest in questsToLoad)
                {
                    if (loadedQuest.ProgressStatus == "Ongoing" || loadedQuest.ProgressStatus == "Nog niet gedaan")
                    {
                        GameObject newQuestButton = Instantiate(QuestButtonPrefab, CurrentQuestsDisplay.transform.GetChild(0).transform);
                        newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
                    }
                }

                foreach (Quest loadedQuest in questsToLoad)
                {
                    GameObject newQuestButton = Instantiate(QuestButtonPrefab, AvailableQuestsDisplay.transform.GetChild(0).transform);
                    newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
                }

                foreach (Quest loadedQuest in questsToLoad)
                {
                    if (loadedQuest.ProgressStatus == "Completed" || loadedQuest.ProgressStatus == "Gedaan")
                    {
                        GameObject newQuestButton = Instantiate(QuestButtonPrefab, CompletedQuestsDisplay.transform.GetChild(0).transform);
                        newQuestButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedQuest.Name;
                    }
                }
            }
        }
    }

    /// <summary>
    /// This function hides and shows interfaces in the main menu whenever the
    /// player selects a new interface he wants to display (settings, diary).
    /// </summary>
    /// <param name="ui"></param>
    public void ToggleUI(Object ui)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        GameObject uiObj = (GameObject)ui;

        foreach (GameObject obj in IconDisplays)
        {
            if (obj == uiObj && obj.activeSelf == false)
            {
                if (CameraBehavior != null)
                {
                    CameraBehavior.IsUIOpen = true;
                }

                obj.SetActive(true);

                if (uiObj.transform.tag == "Settings UI")
                {
                    SettingsManager.Instance.UpdateHiglightedLanguageIcons();
                }
            }
            else if (obj == uiObj && obj.activeSelf == true)
            {
                if (CameraBehavior != null)
                {
                    CameraBehavior.IsUIOpen = false;
                    CameraBehavior.IsInterfaceElementSelected = false;
                    CameraBehavior.TapPosition = Camera.main.transform.position;
                }

                obj.SetActive(false);

            }
            else
            {
                obj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Opens a window or pop-up (game object) by activating it.
    /// </summary>
    /// <param name="obj"></param>
    public void ClosePopup(Object obj)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.CloseWindow);

        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(false);

        if (IconDisplays.Contains(popupObject))
        {
            StartCoroutine(Character.Instance.EnableEntityTapping());
            StartCoroutine(Character.Instance.EnableCameraInteraction());
        }
    }

    /// <summary>
    /// Closes a window or pop-up (game object) by disabling it.
    /// </summary>
    /// <param name="obj"></param>
    public void OpenPopup(Object obj)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(true);
    }

    #region Quests, diary and character functions
    public void DisplayQuestDetails(Object obj)
    {
        if (obj == null)
        {
            SelectedQuestTitle.text = string.Empty;
            SelectedQuestDescription.text = string.Empty;
            //SelectedQuestStatus.text = string.Empty;

            for (int i = 0; i < SelectedQuestObjectivesDisplay.transform.childCount; i++)
            {
                Destroy(SelectedQuestObjectivesDisplay.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            GameObject button = obj as GameObject;

            _currentlySelectedQuest = null;

            switch (SettingsManager.Instance.Language)
            {
                case "English":
                    foreach (Quest loadedQuest in Character.Instance.AllQuests)
                    {
                        if (loadedQuest.Name == button.GetComponentInChildren<TextMeshProUGUI>().text)
                        {
                            _currentlySelectedQuest = loadedQuest;

                            SelectedQuestTitle.text = "Title: " + loadedQuest.Name;
                            SelectedQuestDescription.text = "Description: " + loadedQuest.Description;
                            //SelectedQuestStatus.text = "Status: " + loadedQuest.ProgressStatus;

                            for (int i = 0; i < SelectedQuestObjectivesDisplay.transform.childCount; i++)
                            {
                                Destroy(SelectedQuestObjectivesDisplay.transform.GetChild(i).gameObject);
                            }

                            foreach (Objective objective in loadedQuest.Objectives)
                            {
                                if (objective.CompletedStatus == false)
                                {
                                    GameObject newObjective = Instantiate(QuestObjectivePrefab,
                                    SelectedQuestObjectivesDisplay.transform);
                                    newObjective.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objective.Name;
                                    newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Done: No";
                                    break;
                                }
                                else
                                {
                                    GameObject newObjective = Instantiate(QuestObjectivePrefab,
                                    SelectedQuestObjectivesDisplay.transform);
                                    newObjective.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objective.Name;
                                    newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Done: Yes";
                                }
                            }
                        }
                    }
                    break;
                case "Dutch":
                    foreach (Quest loadedQuest in Character.Instance.AllQuestsDutch)
                    {
                        if (loadedQuest.Name == button.GetComponentInChildren<TextMeshProUGUI>().text)
                        {
                            _currentlySelectedQuest = loadedQuest;

                            SelectedQuestTitle.text = "Naam: " + loadedQuest.Name;
                            SelectedQuestDescription.text = "Omschrijving: " + loadedQuest.Description;
                            //SelectedQuestStatus.text = "Status: " + loadedQuest.ProgressStatus;

                            for (int i = 0; i < SelectedQuestObjectivesDisplay.transform.childCount; i++)
                            {
                                Destroy(SelectedQuestObjectivesDisplay.transform.GetChild(i).gameObject);
                            }
                            foreach (Objective objective in loadedQuest.Objectives)
                            {
                                if (objective.CompletedStatus == false)
                                {
                                    GameObject newObjective = Instantiate(QuestObjectivePrefab,
                                    SelectedQuestObjectivesDisplay.transform);
                                    newObjective.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objective.Name;
                                    newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Gedaan: Nee";
                                    break;
                                }
                                else
                                {
                                    GameObject newObjective = Instantiate(QuestObjectivePrefab,
                                    SelectedQuestObjectivesDisplay.transform);
                                    newObjective.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objective.Name;
                                    newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Gedaan: Ja";
                                }
                            }
                        }
                    }
                    break;
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
        //SelectedQuestStatus.text = string.Empty;

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
        if (Character.Instance != null)
        {
            foreach (Clothing clothing in Character.Instance.Wearables)
            {
                if (clothing.BodyPart == "Body" && clothing.Selected == true)
                {
                    foreach (Sprite sprite in _spritesFromStorage)
                    {
                        if (sprite.name == clothing.Name)
                        {
                            if (_bodyIcon != null)
                            {
                                _bodyIcon.sprite = sprite;
                            }
                        }
                    }
                }
                if (clothing.BodyPart == "Face" && clothing.Selected == true)
                {
                    foreach (Sprite sprite in _spritesFromStorage)
                    {
                        if (sprite.name == clothing.Name)
                        {
                            if (_faceIcon != null)
                            {
                                _faceIcon.sprite = sprite;
                            }
                        }
                    }
                }
                if (clothing.BodyPart == "Hair" && clothing.Selected == true)
                {
                    if (_hairIcon != null)
                    {
                        _hairIcon.enabled = true;
                    }
                    foreach (Sprite sprite in _spritesFromStorage)
                    {
                        if (sprite.name == clothing.Name)
                        {
                            if (_hairIcon != null)
                            {
                                _hairIcon.sprite = sprite;
                            }
                        }
                    }
                }
                else if (clothing.BodyPart == "Hair" && clothing.Selected == false)
                {
                    if (_hairIcon != null)
                    {
                        _hairIcon.enabled = false;
                    }
                }
                if (clothing.BodyPart == "Top" && clothing.Selected == true)
                {
                    foreach (Sprite sprite in _spritesFromStorage)
                    {
                        if (sprite.name == clothing.Name)
                        {
                            if (_topIcon != null)
                            {
                                _topIcon.sprite = sprite;
                            }
                        }
                    }
                }
            }
        }
    }

    public void LoadCharacterDialogueAppearance()
    {
        bool isBodySelected = false;
        bool isFaceSelected = false;
        bool isHairSelected = false;
        bool isTopSelected = false;
        bool isBotSelected = false;
        bool isShoesSelected = false;

        Sprite[] _spritesFromStorage = Resources.LoadAll<Sprite>("Clothing/New Clothing");
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == "Body" && clothing.Selected == true)
            {
                isBodySelected = true;
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _bodyPart.sprite = sprite;
                        _darkBodyPart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Face" && clothing.Selected == true)
            {
                isFaceSelected = true;
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _facePart.sprite = sprite;
                        _darkFacePart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Hair" && clothing.Selected == true)
            {
                isHairSelected = true;
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _hairPart.sprite = sprite;
                        _darkHairPart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Top" && clothing.Selected == true)
            {
                isTopSelected = true;
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _topPart.sprite = sprite;
                        _darkTopPart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Bot" && clothing.Selected == true)
            {
                isBotSelected = true;
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _botPart.sprite = sprite;
                        _darkBotPart.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Shoes" && clothing.Selected == true)
            {
                isShoesSelected = true;
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _shoesPart.sprite = sprite;
                        _darkShoesPart.sprite = sprite;
                    }
                }
            }
        }

        if (isBodySelected == false)
        {
            _bodyPart.color = new Color(255, 255, 255, 0);
            _darkBodyPart.color = new Color(255, 255, 255, 0);
        }
        if (isFaceSelected == false)
        {
            _facePart.color = new Color(255, 255, 255, 0);
            _darkFacePart.color = new Color(255, 255, 255, 0);
        }
        if (isHairSelected == false)
        {
            _hairPart.color = new Color(255, 255, 255, 0);
            _darkHairPart.color = new Color(255, 255, 255, 0);
        }
        if (isTopSelected == false)
        {
            _topPart.color = new Color(255, 255, 255, 0);
            _darkTopPart.color = new Color(255, 255, 255, 0);
        }
        if (isBotSelected == false)
        {
            _botPart.color = new Color(255, 255, 255, 0);
            _darkBotPart.color = new Color(255, 255, 255, 0);
        }
        if (isShoesSelected == false)
        {
            _shoesPart.color = new Color(255, 255, 255, 0);
            _darkShoesPart.color = new Color(255, 255, 255, 0);
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
        }
        else
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
        // TODO: Create item actives and test them from here.
    }

    /// <summary>
    /// Map area =/= from main map. This function will load the last scene that is
    /// also considered a map area type.
    /// </summary>
    public void LoadLastMapAreaScene()
    {
        Character.Instance.LastScene = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadLastMapAreaSceneCo());
    }

    public IEnumerator LoadLastMapAreaSceneCo()
    {
        Character.Instance.LastScene = SceneManager.GetActiveScene().name;

        LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(Character.Instance.LastMapArea);

        yield return new WaitForEndOfFrame();
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadSceneCo(scene));
    }

    public IEnumerator LoadSceneCo(string scene)
    {
        LoadingScreen.SetActive(true);

        if (SceneManager.GetActiveScene().name == "Tutorial Map Area" || SceneManager.GetActiveScene().name == "Castle Area" || SceneManager.GetActiveScene().name == "Jacob's House")
        {
            Character.Instance.LastMapArea = SceneManager.GetActiveScene().name;

            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene);
        }
        else
        {
            if (Character.Instance.LastMapArea == "Test Area")
            {
                AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Test Area");
            }
            else
            {
                Character.Instance.LastScene = scene;
                AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(scene);
            }
        }
        Character.Instance.RefreshJsonData();

        yield return new WaitForEndOfFrame();
    }
    #endregion

    public IEnumerator ResetTheGameCo()
    {
        if (SceneManager.GetActiveScene().name == "Beginning Character Creation")
        {
            SettingsManager.Instance.RemoveSingletonInstance();
            Character.Instance.RemoveSingletonInstance();
            SceneManagement.Instance.RemoveSingletonInstance();
            DeleteJsonDataFromStorage();
        }
        else
        {
            SettingsManager.Instance.RemoveSingletonInstance();
            Character.Instance.RemoveSingletonInstance();
            SceneManagement.Instance.RemoveSingletonInstance();
            DeleteJsonDataFromStorage();
        }

        LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Logo Introduction");

        yield return new WaitForEndOfFrame();
    }

    public void ResetTheGame()
    {
        StartCoroutine(ResetTheGameCo());
    }

    public void DeleteJsonDataFromStorage()
    {
        string path = Application.persistentDataPath;
        DirectoryInfo gameDataDirectory = new DirectoryInfo(path);
        FileInfo[] gameData = gameDataDirectory.GetFiles();

        List<FileInfo> filesToRemove = new List<FileInfo>();

        // Here we retrieve the relevant files from the game's data directory
        // and store a list of them for removal.
        foreach (FileInfo file in gameData)
        {
            string fileTypeString = string.Empty;
            for (int i = 0; i < file.Name.Length; i++)
            {
                if (file.Name[i] == '.')
                {
                    fileTypeString = file.Name.Substring(i, file.Name.Length - i);

                    if (fileTypeString == ".json")
                    {
                        filesToRemove.Add(file);
                    }
                }
            }
        }

        // Then we use the list of files to remove to delete the ones that match
        // in the game data directory.
        foreach (FileInfo file in gameData)
        {
            foreach (FileInfo fileToRemove in filesToRemove)
            {
                if (file.Name == fileToRemove.Name)
                {
                    File.Delete(path + "/" + fileToRemove.Name);
                }
            }
        }
    }
}
