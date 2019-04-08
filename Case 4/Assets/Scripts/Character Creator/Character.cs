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
    private string _defaultsGuessingClothesJsonPath;
    private string _guessingPuzzlesPath;
    private string _escapeGamesJsonFile;
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

    private void Awake()
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
            _defaultsGuessingClothesJsonPath = _pathToAssetsFolder + "/GuessClothingDefaults.json";
            _guessingPuzzlesPath = _pathToAssetsFolder + "/GuessingPuzzles.json";
            _escapeGamesJsonFile = _pathToAssetsFolder + "/EscapeGames.json";

            if (File.Exists(PlayerStatsFilePath))
            {
                //Debug.Log("data is retrieved");

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
                    //Debug.Log("data is created");

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
                    File.WriteAllText(_guessingPuzzlesPath,             puzzleData.text);
                    #endregion

                    #region Creating the default escape games data list
                    TextAsset escapeGamesData = Resources.Load<TextAsset>("Default World Data/EscapeGames");
                    File.WriteAllText(_escapeGamesJsonFile, escapeGamesData.text);
                    #endregion
                }
            }
        }
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
            Item newItem = new Item(
                itemsJsonData["Items"][i]["Name"].ToString(),
                itemsJsonDataDutch["Items"][i]["Name"].ToString(),
                itemsJsonData["Items"][i]["Description"].ToString(),
                itemsJsonDataDutch["Items"][i]["Description"].ToString(),
                itemsJsonData["Items"][i]["Active"].ToString(),
                itemsJsonDataDutch["Items"][i]["Active"].ToString(),
                itemsJsonData["Items"][i]["AssetsImageName"].ToString());

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
                } else if (characterData["Blueprint" + i].ToString() == "False")
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

        //Debug.Log("Loaded character json data!");
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
            //Debug.Log(area.Name);
            foreach (Quest questInFile in AllQuests)
            {
                if (questInFile.Area == area.Name && area.Status == Area.AreaStatus.Unlocked)
                {
                    if (questInFile.ProgressStatus == "Ongoing")
                    {
                        CurrentQuests.Add(questInFile);
                        //Debug.Log("Ongoing " + questInFile.Name);
                    } else if (questInFile.ProgressStatus == "Available" &&
                        questInFile.CompletionStatus)
                    {
                        CompletedQuests.Add(questInFile);
                    } else if (questInFile.ProgressStatus == "Available")
                    {
                        AvailableQuests.Add(questInFile);
                        //Debug.Log("Available " + questInFile.Name);
                    }
                }
            }
        }
    }

    private void SetupItems()
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
                Item newItem = new Item(
                    itemData["Items"][i]["Name"].ToString(),
                    itemDataDutch["Items"][i]["Name"].ToString(),
                    itemData["Items"][i]["Description"].ToString(),
                    itemDataDutch["Items"][i]["Description"].ToString(),
                    itemData["Items"][i]["Active"].ToString(),
                    itemDataDutch["Items"][i]["Active"].ToString(),
                    itemData["Items"][i]["AssetsImageName"].ToString());

                Items.Add(newItem);
                ItemsDutch.Add(newItem);
            }
        }
        //LoadInventory();
        //Debug.Log("Loaded items json data!");
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
                } else if (areasData["Areas"][i]["Status"].ToString() == "Unlocked")
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
        //Debug.Log("Loaded wearables json data!");
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
                } else if (questsData["Quests"][i]["Completed"].ToString() == "False")
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

        //Debug.Log("Loaded " + quests + " json data!");
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

        //UpdateItemIDs(Items);
        //UpdateItemIDs(ItemsDutch);

        RefreshItems();
        LoadInventory();

        //Debug.Log(string.Format("Added {0} to current items!", item.Name));
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        ItemsDutch.Remove(item);

        //UpdateItemIDs(Items);
        //UpdateItemIDs(ItemsDutch);

        RefreshItems();
        LoadInventory();

        //Debug.Log(string.Format("Removed {0} from current items!", item.Name));
    }

    //public void UpdateItemIDs(List<Item> listForItems)
    //{
    //    for (int i = 0; i < listForItems.Count; i++)
    //    {
    //        listForItems[i].ID = i;
    //    }
    //}

    public void AddWearable(Clothing clothing)
    {
        Wearables.Add(clothing);
        WearablesDutch.Add(clothing);
        
        RefreshWearables();

        //Debug.Log(string.Format("Added {0} to current wearables!", clothing.Name));
    }

    public void RemoveWearable(Clothing clothing)
    {
        Wearables.Remove(clothing);
        WearablesDutch.Remove(clothing);

        RefreshWearables();

        //Debug.Log(string.Format("Removed {0} from current wearables!", clothing.Name));
    }

    public void AddAvailableHints(int value)
    {
        AvailableHints += value;
        RefreshJsonData();

        //Debug.Log("Added a hint. Total hints left: " + AvailableHints);
    }

    public void RemoveAvailableHints(int value)
    {
        AvailableHints -= value;
        RefreshJsonData();

        //Debug.Log("Removed a hint. Total hints left: " + AvailableHints);
    }

    public void MoveQuestStageStatus(Quest questToMove, string from, string to)
    {
        if (from == "Current Quests")
        {
            AvailableQuests.Add(questToMove);
            CurrentQuests.Remove(questToMove);
        } else if (from == "Available Quests")
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
        //Debug.Log(returnOutput);
    }

    public void CollectBlueprint(string blueprint)
    {
        if (blueprint == "Blueprint1")
        {
            Blueprint1 = true;
        } else if (blueprint == "Blueprint2")
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

        //Debug.Log("Collected the blueprint: " + blueprint);
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
                                        //Debug.Log(aQuest.Name);
                                        TutorialCompleted = true;
                                        RefreshJsonData();
                                        CollectBlueprint("Blueprint1");
                                        FindObjectOfType<MapEnvironmentManager>().LoadObjectsFromSequence();
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
                //Debug.Log("OOOO");
                foreach (Quest aQuest in AllQuestsDutch)
                {
                    //Debug.Log(aQuest.Name + " | " + quest);
                    if (aQuest.Name == quest && aQuest.ProgressStatus == "Nog niet gedaan")
                    {
                        //Debug.Log(QuestID + " | " + aQuest.ID);
                        QuestID = aQuest.ID;
                        foreach (Objective aObjective in aQuest.Objectives)
                        {
                            //Debug.Log(aObjective.Name + " | " + objectiveID);
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
                                        //Debug.Log(aQuest.Name);
                                        TutorialCompleted = true;
                                        RefreshJsonData();
                                        CollectBlueprint("Blueprint1");
                                        FindObjectOfType<MapEnvironmentManager>().LoadObjectsFromSequence();
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

    //public void CompleteObjective(string objective)
    //{
    //    int QuestID = 0;
    //    int ObjectiveID = 0;
    //    bool completedStatus = false;
    //    bool completionStatusOfQuest = false;

    //    switch (SettingsManager.Instance.Language)
    //    {
    //        case "English":
    //            foreach (Quest aQuest in AllQuests)
    //            {
    //                if (aQuest.ProgressStatus == "Ongoing")
    //                {
    //                    foreach (Objective aObjective in aQuest.Objectives)
    //                    {
    //                        //Debug.Log(aObjective.Name + " | " + objective);
    //                        if (aObjective.Name == objective &&
    //                            aObjective.CompletedStatus == false)
    //                        {
    //                            aObjective.CompletedStatus = true;
    //                            completedStatus = true;
    //                            QuestID = aQuest.ID;
    //                            ObjectiveID = aObjective.ID;

    //                            bool areAllObjectivesDone = true;
    //                            foreach (Objective obj in aQuest.Objectives)
    //                            {
    //                                if (obj.CompletedStatus == false)
    //                                {
    //                                    areAllObjectivesDone = false;
    //                                }
    //                            }

    //                            if (areAllObjectivesDone)
    //                            {
    //                                aQuest.ProgressStatus = "Completed";
    //                                aQuest.CompletionStatus = true;
    //                                aQuest.CompletionStatus = true;
    //                                if (aQuest.Name == "Tutorial" || aQuest.Name == "Trenirovka")
    //                                {
    //                                    //Debug.Log(aQuest.Name);
    //                                    TutorialCompleted = true;
    //                                    RefreshJsonData();
    //                                    CollectBlueprint("Blueprint1");
    //                                    FindObjectOfType<MapEnvironmentManager>().LoadObjectsFromSequence();
    //                                }
    //                            }
    //                            RefreshAllQuests();
    //                        }
    //                    }
    //                }
    //            }

    //            foreach (Quest aQuest in AllQuestsDutch)
    //            {
    //                if (aQuest.ID == QuestID)
    //                {
    //                    aQuest.CompletionStatus = completionStatusOfQuest;
    //                    foreach (Objective aObjective in aQuest.Objectives)
    //                    {
    //                        if (aObjective.ID == ObjectiveID)
    //                        {
    //                            aObjective.CompletedStatus = completedStatus;
    //                        }
    //                    }
    //                }
    //            }
    //            break;
    //        case "Dutch":
    //            foreach (Quest aQuest in AllQuestsDutch)
    //            {
    //                if (aQuest.ProgressStatus == "Nedovurshen")
    //                {
    //                    foreach (Objective aObjective in aQuest.Objectives)
    //                    {
    //                        //Debug.Log(aObjective.Name + " | " + objective);
    //                        if (aObjective.Name == objective &&
    //                            aObjective.CompletedStatus == false)
    //                        {
    //                            aObjective.CompletedStatus = true;
    //                            completedStatus = true;
    //                            QuestID = aQuest.ID;
    //                            ObjectiveID = aObjective.ID;

    //                            bool areAllObjectivesDone = true;
    //                            foreach (Objective obj in aQuest.Objectives)
    //                            {
    //                                if (obj.CompletedStatus == false)
    //                                {
    //                                    areAllObjectivesDone = false;
    //                                }
    //                            }

    //                            if (areAllObjectivesDone)
    //                            {
    //                                aQuest.ProgressStatus = "Zavurshen";
    //                                aQuest.CompletionStatus = true;
    //                                aQuest.CompletionStatus = true;
    //                                if (aQuest.Name == "Tutorial" || aQuest.Name == "Trenirovka")
    //                                {
    //                                    //Debug.Log(aQuest.Name);
    //                                    TutorialCompleted = true;
    //                                    RefreshJsonData();
    //                                    CollectBlueprint("Blueprint1");
    //                                    FindObjectOfType<MapEnvironmentManager>().LoadObjectsFromSequence();
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            foreach (Quest aQuest in AllQuests)
    //            {
    //                if (aQuest.ID == QuestID)
    //                {
    //                    aQuest.CompletionStatus = completionStatusOfQuest;
    //                    foreach (Objective aObjective in aQuest.Objectives)
    //                    {
    //                        if (aObjective.ID == ObjectiveID)
    //                        {
    //                            aObjective.CompletedStatus = completedStatus;
    //                        }
    //                    }
    //                }
    //            }
    //            break;
    //    }

    //    RefreshAllQuests();
    //}

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
                    //Debug.Log("Increased reputation by " + statValues[i] + "!");
                    break;
                case "stamina":
                    Stamina += statValues[i];
                    //Debug.Log("Increased Stamina by " + statValues[i] + "!");
                    break;
                case "knowledge":
                    Knowledge += statValues[i];
                    //Debug.Log("Increased Knowledge by " + statValues[i] + "!");
                    break;
                case "fitness":
                    Fitness += statValues[i];
                    //Debug.Log("Increased Fitness by " + statValues[i] + "!");
                    break;
                case "charisma":
                    Charisma += statValues[i];
                    //Debug.Log("Increased charisma by " + statValues[i] + "!");
                    break;
                case "currency":
                    Currency += statValues[i];
                    //1("Increased currency by " + statValues[i] + "!");
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
        _newQuestsData += "\"Quests\":[";

        if (AllQuests.Count == 0)
        {
            _newQuestsData += "]}";
            return;
        }
        else
        {
            for (int i = 0; i < AllQuests.Count; i++)
            {
                _newQuestsData += "{";
                // This is one field of the quest object
                _newQuestsData += "\"ID\":" + AllQuests[i].ID + ",";
                _newQuestsData += "\"Name\":\"" + AllQuests[i].Name + "\",";
                _newQuestsData += "\"Area\":\"" + AllQuests[i].Area + "\",";
                _newQuestsData += "\"ProgressStatus\":\"" + AllQuests[i].ProgressStatus + "\",";
                _newQuestsData += "\"Description\":\"" + AllQuests[i].Description + "\",";

                if (AllQuests[i].CompletionStatus == true)
                {
                    _newQuestsData += "\"Completed\":true,";
                }
                else if (AllQuests[i].CompletionStatus == false)
                {
                    _newQuestsData += "\"Completed\":false,";
                }

                _newQuestsData += "\"Objectives\":" + "[";
                foreach (Objective objective in AllQuests[i].Objectives)
                {
                    _newQuestsData += "{";
                    _newQuestsData += "\"ID\":" + objective.ID + ",";
                    _newQuestsData += "\"Name\":\"" + objective.Name + "\",";
                    if (objective.CompletedStatus == true)
                    {
                        _newQuestsData += "\"CompletedStatus\":true";
                    }
                    else if (objective.CompletedStatus == false)
                    {
                        _newQuestsData += "\"CompletedStatus\":false";
                    }
                    _newQuestsData += "},";
                }
                // This closes the x type of quests
                _newQuestsData = _newQuestsData.Substring(0, _newQuestsData.Length - 1);

                _newQuestsData += "]},";
            }
        }

        _newQuestsData = _newQuestsData.Substring(0, _newQuestsData.Length - 1);
        _newQuestsData += "]}";
        File.WriteAllText(_questsJsonFilePath, _newQuestsData);

        _newQuestsDutchData = "{";
        _newQuestsDutchData += "\"Quests\":[";

        if (AllQuestsDutch.Count == 0)
        {
            _newQuestsDutchData += "]}";
            return;
        }
        else
        {
            for (int i = 0; i < AllQuestsDutch.Count; i++)
            {
                _newQuestsDutchData += "{";
                // This is one field of the quest object
                _newQuestsDutchData += "\"ID\":" + AllQuestsDutch[i].ID + ",";
                _newQuestsDutchData += "\"Name\":\"" + AllQuestsDutch[i].Name + "\",";
                _newQuestsDutchData += "\"Area\":\"" + AllQuestsDutch[i].Area + "\",";
                _newQuestsDutchData += "\"ProgressStatus\":\"" + AllQuestsDutch[i].ProgressStatus + "\",";
                _newQuestsDutchData += "\"Description\":\"" + AllQuestsDutch[i].Description + "\",";

                if (AllQuestsDutch[i].CompletionStatus == true)
                {
                    _newQuestsDutchData += "\"Completed\":true,";
                }
                else if (AllQuestsDutch[i].CompletionStatus == false)
                {
                    _newQuestsDutchData += "\"Completed\":false,";
                }

                _newQuestsDutchData += "\"Objectives\":" + "[";
                foreach (Objective objective in AllQuestsDutch[i].Objectives)
                {
                    _newQuestsDutchData += "{";
                    _newQuestsDutchData += "\"ID\":" + objective.ID + ",";
                    _newQuestsDutchData += "\"Name\":\"" + objective.Name + "\",";
                    if (objective.CompletedStatus == true)
                    {
                        _newQuestsDutchData += "\"CompletedStatus\":true";
                    }
                    else if (objective.CompletedStatus == false)
                    {
                        _newQuestsDutchData += "\"CompletedStatus\":false";
                    }
                    _newQuestsDutchData += "},";
                }
                // This closes the x type of quests
                _newQuestsDutchData = _newQuestsDutchData.Substring(0, _newQuestsDutchData.Length - 1);

                _newQuestsDutchData += "]},";
            }
        }
        _newQuestsDutchData = _newQuestsDutchData.Substring(0, _newQuestsDutchData.Length - 1);
        // This closes the wrapper of the json file made from the beginning.
        _newQuestsDutchData += "]}";
        File.WriteAllText(_questsDutchJsonFilePath, _newQuestsDutchData);

        //Debug.Log("Refreshed all quests json data!");
    }

    public void RefreshJsonData()
    {
        // We use the jsonMapper instead of the JsonUtility because the latter ruins
        // the objects in the items and clothing arrays for the json file.
        string newPlayerData = JsonUtility.ToJson(this);
        File.WriteAllText(PlayerStatsFilePath, newPlayerData.ToString());

        //Debug.Log("Refreshed player json data!");
    }

    public void DeleteJsonFiles()
    {
        File.Delete(PlayerStatsFilePath);
        File.Delete(_areasJsonFilePath);
        File.Delete(_questsJsonFilePath);
        File.Delete(_questsDutchJsonFilePath);
        File.Delete(_itemsJsonFilePath);
        File.Delete(_itemsDutchJsonFilePath);
        File.Delete(_wearablesJsonFilePath);
        File.Delete(_wearablesDutchJsonFilePath);
        File.Delete(_defaultsGuessingClothesJsonPath);
        File.Delete(_guessingPuzzlesPath);
        File.Delete(_escapeGamesJsonFile);
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

        // This creates the starting wrapper of the json file.
        string newItemsData = "{\"Items\":[";
        
        foreach (Item item in Items)
        {
            newItemsData += "{";
            newItemsData += "\"Name\":\"" + item.Name + "\",";
            newItemsData += "\"Description\":\"" + item.Description + "\",";
            newItemsData += "\"Active\":\"" + item.Active + "\",";
            newItemsData += "\"AssetsImageName\":\"" + item.AssetsImageName + "\"";
            newItemsData += "},";
        }

        if (Items.Count > 0)
        {
            // This removes the last comma at the last item in the array, so
            // that we wont get an error when getting the data later on.
            newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        }
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += "]}";
        File.WriteAllText(_itemsJsonFilePath, newItemsData);

        // *************************************
        // DUTCH VERSION to refresh
        // *************************************
        File.WriteAllText(_itemsDutchJsonFilePath, "");

        // This creates the starting wrapper of the json file.
        newItemsData = "{\"Items\":[";

        foreach (Item item in ItemsDutch)
        {
            newItemsData += "{";
            newItemsData += "\"Name\":\"" + item.NameDutch + "\",";
            newItemsData += "\"Description\":\"" + item.DescriptionDutch + "\",";
            newItemsData += "\"Active\":\"" + item.ActiveDutch + "\",";
            newItemsData += "\"AssetsImageName\":\"" + item.AssetsImageName + "\"";
            newItemsData += "},";
        }

        if (ItemsDutch.Count > 0)
        {
            // This removes the last comma at the last item in the array, so
            // that we wont get an error when getting the data later on.
            newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        }
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += "]}";
        File.WriteAllText(_itemsDutchJsonFilePath, newItemsData);

        //Debug.Log("Refreshed items json data!");
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
        string newItemsData = "{\"Wearables\":[";

        foreach (Clothing cloth in Wearables)
        {
            newItemsData += "{";
            if (cloth.Selected)
            {
                newItemsData += "\"Selected\":true,";
            }
            else
            {
                newItemsData += "\"Selected\":false,";
            }
            newItemsData += "\"BodyPart\":\"" + cloth.BodyPart + "\",";
            newItemsData += "\"Name\":\"" + cloth.Name + "\",";
            newItemsData += "\"Icon\":\"" + cloth.Icon + "\",";
            newItemsData += "\"PortraitImage\":\"" + cloth.PortraitImage + "\",";
            newItemsData += "\"Stamina\":" + cloth.Stamina + ",";
            newItemsData += "\"Knowledge\":" + cloth.Knowledge + ",";
            newItemsData += "\"Fitness\":" + cloth.Fitness + ",";
            newItemsData += "\"Charisma\":" + cloth.Charisma;
            newItemsData += "},";
        }

        if (Wearables.Count > 0)
        {
            // This removes the last comma at the last item in the array, so
            // that we wont get an error when getting the data later on.
            newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        }
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += "]}";
        File.WriteAllText(_wearablesJsonFilePath, newItemsData);

        // *************************************
        // DUTCH VERSION to refresh
        // *************************************
        File.WriteAllText(_wearablesDutchJsonFilePath, "");

        // This creates the starting wrapper of the json file.
        newItemsData = "{\"Wearables\":[";

        foreach (Clothing cloth in WearablesDutch)
        {
            newItemsData += "{";
            if (cloth.Selected)
            {
                newItemsData += "\"Selected\":true,";
            } else
            {
                newItemsData += "\"Selected\":false,";
            }
            newItemsData += "\"BodyPart\":\"" + cloth.BodyPart + "\",";
            newItemsData += "\"Name\":\"" + cloth.NameDutch + "\",";
            newItemsData += "\"Icon\":\"" + cloth.Icon + "\",";
            newItemsData += "\"PortraitImage\":\"" + cloth.PortraitImage + "\",";
            newItemsData += "\"Stamina\":" + cloth.Stamina + ",";
            newItemsData += "\"Knowledge\":" + cloth.Knowledge + ",";
            newItemsData += "\"Fitness\":" + cloth.Fitness + ",";
            newItemsData += "\"Charisma\":" + cloth.Charisma;
            newItemsData += "},";
        }

        if (WearablesDutch.Count > 0)
        {
            // This removes the last comma at the last item in the array, so
            // that we wont get an error when getting the data later on.
            newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        }
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += "]}";
        File.WriteAllText(_wearablesDutchJsonFilePath, newItemsData);
    }

    public void ReloadInventory()
    {
        for (int i = 0; i < GameObject.FindGameObjectWithTag("Items Panel").transform.childCount; i++)
        {
            Destroy(GameObject.FindGameObjectWithTag("Items Panel").transform.GetChild(i).gameObject);
        }

        LoadInventory();
    }
    #endregion

    public void InitiateInteraction()
    {
        StartCoroutine(EnablePlayerInteraction());
        StartCoroutine(EnableEntityTapping());
    }

    public IEnumerator EnablePlayerInteraction()
    {
        yield return new WaitForSeconds(1f);

        CameraBehavior.IsInteracting = false;
    }

    public IEnumerator EnableEntityTapping()
    {
        yield return new WaitForSeconds(1f);

        CameraBehavior.IsUIOpen = false;
    }

    public IEnumerator EnableCameraInteraction()
    {
        yield return new WaitForSeconds(1f);

        CameraBehavior.IsInterfaceElementSelected = false;
    }
}