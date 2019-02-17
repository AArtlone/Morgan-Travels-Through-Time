using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using System;
using Newtonsoft.Json;

public class Character : MonoBehaviour
{
    public string Name;
    #region Stats
    public int Reputation;
    public int Stamina;
    public int Knowledge;
    public int Fitness;
    public int Charisma;
    public int Currency;
    #endregion
    #region Items
    [NonSerialized]
    public List<Clothing> Wearables = new List<Clothing>();
    [NonSerialized]
    public List<Item> Items = new List<Item>();
    public List<string> Clothing = new List<string>();
    #endregion
    public List<Case> AvailableCases = new List<Case>();
    public List<Case> CurrentCases = new List<Case>();
    public List<Case> CompletedCases = new List<Case>();
    public int AvailableHints;
    public string DateOfLastCoffee;
    public bool IsCoffeeAvailable;

    // Location reference of player json file.
    private string _playerStatsFilePath;
    private string _casesJsonFilePath;
    private string _itemsJsonFilePath;
    private string _wearablesJsonFilePath;
    private string _newCasesData;

    private void Start()
    {
        _playerStatsFilePath = Application.streamingAssetsPath + "/Player.json";
        _casesJsonFilePath = Application.streamingAssetsPath + "/Cases.json";
        _itemsJsonFilePath = Application.streamingAssetsPath + "/Items.json";
        _wearablesJsonFilePath = Application.streamingAssetsPath + "/Wearables.json";
        SetupJsonData();
        SetupCases();
        SetupItems();
        SetupWearables();

        //MoveCaseStageStatus(AvailableCases[0], AvailableCases, CurrentCases);
    }

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

            Clothing.Clear();
            for (int i = 0; i < characterData["Clothing"].Count; i++)
            {
                Clothing.Add(characterData["Clothing"][i].ToString());
            }

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
                    characterData["Items"][i]["Passives"].ToString(),
                    characterData["Items"][i]["Actives"].ToString()));
            }
        }

        //Debug.Log("Loaded items json data!");
    }

    private void RefreshItems()
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

    private void SetupWearables()
    {
        if (File.Exists(_wearablesJsonFilePath))
        {
            string dataToJson = File.ReadAllText(_wearablesJsonFilePath);
            JsonData characterData = JsonMapper.ToObject(dataToJson);

            Wearables.Clear();
            for (int i = 0; i < characterData["Wearables"].Count; i++)
            {
                Wearables.Add(new Clothing(
                    characterData["Wearables"][i]["BodyPart"].ToString(),
                    characterData["Wearables"][i]["Name"].ToString()));
            }
        }

        //Debug.Log("Loaded wearables json data!");
    }

    private void RefreshWearables()
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
}