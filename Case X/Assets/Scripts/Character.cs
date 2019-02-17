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
    public List<string> CurrentCases = new List<string>();
    public List<string> CompletedCases = new List<string>();
    public int AvailableHints;
    public string DateOfLastCoffee;
    public bool IsCoffeeAvailable;

    // Location reference of player json file.
    private string _jsonFilePath;
    private string _itemsJsonFilePath;
    private string _wearablesJsonFilePath;

    private void Start()
    {
        _jsonFilePath = Application.streamingAssetsPath + "/Player.json";
        _itemsJsonFilePath = Application.streamingAssetsPath + "/Items.json";
        _wearablesJsonFilePath = Application.streamingAssetsPath + "/Wearables.json";
        SetupJsonData();
        SetupItems();
        SetupWearables();
    }

    // We use this function to read the existing data from the
    // character's file and set his fields to match the data. Basically
    // we load his stored and previously saved data from the storage
    // into the in-game character.
    private void SetupJsonData()
    {
        if (File.Exists(_jsonFilePath))
        {
            string dataToJson = File.ReadAllText(_jsonFilePath);
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
            for (int i = 0; i < characterData["CurrentCases"].Count; i++)
            {
                CurrentCases.Add(characterData["CurrentCases"][i].ToString());
            }
            for (int i = 0; i < characterData["CompletedCases"].Count; i++)
            {
                CompletedCases.Add(characterData["CompletedCases"][i].ToString());
            }

            AvailableHints = int.Parse(characterData["AvailableHints"].ToString());
            DateOfLastCoffee = characterData["DateOfLastCoffee"].ToString();
            
            if (characterData["IsCoffeeAvailable"].ToString() == "True")
            {
                IsCoffeeAvailable = true;
            } else if (characterData["IsCoffeeAvailable"].ToString() == "False")
            {
                IsCoffeeAvailable = false;
            }
        }

        Debug.Log("Loaded character json data!");
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

        Debug.Log("Loaded items json data!");
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

        Debug.Log("Refreshed items json data!");
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

        Debug.Log("Loaded wearables json data!");
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

        Debug.Log("Refreshed wearables json data!");
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

            Debug.Log("Drank coffee at " + DateOfLastCoffee + "!");
        } else
        {
            Debug.LogWarning("You cannot drink coffee at this time!");
        }
    }

    public void RemoveACurrentCase(string caseToRemove)
    {
        CurrentCases.Remove(caseToRemove);
        RefreshJsonData();

        Debug.Log(string.Format("Removed {0} from current cases!", caseToRemove));
    }

    public void AddACurrentCase(string caseToAdd)
    {
        CurrentCases.Add(caseToAdd);
        RefreshJsonData();

        Debug.Log(string.Format("Added {0} to current cases!", caseToAdd));
    }

    public void AddACompletedCase(string caseToComplete)
    {
        CompletedCases.Add(caseToComplete);
        RefreshJsonData();

        Debug.Log(string.Format("Added {0} to completed cases!", caseToComplete));
    }

    public void MoveACaseToCompleted(string caseToComplete)
    {
        RemoveACurrentCase(caseToComplete);
        AddACompletedCase(caseToComplete);
    }

    private void RefreshJsonData()
    {
        // We use the jsonMapper instead of the JsonUtility because the latter ruins
        // the objects in the items and clothing arrays for the json file.
        string newPlayerData = JsonUtility.ToJson(this);
        File.WriteAllText(_jsonFilePath, newPlayerData.ToString());

        Debug.Log("Refreshed player json data!");
    }
}