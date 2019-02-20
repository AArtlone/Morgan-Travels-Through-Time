using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public string Name;
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
    [NonSerialized]
    [Space(10)]
    public List<Item> Items = new List<Item>();
    #endregion
    public List<Case> AvailableCases = new List<Case>();
    public List<Case> CurrentCases = new List<Case>();
    public List<Case> CompletedCases = new List<Case>();
    public List<Worker> Workers = new List<Worker>();
    [Space(10)]
    public string DateOfLastCoffee;
    public bool IsCoffeeAvailable;

    // Location reference of player json file.
    private string _pathToAssetsFolder = Application.streamingAssetsPath;
    private string _playerStatsFilePath;
    private string _casesJsonFilePath;
    private string _itemsJsonFilePath;
    private string _wearablesJsonFilePath;
    private string _newCasesData;
    private string _workersDataJsonFilePath;

    // Inventory-related references
    private GameObject _itemsPanel;
    public GameObject ItemPrefab;

    private void Start()
    {
        _itemsPanel = GameObject.FindGameObjectWithTag("Items Panel");

        _playerStatsFilePath = _pathToAssetsFolder + "/Player.json";
        _casesJsonFilePath = _pathToAssetsFolder + "/Cases.json";
        _itemsJsonFilePath = _pathToAssetsFolder + "/Items.json";
        _wearablesJsonFilePath = _pathToAssetsFolder + "/Wearables.json";
        _workersDataJsonFilePath = _pathToAssetsFolder + "/Workers.json";

        // These functions initialize the game state from the storage.
        SetupJsonData();
        SetupCases();
        SetupItems();
        SetupWearables();
        SetupWorkers();
        LoadInventory();
    }

    /// <summary>
    /// The following functions extract the json data from the storage of
    /// the game and imports it to the character (this) object for in-game
    /// manipulation.
    /// </summary>
    // We use this function to read the existing data from the
    // character's file and set his fields to match the data. Basically
    // we load his stored and previously saved data from the storage
    // into the in-game character.
    private void SetupJsonData()
    {
        if (File.Exists(_playerStatsFilePath))
        {
            string dataToJson = File.ReadAllText(_playerStatsFilePath);
            JsonData characterData = JsonMapper.ToObject(dataToJson);

            Name = characterData["Name"].ToString();
            Reputation = int.Parse(characterData["Reputation"].ToString());
            Stamina = int.Parse(characterData["Stamina"].ToString());
            Knowledge = int.Parse(characterData["Knowledge"].ToString());
            Fitness = int.Parse(characterData["Fitness"].ToString());
            Charisma = int.Parse(characterData["Charisma"].ToString());
            Currency = int.Parse(characterData["Currency"].ToString());

            AvailableHints = int.Parse(characterData["AvailableHints"].ToString());
            DateOfLastCoffee = characterData["DateOfLastCoffee"].ToString();

            if (characterData["IsCoffeeAvailable"].ToString() == "True")
            {
                IsCoffeeAvailable = true;
            }
            else if (characterData["IsCoffeeAvailable"].ToString() == "False")
            {
                IsCoffeeAvailable = false;
            }
        }

        //Debug.Log("Loaded character json data!");
    }

    private void SetupCases()
    {
        // *******************
        // This will load both the currently in progress cases from the player
        // json data and the completed cases into his in-game data storage for in-game
        // manipulation during gameplay
        LoadCases("AvailableCases", AvailableCases);
        LoadCases("CurrentCases", CurrentCases);
        LoadCases("CompletedCases", CompletedCases);
    }

    private void SetupItems()
    {
        if (File.Exists(_itemsJsonFilePath))
        {
            string dataToJson = File.ReadAllText(_itemsJsonFilePath);
            JsonData characterData = JsonMapper.ToObject(dataToJson);

            Items.Clear();
            for (int i = 0; i < characterData["Items"].Count; i++)
            {
                Items.Add(new Item(
                    characterData["Items"][i]["Name"].ToString(),
                    characterData["Items"][i]["Description"].ToString(),
                    characterData["Items"][i]["Active"].ToString(),
                    characterData["Items"][i]["AssetsImageName"].ToString()));
            }
        }

        //Debug.Log("Loaded items json data!");
    }

    private void SetupWearables()
    {
        if (File.Exists(_wearablesJsonFilePath))
        {
            string dataToJson = File.ReadAllText(_wearablesJsonFilePath);
            JsonData characterData = JsonMapper.ToObject(dataToJson);

            Wearables.Clear();
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

                Wearables.Add(new Clothing(
                    isWearableSelected,
                    characterData["Wearables"][i]["BodyPart"].ToString(),
                    characterData["Wearables"][i]["Name"].ToString(),
                    characterData["Wearables"][i]["Icon"].ToString(),
                    characterData["Wearables"][i]["PortraitImage"].ToString(),
                    int.Parse(characterData["Wearables"][i]["Stamina"].ToString()),
                    int.Parse(characterData["Wearables"][i]["Knowledge"].ToString()),
                    int.Parse(characterData["Wearables"][i]["Fitness"].ToString()),
                    int.Parse(characterData["Wearables"][i]["Charisma"].ToString())));
            }
        }

        //Debug.Log("Loaded wearables json data!");
    }

    private void SetupWorkers()
    {
        if (File.Exists(_workersDataJsonFilePath))
        {
            string dataToJson = File.ReadAllText(_workersDataJsonFilePath);
            JsonData workersData = JsonMapper.ToObject(dataToJson);

            Workers.Clear();
            for (int i = 0; i < workersData["OwnedWorkers"].Count; i++)
            {
                Workers.Add(new Worker(
                    workersData["OwnedWorkers"][i]["Name"].ToString(),
                    workersData["OwnedWorkers"][i]["Description"].ToString(),
                    int.Parse(workersData["OwnedWorkers"][i]["Stamina"].ToString()),
                    int.Parse(workersData["OwnedWorkers"][i]["Knowledge"].ToString()),
                    int.Parse(workersData["OwnedWorkers"][i]["Fitness"].ToString()),
                    int.Parse(workersData["OwnedWorkers"][i]["Charisma"].ToString()),
                    workersData["OwnedWorkers"][i]["RelationshipStatus"].ToString()));
            }
        }
    }

    private void LoadCases(string cases, List<Case> casesContainer)
    {
        casesContainer.Clear();
        if (File.Exists(_casesJsonFilePath))
        {
            string dataToJson = File.ReadAllText(_casesJsonFilePath);
            JsonData casesData = JsonMapper.ToObject(dataToJson);

            for (int i = 0; i < casesData[cases].Count; i++)
            {
                List<Objective> newCaseObjectives = new List<Objective>();

                for (int j = 0; j < casesData[cases][i]["Objectives"].Count; j++)
                {
                    // These conditions make sure that we get the right type of data
                    // from the json and convert it accurately for the dictionaries
                    // of objectives for that case later on.
                    bool isObjectiveComplete = false;
                    if (casesData[cases][i]["Objectives"][j]["CompletedStatus"].ToString() == "True")
                    {
                        isObjectiveComplete = true;
                    }
                    else if (casesData[cases][i]["Objectives"][j]["CompletedStatus"].ToString() == "False")
                    {
                        isObjectiveComplete = false;
                    }

                    // Here we store the new dictionary (objective) to the list of
                    // objectives after we set up the new objective.
                    Objective newObjectives = new Objective(casesData[cases][i]["Objectives"][j]["Name"].ToString(), isObjectiveComplete);

                    newCaseObjectives.Add(newObjectives);
                }

                Case newCase = new Case(
                    casesData[cases][i]["Name"].ToString(),
                    casesData[cases][i]["Description"].ToString(),
                    newCaseObjectives);

                casesContainer.Add(newCase);
            }
        }

        //Debug.Log("Loaded " + cases + " json data!");
    }

    /// <summary>
    /// The functions below are used mainly for adding, removing and updating
    /// elements into existing player lists and  data related to stats and cases.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        Debug.Log(string.Format("Added {0} to current items!", item.Name));

        Items.Add(item);
        RefreshItems();
    }

    public void RemoveItem(Item item)
    {
        Debug.Log(string.Format("Removed {0} from current items!", item.Name));

        Items.Remove(item);
        RefreshItems();
    }

    public void AddWearable(Clothing clothing)
    {
        Debug.Log(string.Format("Added {0} to current wearables!", clothing.Name));

        Wearables.Add(clothing);
        RefreshWearables();
    }

    public void RemoveWearable(Clothing clothing)
    {
        Debug.Log(string.Format("Removed {0} from current wearables!", clothing.Name));

        Wearables.Remove(clothing);
        RefreshWearables();
    }

    public void UpdateAvailableHints(int value)
    {
        AvailableHints += value;
        RefreshJsonData();
    }

    public void DrinkCoffee()
    {
        if (IsCoffeeAvailable == true)
        {
            DateOfLastCoffee = DateTime.Now.ToString();
            IsCoffeeAvailable = false;
            RefreshJsonData();

            //Debug.Log("Drank coffee at " + DateOfLastCoffee + "!");
        }
        else
        {
            Debug.LogWarning("You cannot drink coffee at this time!");
        }
    }
    
    public void MoveCaseStageStatus(Case caseToMove, List<Case> fromStage, List<Case> newStage)
    {
        // I store caseToMove in a local variables because im not sure if by
        // removing it from its list, that i am also able to 
        if (fromStage.Contains(caseToMove))
        {
            fromStage.Remove(caseToMove);
        }
        newStage.Add(caseToMove);

        string returnOutput = "Case (" + caseToMove.Name + ") status updated to ";

        if (newStage == AvailableCases)
        {
            returnOutput += "Available";
        } else if (newStage == CurrentCases)
        {
            returnOutput += "Current";
        } else if (newStage == CompletedCases)
        {
            returnOutput += "Complete";
        }
        returnOutput += "!";

        RefreshAllCases();
        //Debug.Log(returnOutput);
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
                    Debug.Log("Increased reputation by " + statValues[i] + "!");
                    break;
                case "stamina":
                    Stamina += statValues[i];
                    Debug.Log("Increased Stamina by " + statValues[i] + "!");
                    break;
                case "knowledge":
                    Knowledge += statValues[i];
                    Debug.Log("Increased Knowledge by " + statValues[i] + "!");
                    break;
                case "fitness":
                    Fitness += statValues[i];
                    Debug.Log("Increased Fitness by " + statValues[i] + "!");
                    break;
                case "charisma":
                    Charisma += statValues[i];
                    Debug.Log("Increased charisma by " + statValues[i] + "!");
                    break;
                case "currency":
                    Currency += statValues[i];
                    Debug.Log("Increased currency by " + statValues[i] + "!");
                    break;
            }
        }

        RefreshJsonData();
    }

    /// <summary>
    /// These functions recount the effects of every worker/clothing for the player
    /// and must be executed every time you recruit a new worker and/or add a new
    /// clothing or one of their stats is changed.
    /// </summary>
    private void ApplyWorkersStats()
    {
        for (int i = 0; i < Workers.Count; i++)
        {
            Stamina += Workers[i].Stamina;
            Knowledge += Workers[i].Knowledge;
            Fitness += Workers[i].Fitness;
            Charisma += Workers[i].Charisma;
        }

        RefreshJsonData();
    }

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

    /// <summary>
    /// All the following functions refresh sections of data from the game
    /// and must be applied whenever a form of data is changed in-game to
    /// reflect that of the files for storage.
    /// </summary>
    private void RefreshCase(string caseToRefresh, List<Case> cases)
    {
        _newCasesData += "\"" + caseToRefresh + "\":[";
        // If we have an empty case then we will just create an
        // empty array of that case instead of populating it with
        // non-existent values.
        if (cases.Count == 0)
        {
            _newCasesData += "],";
            return;
        }
        else
        {
            for (int i = 0; i < cases.Count; i++)
            {
                _newCasesData += "{";
                // This is one field of the case object
                _newCasesData += "\"Name\":\"" + cases[i].Name + "\",";
                _newCasesData += "\"Description\":\"" + cases[i].Description + "\",";

                if (cases[i].CompletionStatus == true)
                {
                    _newCasesData += "\"Completed\":true,";
                }
                else if (cases[i].CompletionStatus == false)
                {
                    _newCasesData += "\"Completed\":false,";
                }

                _newCasesData += "\"Objectives\":" + "[";
                foreach (Objective objective in cases[i].Objectives)
                {
                    _newCasesData += "{";
                    _newCasesData += "\"Name\":\"" + objective.Name + "\",";
                    if (objective.CompletedStatus == true)
                    {
                        _newCasesData += "\"CompletedStatus\":true,";
                    }
                    else if (objective.CompletedStatus == false)
                    {
                        _newCasesData += "\"CompletedStatus\":false";
                    }
                    _newCasesData += "},";
                }
                // This closes the x type of cases
                _newCasesData = _newCasesData.Substring(0, _newCasesData.Length - 1);

                _newCasesData += "]},";
            }
            _newCasesData = _newCasesData.Substring(0, _newCasesData.Length - 1);
            _newCasesData += "],";
        }
    }

    private void RefreshAllCases()
    {
        File.WriteAllText(_casesJsonFilePath, "");
        // Reset the existing cases data string
        _newCasesData = "{";

        RefreshCase("AvailableCases", AvailableCases);
        RefreshCase("CurrentCases", CurrentCases);
        RefreshCase("CompletedCases", CompletedCases);

        _newCasesData = _newCasesData.Substring(0, _newCasesData.Length - 1);
        // This closes the wrapper of the json file made from the beginning.
        _newCasesData += "}";
        File.WriteAllText(_casesJsonFilePath, _newCasesData);

        //Debug.Log("Refreshed all cases json data!");
    }

    private void RefreshJsonData()
    {
        // We use the jsonMapper instead of the JsonUtility because the latter ruins
        // the objects in the items and clothing arrays for the json file.
        string newPlayerData = JsonUtility.ToJson(this);
        File.WriteAllText(_playerStatsFilePath, newPlayerData.ToString());

        //Debug.Log("Refreshed player json data!");
    }

    public void RefreshItems()
    {
        // We reset the existing items json list content, so that we can
        // append new one afterwards.
        File.WriteAllText(_itemsJsonFilePath, "");

        // This creates the starting wrapper of the json file.
        string newItemsData = "{\"Items\":[";

        foreach (Item item in Items)
        {
            string itemJson = JsonUtility.ToJson(item);
            newItemsData += itemJson + ",";
        }
        // This removes the last comma at the last item in the array, so
        // that we wont get an error when getting the data later on.
        newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += "]}";
        File.WriteAllText(_itemsJsonFilePath, newItemsData);

        //Debug.Log("Refreshed items json data!");
    }

    public void RefreshWorkersJson()
    {
        File.WriteAllText(_workersDataJsonFilePath, "");
        string newWorkersData = "{\"OwnedWorkers\":[";

        for (int i = 0; i < Workers.Count; i++)
        {
            newWorkersData += "{";
            newWorkersData += "\"Name\":\"" + Workers[i].Name + "\",";
            newWorkersData += "\"Description\":\"" + Workers[i].Description + "\",";
            newWorkersData += "\"Stamina\":" + Workers[i].Stamina + ",";
            newWorkersData += "\"Knowledge\":" + Workers[i].Knowledge + ",";
            newWorkersData += "\"Fitness\":" + Workers[i].Fitness + ",";
            newWorkersData += "\"Charisma\":" + Workers[i].Charisma + ",";
            newWorkersData += "\"RelationshipStatus\":\"" + Workers[i].RelationshipStatus + "\"";
            newWorkersData += "},";
        }

        newWorkersData = newWorkersData.Substring(0, newWorkersData.Length - 1);
        newWorkersData += "]}";

        File.WriteAllText(_workersDataJsonFilePath, newWorkersData);
    }

    public void RefreshWearables()
    {
        File.WriteAllText(_wearablesJsonFilePath, "");
        string newItemsData = "{\"Wearables\":[";

        foreach (Clothing item in Wearables)
        {
            string itemJson = JsonUtility.ToJson(item);
            newItemsData += itemJson + ",";
        }
        newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += "]}";
        File.WriteAllText(_wearablesJsonFilePath, newItemsData);

        //Debug.Log("Refreshed wearables json data!");
    }

    public void LoadInventory()
    {
        foreach (Item item in Items)
        {
            GameObject newItem = Instantiate(ItemPrefab, _itemsPanel.transform);
            Item newItemScript = newItem.GetComponent<Item>();

            // We use predefined images from the resources folder to load each
            // item's sprites from outside the game and assign it to the new item.
            Sprite sprite = Resources.Load<Sprite>("Items/" + item.AssetsImageName);

            newItem.GetComponent<Image>().sprite = sprite;
            newItemScript.Name = item.Name;
            newItemScript.Description = item.Description;
            newItemScript.Active = item.Active;
            newItemScript.AssetsImageName = item.AssetsImageName;
        }
    }

    public void ReloadInventory()
    {
        for (int i = 0; i < _itemsPanel.transform.childCount; i++)
        {
            Destroy(_itemsPanel.transform.GetChild(i).gameObject);
        }

        LoadInventory();
    }
}