using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class Worker : MonoBehaviour
{
    public string Name;
    public string Description;
    #region Stats
    public int Stamina;
    public int Knowledge;
    public int Fitness;
    public int Charisma;
    #endregion
    // Json parsing wouldnt allow enum types so we
    // are using a string intentionally.
    public string RelationshipStatus;
    string jsonFilePath;

    public Worker(string Name, string Description, int Stamina, int Knowledge, int Fitness, int Charisma)
    {
        this.Name = Name;
        this.Description = Description;
        this.Stamina = Stamina;
        this.Knowledge = Knowledge;
        this.Fitness = Fitness;
        this.Charisma = Charisma;
        RelationshipStatus = "Neutral";

        // Because workers dont exist at the start of the game, we have to
        // initiate their json files after instantiation and update them later on.
        jsonFilePath = Application.streamingAssetsPath + "/" + Name + ".json";
        InitiateJsonData();
    }

    private void InitiateJsonData()
    {
        if (!File.Exists(jsonFilePath))
        {
            string newWorkerData = JsonUtility.ToJson(this);
            File.WriteAllText(jsonFilePath, newWorkerData.ToString());
        }
    }

    // Whenever the worker data is updated or changed in-game, this
    // function makes sure the file related to this instance of workers
    // is updated as well.
    private void RefreshJsonData()
    {
        string newWorkerData = JsonUtility.ToJson(this);
        File.WriteAllText(jsonFilePath, newWorkerData.ToString());
    }

    public void UpdateRelationship(string newRelationshipStatus)
    {
        RelationshipStatus = newRelationshipStatus;
        RefreshJsonData();

        Debug.Log("Updated relationship status to " + RelationshipStatus);
    }
}
