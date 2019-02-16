using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using System;

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
    public List<Clothing> Wearables;
    public List<Item> Items;
    public List<string> Clothing;
    #endregion
    public List<string> CurrentCases;
    public List<string> CompletedCases;
    public int AvailableHints;
    public string DateOfLastCoffee;
    public bool IsCoffeeAvailable;

    private void Start()
    {
        SetupJsonData();
    }

    // We use this function to read the existing data from the
    // character's file and set his fields to match the data. Basically
    // we load his stored and previously saved data from the storage
    // into the in-game character.
    private void SetupJsonData()
    {
        // Location of player json file.
        string path = Application.streamingAssetsPath + "/Player.json";

        if (File.Exists(path))
        {
            string dataToJson = File.ReadAllText(path);
            JsonData characterData = JsonMapper.ToObject(dataToJson);

            Name = characterData["Name"].ToString();
            Reputation = int.Parse(characterData["Stats"]["Reputation"].ToString());
            Stamina = int.Parse(characterData["Stats"]["Stamina"].ToString());
            Knowledge = int.Parse(characterData["Stats"]["Knowledge"].ToString());
            Fitness = int.Parse(characterData["Stats"]["Fitness"].ToString());
            Charisma = int.Parse(characterData["Stats"]["Charisma"].ToString());
            Currency = int.Parse(characterData["Currency"].ToString());

            Wearables.Clear();
            for (int i = 0; i < characterData["Wearables"].Count; i++)
            {
                Wearables.Add(new Clothing(
                    characterData["Wearables"][i]["Body Part"].ToString(),
                    characterData["Wearables"][i]["Name"].ToString()));
            }

            Items.Clear();
            for (int i = 0; i < characterData["Items"].Count; i++)
            {
                Items.Add(new Item(
                    characterData["Items"][i]["Name"].ToString(),
                    characterData["Items"][i]["Description"].ToString(),
                    characterData["Items"][i]["Passives"].ToString(),
                    characterData["Items"][i]["Actives"].ToString()));
            }

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
    }
}
