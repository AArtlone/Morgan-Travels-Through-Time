using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class Character : MonoBehaviour
{
    public static Character Instance;
    public string Name;
    public bool IsDataCreated;
    public bool CharacterCreation;
    public bool TutorialCompleted;
    public bool HasMap;
    public bool HasDiary;
    public bool AreIconsExplained;
    public bool IsCutscenePassed;
    public string LastMapArea;
    public string LastScene;
    [Space(10)]
    #region Stats
    public int Reputation;
    public int Stamina;
    public int Knowledge;
    public int Fitness;
    public int Charisma;
    public int Currency;
    public int AvailableHints;
    #endregion
    #region Items
    public List<Clothing> Wearables = new List<Clothing>();
    public List<Clothing> WearablesDutch = new List<Clothing>();
    [Space(10)]
    public List<Item> Items = new List<Item>();
    public List<Item> ItemsDutch = new List<Item>();
    #endregion
    public List<Area> Areas = new List<Area>();
    #region Quests references
    public List<Quest> AvailableQuests = new List<Quest>();
    public List<Quest> CurrentQuests = new List<Quest>();
    public List<Quest> CompletedQuests = new List<Quest>();
    public List<Quest> AllQuests = new List<Quest>();
    public List<Quest> AllQuestsDutch = new List<Quest>();
    #endregion
    #region Puzzle references
    public List<HiddenObjectsPuzzle> HiddenObjectsPuzzles = new List<HiddenObjectsPuzzle>();
    #endregion
    [Space(10)]
    public string DateOfLastCoffee;

    #region Json files location reference of player data.
    private string _newLine = Environment.NewLine;
    private string _pathToAssetsFolder;
    public string PlayerStatsFilePath;
    private string _areasJsonFilePath;
    private string _questsJsonFilePath;
    private string _questsDutchJsonFilePath;
    private string _itemsJsonFilePath;
    private string _itemsDutchJsonFilePath;
    private string _wearablesJsonFilePath;
    private string _wearablesDutchJsonFilePath;
    private string _newQuestsData;
    private string _newQuestsDutchData;
    private string _guessingPuzzlesPath;
    private string _escapeGamesJsonFile;
    private string _puzzlesJsonPath;
    #endregion

    #region Blueprints references
    public bool Blueprint1;
    public bool Blueprint2;
    public bool Blueprint3;
    public bool Blueprint4;
    public bool Blueprint5;
    #endregion

    // Inventory-related references
    public GameObject ItemPrefab;
    public CameraBehavior CameraBehavior;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            // We want to be able to access the dialogue information from any scene.
            DontDestroyOnLoad(gameObject);

            TextAsset playerData = Resources.Load<TextAsset>("Default World Data/Player");
            JsonData playerJsonData = JsonMapper.ToObject(playerData.text);

            _pathToAssetsFolder = Application.persistentDataPath;
            PlayerStatsFilePath = _pathToAssetsFolder + "/Player.json";
            _areasJsonFilePath = _pathToAssetsFolder + "/Areas.json";
            _questsJsonFilePath = _pathToAssetsFolder + "/Quests.json";
            _questsDutchJsonFilePath = _pathToAssetsFolder + "/QuestsDutch.json";
            _itemsJsonFilePath = _pathToAssetsFolder + "/Items.json";
            _itemsDutchJsonFilePath = _pathToAssetsFolder + "/ItemsDutch.json";
            _wearablesJsonFilePath = _pathToAssetsFolder + "/Wearables.json";
            _wearablesDutchJsonFilePath = _pathToAssetsFolder + "/WearablesDutch.json";
            _guessingPuzzlesPath = _pathToAssetsFolder + "/GuessingPuzzles.json";
            _escapeGamesJsonFile = _pathToAssetsFolder + "/EscapeGames.json";
            _puzzlesJsonPath = _pathToAssetsFolder + "/HiddenObjectPuzzles.json";

            if (File.Exists(PlayerStatsFilePath))
            {
                string dataToJson = File.ReadAllText(PlayerStatsFilePath);
                JsonData characterData = JsonMapper.ToObject(dataToJson);

                // These functions initialize the game state from the storage.
                SetupWorldData();
            }
            else
            {
                // This reads every default file settings for the world data
                // and we use it to setup the starting player, Quests, items
                // and so on collections of data for when the game first starts.
                if (IsDataCreated == false)
                {
                    #region Creating the player file
                    Name = playerJsonData["Name"].ToString();
                    IsDataCreated = true;

                    if (playerJsonData["CharacterCreation"].ToString() == "True")
                    {
                        CharacterCreation = true;
                    }
                    else if (playerJsonData["CharacterCreation"].ToString() == "False")
                    {
                        CharacterCreation = false;
                    }

                    if (playerJsonData["TutorialCompleted"].ToString() == "True")
                    {
                        TutorialCompleted = true;
                    }
                    else if (playerJsonData["TutorialCompleted"].ToString() == "False")
                    {
                        TutorialCompleted = false;
                    }

                    if (playerJsonData["HasMap"].ToString() == "True")
                    {
                        HasMap = true;
                    }
                    else if (playerJsonData["HasMap"].ToString() == "False")
                    {
                        HasMap = false;
                    }

                    if (playerJsonData["HasDiary"].ToString() == "True")
                    {
                        HasDiary = true;
                    }
                    else if (playerJsonData["HasDiary"].ToString() == "False")
                    {
                        HasDiary = false;
                    }

                    if (playerJsonData["AreIconsExplained"].ToString() == "True")
                    {
                        AreIconsExplained = true;
                    }
                    else if (playerJsonData["AreIconsExplained"].ToString() == "False")
                    {
                        AreIconsExplained = false;
                    }

                    if (playerJsonData["IsCutscenePassed"].ToString() == "True")
                    {
                        IsCutscenePassed = true;
                    }
                    else if (playerJsonData["IsCutscenePassed"].ToString() == "False")
                    {
                        IsCutscenePassed = false;
                    }

                    #region Blueprints
                    for (int i = 1; i < 6; i++)
                    {
                        bool isBlueprintCollected = false;
                        if (playerJsonData["Blueprint" + i].ToString() == "True")
                        {
                            isBlueprintCollected = true;
                        }
                        else if (playerJsonData["Blueprint" + i].ToString() == "False")
                        {
                            isBlueprintCollected = false;
                        }

                        if (i == 1)
                        {
                            Blueprint1 = isBlueprintCollected;
                        }
                        else if (i == 2)
                        {
                            Blueprint2 = isBlueprintCollected;
                        }
                        else if (i == 3)
                        {
                            Blueprint3 = isBlueprintCollected;
                        }
                        else if (i == 4)
                        {
                            Blueprint4 = isBlueprintCollected;
                        }
                        else if (i == 5)
                        {
                            Blueprint5 = isBlueprintCollected;
                        }
                    }
                    #endregion

                    LastMapArea = playerJsonData["LastMapArea"].ToString();
                    LastScene = playerJsonData["LastScene"].ToString();
                    Reputation = int.Parse(playerJsonData["Reputation"].ToString());
                    Stamina = int.Parse(playerJsonData["Stamina"].ToString());
                    Knowledge = int.Parse(playerJsonData["Knowledge"].ToString());
                    Fitness = int.Parse(playerJsonData["Fitness"].ToString());
                    Charisma = int.Parse(playerJsonData["Charisma"].ToString());
                    Currency = int.Parse(playerJsonData["Currency"].ToString());

                    AvailableHints = int.Parse(playerJsonData["AvailableHints"].ToString());
                    DateOfLastCoffee = playerJsonData["DateOfLastCoffee"].ToString();

                    string newPlayerData = JsonUtility.ToJson(this);
                    File.WriteAllText(PlayerStatsFilePath, newPlayerData.ToString());
                    #endregion

                    #region Creating the areas list
                    TextAsset areasData = Resources.Load<TextAsset>("Default World Data/Areas");
                    JsonData areasJsonData = JsonMapper.ToObject(areasData.text);

                    Areas.Clear();
                    for (int i = 0; i < areasJsonData["Areas"].Count; i++)
                    {
                        Area.AreaStatus areaType = Area.AreaStatus.Locked;
                        if (areasJsonData["Areas"][i]["Status"].ToString() == "Locked")
                        {
                            areaType = Area.AreaStatus.Locked;
                        }
                        else if (areasJsonData["Areas"][i]["Status"].ToString() == "Unlocked")
                        {
                            areaType = Area.AreaStatus.Unlocked;
                        }

                        Areas.Add(
                            new Area(areasJsonData["Areas"][i]["Name"].ToString(),
                            areaType));
                    }

                    File.WriteAllText(_areasJsonFilePath, areasJsonData.ToJson());
                    #endregion

                    #region Creating the quests list
                    TextAsset questsData = Resources.Load<TextAsset>("Default World Data/Quests");
                    JsonData questsJsonData = JsonMapper.ToObject(questsData.text);

                    TextAsset questsDutchData = Resources.Load<TextAsset>("Default World Data/QuestsDutch");
                    JsonData questsDutchJsonData = JsonMapper.ToObject(questsDutchData.text);

                    AllQuests.Clear();
                    for (int i = 0; i < questsJsonData["Quests"].Count; i++)
                    {
                        List<Objective> newQuestObjectives = new List<Objective>();

                        for (int j = 0; j < questsJsonData["Quests"][i]["Objectives"].Count; j++)
                        {
                            // These conditions make sure that we get the right type of data
                            // from the json and convert it accurately for the dictionaries
                            // of objectives for that quest later on.
                            bool isObjectiveComplete = false;
                            if (questsJsonData["Quests"][i]["Objectives"][j]["CompletedStatus"].ToString() == "True")
                            {
                                isObjectiveComplete = true;
                            }
                            else if (questsJsonData["Quests"][i]["Objectives"][j]["CompletedStatus"].ToString() == "False")
                            {
                                isObjectiveComplete = false;
                            }

                            // Here we store the new dictionary (objective) to the list of
                            // objectives after we set up the new objective.
                            Objective newObjectives =
                                new Objective(
                                    int.Parse(questsJsonData["Quests"][i]["Objectives"][j]["ID"].ToString()),
                                    questsJsonData["Quests"][i]["Objectives"][j]["Name"].ToString(), isObjectiveComplete);

                            newQuestObjectives.Add(newObjectives);
                        }

                        bool statusOfQuestCompletion = false;
                        if (questsJsonData["Quests"][i]["Completed"].ToString() == "True")
                        {
                            statusOfQuestCompletion = true;
                        }
                        else if (questsJsonData["Quests"][i]["Completed"].ToString() == "False")
                        {
                            statusOfQuestCompletion = false;
                        }

                        Quest newQuest = new Quest(
                            int.Parse(questsJsonData["Quests"][i]["ID"].ToString()),
                            questsJsonData["Quests"][i]["Name"].ToString(),
                            questsJsonData["Quests"][i]["Area"].ToString(),
                            questsJsonData["Quests"][i]["ProgressStatus"].ToString(),
                            questsJsonData["Quests"][i]["Description"].ToString(),
                            statusOfQuestCompletion,
                            newQuestObjectives);

                        AllQuests.Add(newQuest);
                    }

                    AllQuestsDutch.Clear();
                    for (int i = 0; i < questsDutchJsonData["Quests"].Count; i++)
                    {
                        List<Objective> newQuestObjectives = new List<Objective>();

                        for (int j = 0; j < questsDutchJsonData["Quests"][i]["Objectives"].Count; j++)
                        {
                            // These conditions make sure that we get the right type of data
                            // from the json and convert it accurately for the dictionaries
                            // of objectives for that quest later on.
                            bool isObjectiveComplete = false;
                            if (questsDutchJsonData["Quests"][i]["Objectives"][j]["CompletedStatus"].ToString() == "True")
                            {
                                isObjectiveComplete = true;
                            }
                            else if (questsDutchJsonData["Quests"][i]["Objectives"][j]["CompletedStatus"].ToString() == "False")
                            {
                                isObjectiveComplete = false;
                            }

                            // Here we store the new dictionary (objective) to the list of
                            // objectives after we set up the new objective.
                            Objective newObjectives =
                                new Objective(
                                    int.Parse(questsDutchJsonData["Quests"][i]["Objectives"][j]["ID"].ToString()),
                                    questsDutchJsonData["Quests"][i]["Objectives"][j]["Name"].ToString(), isObjectiveComplete);

                            newQuestObjectives.Add(newObjectives);
                        }

                        bool statusOfQuestCompletion = false;
                        if (questsDutchJsonData["Quests"][i]["Completed"].ToString() == "True")
                        {
                            statusOfQuestCompletion = true;
                        }
                        else if (questsDutchJsonData["Quests"][i]["Completed"].ToString() == "False")
                        {
                            statusOfQuestCompletion = false;
                        }

                        Quest newQuest = new Quest(
                            int.Parse(questsDutchJsonData["Quests"][i]["ID"].ToString()),
                            questsDutchJsonData["Quests"][i]["Name"].ToString(),
                            questsDutchJsonData["Quests"][i]["Area"].ToString(),
                            questsDutchJsonData["Quests"][i]["ProgressStatus"].ToString(),
                            questsDutchJsonData["Quests"][i]["Description"].ToString(),
                            statusOfQuestCompletion,
                            newQuestObjectives);

                        AllQuestsDutch.Add(newQuest);
                    }

                    File.WriteAllText(_questsJsonFilePath, questsData.text);
                    File.WriteAllText(_questsDutchJsonFilePath, questsDutchData.text);
                    RefreshAllQuests();
                    #endregion

                    #region Creating the items list BASED on the default language
                    InstantiateItems();
                    //RefreshItems();
                    #endregion

                    #region Creating the wearables list
                    TextAsset wearablesData = Resources.Load<TextAsset>("Default World Data/Wearables");
                    JsonData wearablesJsonData = JsonMapper.ToObject(wearablesData.text);

                    TextAsset wearablesDataDutch = Resources.Load<TextAsset>("Default World Data/WearablesDutch");
                    JsonData wearablesJsonDataDutch = JsonMapper.ToObject(wearablesDataDutch.text);

                    Wearables.Clear();
                    WearablesDutch.Clear();
                    for (int i = 0; i < wearablesJsonData["Wearables"].Count; i++)
                    {
                        bool isWearableSelected = false;
                        if (wearablesJsonData["Wearables"][i]["Selected"].ToString() == "True")
                        {
                            isWearableSelected = true;
                        }
                        else if (wearablesJsonData["Wearables"][i]["Selected"].ToString() == "False")
                        {
                            isWearableSelected = false;
                        }

                        Clothing clothing = new Clothing(
                            isWearableSelected,
                            wearablesJsonData["Wearables"][i]["BodyPart"].ToString(),
                            wearablesJsonData["Wearables"][i]["Name"].ToString(),
                            wearablesJsonDataDutch["Wearables"][i]["Name"].ToString(),
                            wearablesJsonData["Wearables"][i]["Icon"].ToString(),
                            wearablesJsonData["Wearables"][i]["PortraitImage"].ToString(),
                            int.Parse(wearablesJsonData["Wearables"][i]["Stamina"].ToString()),
                            int.Parse(wearablesJsonData["Wearables"][i]["Knowledge"].ToString()),
                            int.Parse(wearablesJsonData["Wearables"][i]["Fitness"].ToString()),
                            int.Parse(wearablesJsonData["Wearables"][i]["Charisma"].ToString()));

                        Wearables.Add(clothing);
                        WearablesDutch.Add(clothing);
                    }

                    File.WriteAllText(_wearablesJsonFilePath, wearablesData.text);
                    File.WriteAllText(_wearablesDutchJsonFilePath, wearablesDataDutch.text);
                    RefreshWearables();
                    #endregion

                    #region Creating the default guessing clothes puzzles data list
                    TextAsset puzzleData = Resources.Load<TextAsset>("Default World Data/GuessingPuzzles");
                    File.WriteAllText(_guessingPuzzlesPath, puzzleData.text);
                    #endregion

                    #region Creating the default escape games data list
                    TextAsset escapeGamesData = Resources.Load<TextAsset>("Default World Data/EscapeGames");
                    File.WriteAllText(_escapeGamesJsonFile, escapeGamesData.text);
                    #endregion

                    #region Creating the puzzles file
                    TextAsset puzzlesData = Resources.Load<TextAsset>("Default World Data/HiddenObjectPuzzles");
                    File.WriteAllText(_puzzlesJsonPath, puzzlesData.text);
                    #endregion
                }
            }
        }
        RefreshItems();
    }

    #region Setup data from storage functionality
    // The following functions extract the json data from the storage of
    // the game and imports it to the character (this) object for in-game
    // manipulation.

    /// <summary>
    /// Given a default items json file, it will extract its data and store it in a
    /// new or existing (overwrittes it) file and to the character's specified list
    /// of items (multiple languages, multiple lists).
    /// </summary>
    /// <param name="ResourceFilePath"></param>
    /// <param name="NewFilePath"></param>
    /// <param name="listForItems"></param>
    private void InstantiateItems()
    {
        TextAsset itemsData = Resources.Load<TextAsset>("Default World Data/Items");
        JsonData itemsJsonData = JsonMapper.ToObject(itemsData.text);

        TextAsset itemsDataDutch = Resources.Load<TextAsset>("Default World Data/ItemsDutch");
        JsonData itemsJsonDataDutch = JsonMapper.ToObject(itemsDataDutch.text);

        Items.Clear();
        ItemsDutch.Clear();
        for (int i = 0; i < itemsJsonData["Items"].Count; i++)
        {
            Item newItem = new GameObject().AddComponent<Item>();
            newItem.Name = itemsJsonData["Items"][i]["Name"].ToString();
            newItem.Name = itemsJsonDataDutch["Items"][i]["Name"].ToString();
            newItem.Description = itemsJsonData["Items"][i]["Description"].ToString();
            newItem.Description = itemsJsonDataDutch["Items"][i]["Description"].ToString();
            newItem.Active = itemsJsonData["Items"][i]["Active"].ToString();
            newItem.Active = itemsJsonDataDutch["Items"][i]["Active"].ToString();
            newItem.AssetsImageName = itemsJsonData["Items"][i]["AssetsImageName"].ToString();

            Items.Add(newItem);
            ItemsDutch.Add(newItem);
        }

        File.WriteAllText(_itemsJsonFilePath, itemsData.text);
        File.WriteAllText(_itemsDutchJsonFilePath, itemsDataDutch.text);
    }

    // We use this function to read the existing data from the
    // character's file and set his fields to match the data. Basically
    // we load his stored and previously saved data from the storage
    // into the in-game character.
    private void SetupJsonData()
    {
        if (File.Exists(PlayerStatsFilePath))
        {
            string dataToJson = File.ReadAllText(PlayerStatsFilePath);
            JsonData characterData = JsonMapper.ToObject(dataToJson);

            Name = characterData["Name"].ToString();

            if (characterData["IsDataCreated"].ToString() == "True")
            {
                IsDataCreated = true;
            }
            else if (characterData["IsDataCreated"].ToString() == "False")
            {
                IsDataCreated = false;
            }

            if (characterData["CharacterCreation"].ToString() == "True")
            {
                CharacterCreation = true;
            }
            else if (characterData["CharacterCreation"].ToString() == "False")
            {
                CharacterCreation = false;
            }

            if (characterData["TutorialCompleted"].ToString() == "True")
            {
                TutorialCompleted = true;
            }
            else if (characterData["TutorialCompleted"].ToString() == "False")
            {
                TutorialCompleted = false;
            }

            if (characterData["HasMap"].ToString() == "True")
            {
                HasMap = true;
            }
            else if (characterData["HasMap"].ToString() == "False")
            {
                HasMap = false;
            }

            if (characterData["HasDiary"].ToString() == "True")
            {
                HasDiary = true;
            }
            else if (characterData["HasDiary"].ToString() == "False")
            {
                HasDiary = false;
            }

            if (characterData["AreIconsExplained"].ToString() == "True")
            {
                AreIconsExplained = true;
            }
            else if (characterData["AreIconsExplained"].ToString() == "False")
            {
                AreIconsExplained = false;
            }

            if (characterData["IsCutscenePassed"].ToString() == "True")
            {
                IsCutscenePassed = true;
            }
            else if (characterData["IsCutscenePassed"].ToString() == "False")
            {
                IsCutscenePassed = false;
            }

            #region Blueprints
            for (int i = 1; i < 6; i++)
            {
                bool isBlueprintCollected = false;
                if (characterData["Blueprint" + i].ToString() == "True")
                {
                    isBlueprintCollected = true;
                }
                else if (characterData["Blueprint" + i].ToString() == "False")
                {
                    isBlueprintCollected = false;
                }

                if (i == 1)
                {
                    Blueprint1 = isBlueprintCollected;
                }
                else if (i == 2)
                {
                    Blueprint2 = isBlueprintCollected;
                }
                else if (i == 3)
                {
                    Blueprint3 = isBlueprintCollected;
                }
                else if (i == 4)
                {
                    Blueprint4 = isBlueprintCollected;
                }
                else if (i == 5)
                {
                    Blueprint5 = isBlueprintCollected;
                }
            }
            #endregion

            LastMapArea = characterData["LastMapArea"].ToString();
            LastScene = characterData["LastScene"].ToString();
            Reputation = int.Parse(characterData["Reputation"].ToString());
            Stamina = int.Parse(characterData["Stamina"].ToString());
            Knowledge = int.Parse(characterData["Knowledge"].ToString());
            Fitness = int.Parse(characterData["Fitness"].ToString());
            Charisma = int.Parse(characterData["Charisma"].ToString());
            Currency = int.Parse(characterData["Currency"].ToString());

            AvailableHints = int.Parse(characterData["AvailableHints"].ToString());
            DateOfLastCoffee = characterData["DateOfLastCoffee"].ToString();
        }
    }

    public void SetupQuests()
    {
        // *******************
        // This will load both the currently in progress quests from the player
        // json data and the completed quests into his in-game data storage for in-game
        // manipulation during gameplay
        LoadQuests();

        CurrentQuests.Clear();
        AvailableQuests.Clear();
        foreach (Area area in Areas)
        {
            foreach (Quest questInFile in AllQuests)
            {
                if (questInFile.Area == area.Name && area.Status == Area.AreaStatus.Unlocked)
                {
                    if (questInFile.ProgressStatus == "Ongoing")
                    {
                        CurrentQuests.Add(questInFile);
                    }
                    else if (questInFile.ProgressStatus == "Available" &&
                      questInFile.CompletionStatus)
                    {
                        CompletedQuests.Add(questInFile);
                    }
                    else if (questInFile.ProgressStatus == "Available")
                    {
                        AvailableQuests.Add(questInFile);
                    }
                }
            }
        }
    }

    public void SetupItems()
    {
        string dataToJson = File.ReadAllText(_itemsJsonFilePath);
        JsonData itemData = JsonMapper.ToObject(dataToJson);

        string dataToJsonDutch = File.ReadAllText(_itemsDutchJsonFilePath);
        JsonData itemDataDutch = JsonMapper.ToObject(dataToJsonDutch);

        if (File.Exists(_itemsJsonFilePath) && File.Exists(_itemsDutchJsonFilePath))
        {
            Items.Clear();
            ItemsDutch.Clear();
            for (int i = 0; i < itemData["Items"].Count; i++)
            {
                Item newItem = new GameObject().AddComponent<Item>();
                newItem.Name = itemData["Items"][i]["Name"].ToString();
                newItem.NameDutch = itemDataDutch["Items"][i]["Name"].ToString();
                newItem.Description = itemData["Items"][i]["Description"].ToString();
                newItem.DescriptionDutch = itemDataDutch["Items"][i]["Description"].ToString();
                newItem.Active = itemData["Items"][i]["Active"].ToString();
                newItem.ActiveDutch = itemDataDutch["Items"][i]["Active"].ToString();
                newItem.AssetsImageName = itemData["Items"][i]["AssetsImageName"].ToString();

                Items.Add(newItem);
                ItemsDutch.Add(newItem);
            }
        }
    }

    private void SetupAreas()
    {
        if (File.Exists(_areasJsonFilePath))
        {
            string dataToJson = File.ReadAllText(_areasJsonFilePath);
            JsonData areasData = JsonMapper.ToObject(dataToJson);

            Areas.Clear();
            for (int i = 0; i < areasData["Areas"].Count; i++)
            {
                Area.AreaStatus areaType = Area.AreaStatus.Locked;
                if (areasData["Areas"][i]["Status"].ToString() == "Locked")
                {
                    areaType = Area.AreaStatus.Locked;
                }
                else if (areasData["Areas"][i]["Status"].ToString() == "Unlocked")
                {
                    areaType = Area.AreaStatus.Unlocked;
                }

                Areas.Add(
                    new Area(areasData["Areas"][i]["Name"].ToString(),
                    areaType));
            }
        }
    }

    private void SetupWearables()
    {
        if (File.Exists(_wearablesJsonFilePath) && File.Exists(_wearablesDutchJsonFilePath))
        {
            string dataToJson = File.ReadAllText(_wearablesJsonFilePath);
            JsonData characterData = JsonMapper.ToObject(dataToJson);

            string dataToJsonDutch = File.ReadAllText(_wearablesDutchJsonFilePath);
            JsonData characterDataDutch = JsonMapper.ToObject(dataToJsonDutch);

            Wearables.Clear();
            WearablesDutch.Clear();
            for (int i = 0; i < characterData["Wearables"].Count; i++)
            {
                bool isWearableSelected = false;
                if (characterData["Wearables"][i]["Selected"].ToString() == "True")
                {
                    isWearableSelected = true;
                }
                else if (characterData["Wearables"][i]["Selected"].ToString() == "False")
                {
                    isWearableSelected = false;
                }

                Clothing clothing = new Clothing(
                    isWearableSelected,
                    characterData["Wearables"][i]["BodyPart"].ToString(),
                    characterData["Wearables"][i]["Name"].ToString(),
                    characterDataDutch["Wearables"][i]["Name"].ToString(),
                    characterData["Wearables"][i]["Icon"].ToString(),
                    characterData["Wearables"][i]["PortraitImage"].ToString(),
                    int.Parse(characterData["Wearables"][i]["Stamina"].ToString()),
                    int.Parse(characterData["Wearables"][i]["Knowledge"].ToString()),
                    int.Parse(characterData["Wearables"][i]["Fitness"].ToString()),
                    int.Parse(characterData["Wearables"][i]["Charisma"].ToString()));

                Wearables.Add(clothing);
                WearablesDutch.Add(clothing);
            }
        }
    }

    private void LoadQuests()
    {
        if (File.Exists(_questsJsonFilePath) && File.Exists(_questsDutchJsonFilePath))
        {
            string dataToJson = File.ReadAllText(_questsJsonFilePath);
            JsonData questsData = JsonMapper.ToObject(dataToJson);

            string dataToJsonDutch = File.ReadAllText(_questsDutchJsonFilePath);
            JsonData questsDutchData = JsonMapper.ToObject(dataToJsonDutch);

            AllQuests.Clear();
            for (int i = 0; i < questsData["Quests"].Count; i++)
            {
                List<Objective> newQuestObjectives = new List<Objective>();

                for (int j = 0; j < questsData["Quests"][i]["Objectives"].Count; j++)
                {
                    // These conditions make sure that we get the right type of data
                    // from the json and convert it accurately for the dictionaries
                    // of objectives for that quest later on.
                    bool isObjectiveComplete = false;
                    if (questsData["Quests"][i]["Objectives"][j]["CompletedStatus"].ToString() == "True")
                    {
                        isObjectiveComplete = true;
                    }
                    else if (questsData["Quests"][i]["Objectives"][j]["CompletedStatus"].ToString() == "False")
                    {
                        isObjectiveComplete = false;
                    }

                    // Here we store the new dictionary (objective) to the list of
                    // objectives after we set up the new objective.
                    Objective newObjectives = new Objective(
                        int.Parse(questsData["Quests"][i]["Objectives"][j]["ID"].ToString()),
                        questsData["Quests"][i]["Objectives"][j]["Name"].ToString(), isObjectiveComplete);

                    newQuestObjectives.Add(newObjectives);
                }

                bool statusOfQuestCompletion = false;
                if (questsData["Quests"][i]["Completed"].ToString() == "True")
                {
                    statusOfQuestCompletion = true;
                }
                else if (questsData["Quests"][i]["Completed"].ToString() == "False")
                {
                    statusOfQuestCompletion = false;
                }

                Quest newQuest = new Quest(
                    int.Parse(questsData["Quests"][i]["ID"].ToString()),
                    questsData["Quests"][i]["Name"].ToString(),
                    questsData["Quests"][i]["Area"].ToString(),
                    questsData["Quests"][i]["ProgressStatus"].ToString(),
                    questsData["Quests"][i]["Description"].ToString(),
                    statusOfQuestCompletion,
                    newQuestObjectives);

                AllQuests.Add(newQuest);
            }

            AllQuestsDutch.Clear();
            for (int i = 0; i < questsDutchData["Quests"].Count; i++)
            {
                List<Objective> newQuestObjectives = new List<Objective>();

                for (int j = 0; j < questsDutchData["Quests"][i]["Objectives"].Count; j++)
                {
                    // These conditions make sure that we get the right type of data
                    // from the json and convert it accurately for the dictionaries
                    // of objectives for that quest later on.
                    bool isObjectiveComplete = false;
                    if (questsDutchData["Quests"][i]["Objectives"][j]["CompletedStatus"].ToString() == "True")
                    {
                        isObjectiveComplete = true;
                    }
                    else if (questsDutchData["Quests"][i]["Objectives"][j]["CompletedStatus"].ToString() == "False")
                    {
                        isObjectiveComplete = false;
                    }

                    // Here we store the new dictionary (objective) to the list of
                    // objectives after we set up the new objective.
                    Objective newObjectives = new Objective(
                        int.Parse(questsDutchData["Quests"][i]["Objectives"][j]["ID"].ToString()),
                        questsDutchData["Quests"][i]["Objectives"][j]["Name"].ToString(), isObjectiveComplete);

                    newQuestObjectives.Add(newObjectives);
                }

                bool statusOfQuestCompletion = false;
                if (questsDutchData["Quests"][i]["Completed"].ToString() == "True")
                {
                    statusOfQuestCompletion = true;
                }
                else if (questsDutchData["Quests"][i]["Completed"].ToString() == "False")
                {
                    statusOfQuestCompletion = false;
                }

                Quest newQuest = new Quest(
                    int.Parse(questsDutchData["Quests"][i]["ID"].ToString()),
                    questsDutchData["Quests"][i]["Name"].ToString(),
                    questsDutchData["Quests"][i]["Area"].ToString(),
                    questsDutchData["Quests"][i]["ProgressStatus"].ToString(),
                    questsDutchData["Quests"][i]["Description"].ToString(),
                    statusOfQuestCompletion,
                    newQuestObjectives);

                AllQuestsDutch.Add(newQuest);
            }
        }
    }

    public void LoadInventory()
    {
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                InstantiateItemFormatInInventory(Items);
                break;
            case "Dutch":
                InstantiateItemFormatInInventory(ItemsDutch);
                break;
        }
    }

    private void InstantiateItemFormatInInventory(List<Item> itemsForPanel)
    {
        foreach (Item item in itemsForPanel)
        {
            if (GameObject.FindGameObjectWithTag("Items Panel") != null)
            {
                GameObject newItem;
                newItem = Instantiate(ItemPrefab, GameObject.FindGameObjectWithTag("Items Panel").transform);
                Item newItemScript = newItem.GetComponent<Item>();

                // We use predefined images from the resources folder to load each
                // item's sprites from outside the game and assign it to the new item.
                Sprite sprite = Resources.Load<Sprite>("Items/Inventory/" + item.AssetsImageName);

                switch (SettingsManager.Instance.Language)
                {
                    case "English":
                        newItem.GetComponent<Image>().sprite = sprite;
                        newItemScript.Name = item.Name;
                        newItemScript.Description = item.Description;
                        newItemScript.Active = item.Active;
                        newItemScript.AssetsImageName = item.AssetsImageName;
                        break;
                    case "Dutch":
                        newItem.GetComponent<Image>().sprite = sprite;
                        newItemScript.NameDutch = item.NameDutch;
                        newItemScript.DescriptionDutch = item.DescriptionDutch;
                        newItemScript.ActiveDutch = item.ActiveDutch;
                        newItemScript.AssetsImageName = item.AssetsImageName;
                        break;
                }
            }
        }
    }

    public void SetupWorldData()
    {
        #region Game state setup
        SetupJsonData();
        SetupAreas();
        SetupQuests();
        SetupItems();
        SetupWearables();
        if (SceneManager.GetActiveScene().name == "Main Map")
        {
            InterfaceManager.Instance.LoadBlueprints();
        }
        #endregion
    }
    #endregion

    #region Data manipulation from existing player parameters
    /// <summary>
    /// The functions below are used mainly for adding, removing and updating
    /// elements into existing player lists and  data related to stats and quests.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        Items.Add(item);
        ItemsDutch.Add(item);

        ItemNotificationManager.Instance.GetItem(item);
        RefreshItems();
        LoadInventory();
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        ItemsDutch.Remove(item);

        RefreshItems();
        LoadInventory();
    }

    public void AddWearable(Clothing clothing)
    {
        Wearables.Add(clothing);
        WearablesDutch.Add(clothing);

        ItemNotificationManager.Instance.GetItem(clothing);
        RefreshWearables();
    }

    public void RemoveWearable(Clothing clothing)
    {
        Wearables.Remove(clothing);
        WearablesDutch.Remove(clothing);

        RefreshWearables();
    }

    public void AddAvailableHints(int value)
    {
        AvailableHints += value;
        RefreshJsonData();
    }

    public void RemoveAvailableHints(int value)
    {
        AvailableHints -= value;
        RefreshJsonData();
    }

    public void MoveQuestStageStatus(Quest questToMove, string from, string to)
    {
        if (from == "Current Quests")
        {
            AvailableQuests.Add(questToMove);
            CurrentQuests.Remove(questToMove);
        }
        else if (from == "Available Quests")
        {
            CurrentQuests.Add(questToMove);
            AvailableQuests.Remove(questToMove);
        }
        else if (from == "Completed Quests")
        {
            CurrentQuests.Add(questToMove);
            CompletedQuests.Remove(questToMove);
        }

        string returnOutput = "Quest (" + questToMove.Name + ") status updated to " + to;
        returnOutput += "!";

        RefreshAllQuests();
    }

    public void UpdateAreaStatus(string areaName, string status)
    {
        Area.AreaStatus newAreaStatus;
        if (status == "Locked")
        {
            newAreaStatus = Area.AreaStatus.Locked;
        }
        else if (status == "Unlocked")
        {
            newAreaStatus = Area.AreaStatus.Unlocked;
        }
        else
        {
            newAreaStatus = Area.AreaStatus.Unlocked;
        }

        foreach (Area area in Areas)
        {
            if (area.Name == areaName)
            {
                area.Status = newAreaStatus;
            }
        }

        RefreshAreas();
    }

    public void CollectBlueprint(string blueprint)
    {
        if (blueprint == "Blueprint1")
        {
            Blueprint1 = true;
        }
        else if (blueprint == "Blueprint2")
        {
            Blueprint2 = true;
        }
        else if (blueprint == "Blueprint3")
        {
            Blueprint3 = true;
        }
        else if (blueprint == "Blueprint4")
        {
            Blueprint4 = true;
        }
        else if (blueprint == "Blueprint5")
        {
            Blueprint5 = true;
        }

        RefreshJsonData();
    }

    public void CompleteObjectiveInQuest(int objectiveID, string quest)
    {
        int QuestID = 0;
        bool completedStatus = false;
        bool completionStatusOfQuest = false;

        switch (SettingsManager.Instance.Language)
        {
            case "English":
                foreach (Quest aQuest in AllQuests)
                {
                    if (aQuest.Name == quest && aQuest.ProgressStatus == "Ongoing")
                    {
                        QuestID = aQuest.ID;
                        foreach (Objective aObjective in aQuest.Objectives)
                        {
                            if (aObjective.ID == objectiveID &&
                                aObjective.CompletedStatus == false)
                            {
                                aObjective.CompletedStatus = true;
                                completedStatus = true;

                                bool areAllObjectivesDone = true;
                                foreach (Objective obj in aQuest.Objectives)
                                {
                                    if (obj.CompletedStatus == false)
                                    {
                                        areAllObjectivesDone = false;
                                    }
                                }

                                if (areAllObjectivesDone)
                                {
                                    aQuest.ProgressStatus = "Completed";
                                    aQuest.CompletionStatus = true;
                                    completionStatusOfQuest = true;
                                    if (aQuest.Name == "1672???" || aQuest.Name == "1672???")
                                    {
                                        TutorialCompleted = true;
                                        RefreshJsonData();
                                        CollectBlueprint("Blueprint1");
                                        //FindObjectOfType<MapEnvironmentManager>().LoadObjectsFromSequence();
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (Quest aQuest in AllQuestsDutch)
                {
                    if (aQuest.ID == QuestID)
                    {
                        aQuest.CompletionStatus = completionStatusOfQuest;
                        foreach (Objective aObjective in aQuest.Objectives)
                        {
                            if (aObjective.ID == objectiveID)
                            {
                                aObjective.CompletedStatus = completedStatus;
                            }
                        }
                    }
                }
                break;
            case "Dutch":
                foreach (Quest aQuest in AllQuestsDutch)
                {
                    if (aQuest.Name == quest && aQuest.ProgressStatus == "Nog niet gedaan")
                    {
                        QuestID = aQuest.ID;
                        foreach (Objective aObjective in aQuest.Objectives)
                        {
                            if (aObjective.ID == objectiveID &&
                                aObjective.CompletedStatus == false)
                            {
                                objectiveID = aObjective.ID;
                                aObjective.CompletedStatus = true;
                                completedStatus = true;

                                bool areAllObjectivesDone = true;
                                foreach (Objective obj in aQuest.Objectives)
                                {
                                    if (obj.CompletedStatus == false)
                                    {
                                        areAllObjectivesDone = false;
                                    }
                                }

                                if (areAllObjectivesDone)
                                {
                                    aQuest.ProgressStatus = "Gedaan";
                                    aQuest.CompletionStatus = true;
                                    completionStatusOfQuest = true;
                                    if (aQuest.Name == "1672???" || aQuest.Name == "1672???")
                                    {
                                        TutorialCompleted = true;
                                        RefreshJsonData();
                                        CollectBlueprint("Blueprint1");
                                        MapEnvironmentManager mapEnvironmentManager = FindObjectOfType<MapEnvironmentManager>();
                                        mapEnvironmentManager?.LoadObjectsFromSequence();
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (Quest aQuest in AllQuests)
                {
                    if (aQuest.ID == QuestID)
                    {
                        aQuest.CompletionStatus = completionStatusOfQuest;
                        foreach (Objective aObjective in aQuest.Objectives)
                        {
                            if (aObjective.ID == objectiveID)
                            {
                                aObjective.CompletedStatus = completedStatus;
                            }
                        }
                    }
                }
                break;
        }

        RefreshAllQuests();
        RefreshJsonData();
    }

    /// <summary>
    /// Updates stats starting from left to right using the new stat values in the same order. You can use it the following way: "charisma fitness", new int[] {2, 5}. The charisma value will be incremented by two while the fitness by five. Dont forget to use small letters for the stat strings.
    /// </summary>
    /// <param name="stats"></param>
    /// <param name="statValues"></param>
    public void UpdateStats(string stats, params int[] statValues)
    {
        string[] splitStats = stats.Split(' ');

        for (int i = 0; i < splitStats.Length; i++)
        {
            switch (splitStats[i])
            {
                case "reputation":
                    Reputation += statValues[i];
                    break;
                case "stamina":
                    Stamina += statValues[i];
                    break;
                case "knowledge":
                    Knowledge += statValues[i];
                    break;
                case "fitness":
                    Fitness += statValues[i];
                    break;
                case "charisma":
                    Charisma += statValues[i];
                    break;
                case "currency":
                    Currency += statValues[i];
                    break;
            }
        }

        RefreshJsonData();
    }

    /// <summary>
    /// This function recounts the effects of every clothing for the player
    /// and must be executed every time you add a new clothing or one of their stats is changed.
    /// </summary>
    private void ApplyClothingStats()
    {
        for (int i = 0; i < Wearables.Count; i++)
        {
            Stamina += Wearables[i].Stamina;
            Knowledge += Wearables[i].Knowledge;
            Fitness += Wearables[i].Fitness;
            Charisma += Wearables[i].Charisma;
        }

        RefreshJsonData();
    }
    #endregion

    #region Refreshing functions that export the existing player data to storage
    public void RefreshAllQuests()
    {
        File.WriteAllText(_questsJsonFilePath, "");
        File.WriteAllText(_questsDutchJsonFilePath, "");
        // Reset the existing quests data string
        _newQuestsData = "{";
        _newQuestsData += _newLine;
        _newQuestsData += InsertTabs(1) + "\"Quests\": [";

        if (AllQuests.Count == 0)
        {
            _newQuestsData += "]" + _newLine + "}";
            return;
        }
        else
        {
            for (int i = 0; i < AllQuests.Count; i++)
            {
                _newQuestsData += InsertNewLineTabs(2);
                _newQuestsData += "{";
                _newQuestsData += InsertNewLineTabs(3);
                // This is one field of the quest object
                _newQuestsData += "\"ID\": " + AllQuests[i].ID + ",";
                _newQuestsData += InsertNewLineTabs(3);
                _newQuestsData += "\"Name\": \"" + AllQuests[i].Name + "\",";
                _newQuestsData += InsertNewLineTabs(3);
                _newQuestsData += "\"Area\": \"" + AllQuests[i].Area + "\",";
                _newQuestsData += InsertNewLineTabs(3);
                _newQuestsData += "\"ProgressStatus\": \"" + AllQuests[i].ProgressStatus + "\",";
                _newQuestsData += InsertNewLineTabs(3);
                _newQuestsData += "\"Description\": \"" + AllQuests[i].Description + "\",";

                _newQuestsData += InsertNewLineTabs(3);
                if (AllQuests[i].CompletionStatus == true)
                {
                    _newQuestsData += "\"Completed\": true,";
                }
                else if (AllQuests[i].CompletionStatus == false)
                {
                    _newQuestsData += "\"Completed\": false,";
                }

                _newQuestsData += InsertNewLineTabs(3);
                _newQuestsData += "\"Objectives\": [";
                foreach (Objective objective in AllQuests[i].Objectives)
                {
                    _newQuestsData += InsertNewLineTabs(4);
                    _newQuestsData += "{";
                    _newQuestsData += InsertNewLineTabs(5);
                    _newQuestsData += "\"ID\": " + objective.ID + ",";
                    _newQuestsData += InsertNewLineTabs(5);
                    _newQuestsData += "\"Name\": \"" + objective.Name + "\",";

                    _newQuestsData += InsertNewLineTabs(5);
                    if (objective.CompletedStatus == true)
                    {
                        _newQuestsData += "\"CompletedStatus\": true";
                    }
                    else if (objective.CompletedStatus == false)
                    {
                        _newQuestsData += "\"CompletedStatus\": false";
                    }
                    _newQuestsData += InsertNewLineTabs(4);
                    _newQuestsData += "},";
                }
                // This closes the x type of quests
                _newQuestsData = _newQuestsData.Substring(0, _newQuestsData.Length - 1) + _newLine;

                _newQuestsData += InsertTabs(3) + "]" + _newLine + InsertTabs(2) + "},";
            }
        }

        _newQuestsData = _newQuestsData.Substring(0, _newQuestsData.Length - 1) + _newLine;
        _newQuestsData += InsertTabs(1) + "]" + _newLine + "}";
        File.WriteAllText(_questsJsonFilePath, _newQuestsData);

        _newQuestsDutchData = "{";
        _newQuestsDutchData += _newLine;
        _newQuestsDutchData += InsertTabs(1) + "\"Quests\": [";

        if (AllQuestsDutch.Count == 0)
        {
            _newQuestsDutchData += "]" + _newLine + "}";
            return;
        }
        else
        {
            for (int i = 0; i < AllQuestsDutch.Count; i++)
            {
                _newQuestsDutchData += InsertNewLineTabs(2);
                _newQuestsDutchData += "{";
                _newQuestsDutchData += InsertNewLineTabs(3);
                // This is one field of the quest object
                _newQuestsDutchData += "\"ID\": " + AllQuestsDutch[i].ID + ",";
                _newQuestsDutchData += InsertNewLineTabs(3);
                _newQuestsDutchData += "\"Name\": \"" + AllQuestsDutch[i].Name + "\",";
                _newQuestsDutchData += InsertNewLineTabs(3);
                _newQuestsDutchData += "\"Area\": \"" + AllQuestsDutch[i].Area + "\",";
                _newQuestsDutchData += InsertNewLineTabs(3);
                _newQuestsDutchData += "\"ProgressStatus\": \"" + AllQuestsDutch[i].ProgressStatus + "\",";
                _newQuestsDutchData += InsertNewLineTabs(3);
                _newQuestsDutchData += "\"Description\": \"" + AllQuestsDutch[i].Description + "\",";

                _newQuestsDutchData += InsertNewLineTabs(3);
                if (AllQuestsDutch[i].CompletionStatus == true)
                {
                    _newQuestsDutchData += "\"Completed\": true,";
                }
                else if (AllQuestsDutch[i].CompletionStatus == false)
                {
                    _newQuestsDutchData += "\"Completed\": false,";
                }

                _newQuestsDutchData += InsertNewLineTabs(3);
                _newQuestsDutchData += "\"Objectives\": " + "[";
                foreach (Objective objective in AllQuestsDutch[i].Objectives)
                {
                    _newQuestsDutchData += InsertNewLineTabs(4);
                    _newQuestsDutchData += "{";
                    _newQuestsDutchData += InsertNewLineTabs(5);
                    _newQuestsDutchData += "\"ID\": " + objective.ID + ",";
                    _newQuestsDutchData += InsertNewLineTabs(5);
                    _newQuestsDutchData += "\"Name\": \"" + objective.Name + "\",";

                    _newQuestsDutchData += InsertNewLineTabs(5);
                    if (objective.CompletedStatus == true)
                    {
                        _newQuestsDutchData += "\"CompletedStatus\": true";
                    }
                    else if (objective.CompletedStatus == false)
                    {
                        _newQuestsDutchData += "\"CompletedStatus\": false";
                    }
                    _newQuestsDutchData += InsertNewLineTabs(4);
                    _newQuestsDutchData += "},";
                }
                // This closes the x type of quests
                _newQuestsDutchData = _newQuestsDutchData.Substring(0, _newQuestsDutchData.Length - 1);

                _newQuestsDutchData += InsertNewLineTabs(3) + "]" + _newLine + InsertTabs(2) + "},";
            }
        }
        _newQuestsDutchData = _newQuestsDutchData.Substring(0, _newQuestsDutchData.Length - 1) + _newLine;
        // This closes the wrapper of the json file made from the beginning.
        _newQuestsDutchData += InsertTabs(1) + "]" + _newLine + "}";
        File.WriteAllText(_questsDutchJsonFilePath, _newQuestsDutchData);
    }

    public void RefreshJsonData()
    {
        // We use the jsonMapper instead of the JsonUtility because the latter ruins
        // the objects in the items and clothing arrays for the json file.
        string newPlayerData = JsonUtility.ToJson(this);
        File.WriteAllText(PlayerStatsFilePath, newPlayerData.ToString());
    }

    public void RemoveSingletonInstance()
    {
        Destroy(Instance);
        Destroy(gameObject);
    }

    public void RefreshItems()
    {
        // *************************************
        // ENGLISH VERSION to refresh
        // *************************************
        // We reset the existing items json list content, so that we can
        // append new one afterwards.
        File.WriteAllText(_itemsJsonFilePath, "");
        string dataToJson = File.ReadAllText(_itemsJsonFilePath);
        JsonData itemData = JsonMapper.ToObject(dataToJson);

        // This creates the starting wrapper of the json file.
        string newItemsData = "{";
        newItemsData += _newLine;
        newItemsData += InsertTabs(1) + "\"Items\": [";

        foreach (Item item in Items)
        {
            newItemsData += InsertNewLineTabs(2);
            newItemsData += "{";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Name\": \"" + item.Name + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Description\": \"" + item.Description + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Active\": \"" + item.Active + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"AssetsImageName\": \"" + item.AssetsImageName + "\"";
            newItemsData += InsertNewLineTabs(2);
            newItemsData += "},";
        }

        //if (Items.Count > 0)
        //{
        // This removes the last comma at the last item in the array, so
        // that we wont get an error when getting the data later on.
        if (Items.Count > 0)
        {
            newItemsData = newItemsData.Substring(0, newItemsData.Length - 1) + _newLine;
        }
        //}
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += InsertTabs(1) + "]" + _newLine + "}";
        File.WriteAllText(_itemsJsonFilePath, newItemsData);

        // *************************************
        // DUTCH VERSION to refresh
        // *************************************
        File.WriteAllText(_itemsDutchJsonFilePath, "");
        string dataToJsonDutch = File.ReadAllText(_itemsDutchJsonFilePath);
        JsonData itemDataDutch = JsonMapper.ToObject(dataToJsonDutch);

        // This creates the starting wrapper of the json file.
        newItemsData = "{";
        newItemsData += _newLine;
        newItemsData += InsertTabs(1) + "\"Items\": [";

        foreach (Item item in ItemsDutch)
        {
            newItemsData += InsertNewLineTabs(2);
            newItemsData += "{";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Name\": \"" + item.NameDutch + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Description\": \"" + item.DescriptionDutch + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Active\": \"" + item.ActiveDutch + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"AssetsImageName\": \"" + item.AssetsImageName + "\"";
            newItemsData += InsertNewLineTabs(2);
            newItemsData += "},";
        }

        //if (ItemsDutch.Count > 0)
        //{
        // This removes the last comma at the last item in the array, so
        // that we wont get an error when getting the data later on.
        if (ItemsDutch.Count > 0)
        {
            newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        }
        //}
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += InsertNewLineTabs(1) + "]" + _newLine + "}";
        File.WriteAllText(_itemsDutchJsonFilePath, newItemsData);
    }

    public void RefreshWearables()
    {
        // *************************************
        // ENGLISH VERSION to refresh
        // *************************************
        // We reset the existing items json list content, so that we can
        // append new one afterwards.
        File.WriteAllText(_wearablesJsonFilePath, "");

        // This creates the starting wrapper of the json file.
        string newItemsData = "{";
        newItemsData += InsertNewLineTabs(1);
        newItemsData += "\"Wearables\": [";

        foreach (Clothing cloth in Wearables)
        {
            newItemsData += InsertNewLineTabs(2);
            newItemsData += "{";
            newItemsData += InsertNewLineTabs(3);
            if (cloth.Selected)
            {
                newItemsData += "\"Selected\": true,";
            }
            else
            {
                newItemsData += "\"Selected\": false,";
            }
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"BodyPart\": \"" + cloth.BodyPart + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Name\": \"" + cloth.Name + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Icon\": \"" + cloth.Icon + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"PortraitImage\": \"" + cloth.PortraitImage + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Stamina\": " + cloth.Stamina + ",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Knowledge\": " + cloth.Knowledge + ",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Fitness\": " + cloth.Fitness + ",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Charisma\": " + cloth.Charisma;
            newItemsData += InsertNewLineTabs(2);
            newItemsData += "},";
        }

        if (Wearables.Count > 0)
        {
            // This removes the last comma at the last item in the array, so
            // that we wont get an error when getting the data later on.
            newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        }
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += InsertNewLineTabs(1) + "]" + _newLine + "}";
        File.WriteAllText(_wearablesJsonFilePath, newItemsData);

        // *************************************
        // DUTCH VERSION to refresh
        // *************************************
        File.WriteAllText(_wearablesDutchJsonFilePath, "");

        // This creates the starting wrapper of the json file.
        newItemsData = "{";
        newItemsData += InsertNewLineTabs(1);
        newItemsData += "\"Wearables\": [";

        foreach (Clothing cloth in WearablesDutch)
        {
            newItemsData += InsertNewLineTabs(2);
            newItemsData += "{";
            newItemsData += InsertNewLineTabs(3);
            if (cloth.Selected)
            {
                newItemsData += "\"Selected\": true,";
            }
            else
            {
                newItemsData += "\"Selected\": false,";
            }
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"BodyPart\": \"" + cloth.BodyPart + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Name\": \"" + cloth.NameDutch + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Icon\": \"" + cloth.Icon + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"PortraitImage\": \"" + cloth.PortraitImage + "\",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Stamina\": " + cloth.Stamina + ",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Knowledge\": " + cloth.Knowledge + ",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Fitness\": " + cloth.Fitness + ",";
            newItemsData += InsertNewLineTabs(3);
            newItemsData += "\"Charisma\": " + cloth.Charisma;
            newItemsData += InsertNewLineTabs(2);
            newItemsData += "},";
        }

        if (WearablesDutch.Count > 0)
        {
            // This removes the last comma at the last item in the array, so
            // that we wont get an error when getting the data later on.
            newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        }
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += InsertNewLineTabs(1) + "]" + _newLine + "}";
        File.WriteAllText(_wearablesDutchJsonFilePath, newItemsData);
    }

    public void ReloadInventory()
    {
        GameObject itemsPanel = GameObject.FindGameObjectWithTag("Items Panel");
        if (itemsPanel != null)
        {
            for (int i = 0; i < itemsPanel.transform.childCount; i++)
            {
                Destroy(GameObject.FindGameObjectWithTag("Items Panel").transform.GetChild(i).gameObject);
            }

            LoadInventory();
        }
    }

    public void RefreshAreas()
    {
        File.WriteAllText(_areasJsonFilePath, "");

        // This creates the starting wrapper of the json file.
        string newAreasData = "{";
        newAreasData += _newLine;
        newAreasData += InsertTabs(1) + "\"Areas\": [";

        foreach (Area area in Areas)
        {
            newAreasData += InsertNewLineTabs(2);
            newAreasData += "{";
            newAreasData += InsertNewLineTabs(3);
            newAreasData += "\"Name\": \"" + area.Name + "\",";
            newAreasData += InsertNewLineTabs(3);
            newAreasData += "\"Status\": \"" + area.Status.ToString() + "\"";
            newAreasData += InsertNewLineTabs(2);
            newAreasData += "},";
        }

        if (Areas.Count > 0)
        {
            // This removes the last comma at the last item in the array, so
            // that we wont get an error when getting the data later on.
            newAreasData = newAreasData.Substring(0, newAreasData.Length - 1);
        }
        // This closes the wrapper of the json file made from the beginning.
        newAreasData += InsertNewLineTabs(1) + "]" + _newLine + "}";
        File.WriteAllText(_areasJsonFilePath, newAreasData);
    }
    #endregion

    public void InitiateInteraction()
    {
        StartCoroutine(EnablePlayerInteraction());
        StartCoroutine(EnableEntityTapping());
    }

    public IEnumerator EnablePlayerInteraction()
    {
        yield return new WaitForSeconds(0.2f);

        CameraBehavior.IsInteracting = false;
    }

    public IEnumerator EnableEntityTapping()
    {
        yield return new WaitForSeconds(0.2f);

        CameraBehavior.IsUIOpen = false;
        CameraBehavior.IsEntityTappedOn = false;
    }

    public IEnumerator EnableCameraInteraction()
    {
        yield return new WaitForSeconds(0.2f);

        CameraBehavior.IsInterfaceElementSelected = false;
    }

    private string InsertTabs(int numberOfTabs)
    {
        string whiteSpace = string.Empty;
        for (int i = 0; i < numberOfTabs; i++)
        {
            whiteSpace += "\t";
        }

        return whiteSpace;
    }
    private string InsertNewLineTabs(int numberOfTabs)
    {
        string whiteSpace = _newLine;
        for (int i = 0; i < numberOfTabs; i++)
        {
            whiteSpace += "\t";
        }

        return whiteSpace;
    }
}