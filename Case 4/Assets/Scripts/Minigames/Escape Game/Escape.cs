using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;
using System;
using Anima2D;

public class Escape : MonoBehaviour
{
    public EscapeInventory Inventory;
    public CannonBallShooting _cannonInterface;
    public bool CustomizeNPC = false;
    public string Name;
    public string NameDutch;
    public bool Completed;
    public int CurrentWave;
    public int RefugeesSaved;
    public int DelayBetweenWaves;
    public int TotalPoints;
    public GameObject TreeObstaclePrefab;
    public List<List<Refugee>> CurrentRefugees = new List<List<Refugee>>();
    public List<Checkpoint> Checkpoints = new List<Checkpoint>();
    public List<KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement>> AllObsIntElements = new List<KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement>>();
    public List<RefugeeWaves> RefugeeWaves = new List<RefugeeWaves>();
    public List<GameObject> ItemsOnMap = new List<GameObject>();
    public string QuestForObjective;
    public int ObjectiveToCompleteID;
    public string AreaToUnlock;
    public GameObject EndGamePopUp;
    public GameObject EndGameLosePopUp;
    public GameObject NewWaveNotification;
    public List<GameObject> AllNPCPrefabs = new List<GameObject>();
    public List<GameObject> Stars;
    public int RefugeesSavedInThisSession;

    [Header("List of Item Prefabs that player will receive but that are not in the scene.")]
    public Item FullBucketPrefab;
    public Item EmptyBucketPrefab;
    public Item BommenBerendFlagPrefab;

    [Header("Obstacle related objects.")]
    public GameObject Antagonist;
    public GameObject Fire;

    [Header("Other prefabs.")]
    public GameObject StretcherPrefab;

    private string _escapeGamesJsonFile;

    // Layer-related references
    [NonSerialized]
    public List<LayerAdjuster> LayerAdjusters = new List<LayerAdjuster>();
    private int LayerMultiplier;

    private void Awake()
    {
        Inventory = new EscapeInventory();
        _escapeGamesJsonFile = Application.persistentDataPath + "/EscapeGames.json";

        string dataToJson = File.ReadAllText(_escapeGamesJsonFile);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);

        for (int i = 0; i < puzzlesJsonData["EscapeGames"].Count; i++)
        {
            if (puzzlesJsonData["EscapeGames"][i]["Name"].ToString() == Name || puzzlesJsonData["EscapeGames"][i]["NameDutch"].ToString() == NameDutch)
            {
                if (puzzlesJsonData["EscapeGames"][i]["Completed"].ToString() == "True")
                {
                    Completed = true;
                }
                else if (puzzlesJsonData["EscapeGames"][i]["Completed"].ToString() == "False")
                {
                    Completed = false;
                }
                TotalPoints = int.Parse(puzzlesJsonData["EscapeGames"][i]["TotalPoints"].ToString());
                RefugeesSaved = int.Parse(puzzlesJsonData["EscapeGames"][i]["RefugeesSaved"].ToString());
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < RefugeeWaves.Count; i++)
        {
            CurrentRefugees.Add(new List<Refugee>());
        }
        StartNextWave();
    }

    private IEnumerator SpawnRefugee()
    {
        if (CurrentWave > 0)
        {
            yield return new WaitForSeconds(DelayBetweenWaves);
        }
        for (int i = 0; i < RefugeeWaves[CurrentWave].Wave.Count; i++)
        {
            /*foreach(Checkpoint checkPoint in Checkpoints)
            {
                checkPoint.CreateNewQueueElement();
            }*/
            GameObject newRefugee;
            if (CustomizeNPC)
            {
                newRefugee = Instantiate(RefugeeWaves[CurrentWave].Wave[i].gameObject, GameObject.FindGameObjectWithTag("Refugees Container").transform);
                Physics2D.IgnoreLayerCollision(10, 10);
                CurrentRefugees[CurrentWave].Add(newRefugee.GetComponent<Refugee>());
                newRefugee.GetComponent<Refugee>().RefugeeIndex = CurrentRefugees[CurrentWave].Count - 1;
                newRefugee.GetComponent<Refugee>().WaveIndex = CurrentWave;
            } else
            {
                int randomRefugeeIndex = UnityEngine.Random.Range(0, AllNPCPrefabs.Count);
                newRefugee = Instantiate(AllNPCPrefabs[randomRefugeeIndex].gameObject, GameObject.FindGameObjectWithTag("Refugees Container").transform);
                Physics2D.IgnoreLayerCollision(10, 10);
                CurrentRefugees[CurrentWave].Add(newRefugee.GetComponent<Refugee>());
                newRefugee.GetComponent<Refugee>().RefugeeIndex = CurrentRefugees[CurrentWave].Count - 1;
                newRefugee.GetComponent<Refugee>().WaveIndex = CurrentWave;
            }

            foreach (SpriteMeshInstance component in newRefugee.GetComponentsInChildren<SpriteMeshInstance>())
            {
                component.sortingOrder = component.sortingOrder + LayerMultiplier;
            }

            foreach (LayerAdjuster layerAdjuster in LayerAdjusters)
            {
                layerAdjuster.UpdateLayer();
            }

            LayerMultiplier += 4;
            
            yield return new WaitForSeconds(.5f);
        }
    }
    
    public void RespawnFlagObstacle(GameObject obstacle)
    {
        StartCoroutine(RespawnFlagObstacleCo(obstacle, 5f));
    }

    private IEnumerator RespawnFlagObstacleCo(GameObject obstacle, float timeDelay)
    {
        Antagonist.SetActive(true);
        // TODO: Play evil laugh
        yield return new WaitForSeconds(2f);
        Antagonist.SetActive(false);
        yield return new WaitForSeconds(timeDelay);
        obstacle.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Items/Inventory/BommenBerend Flag");
    }

    public void RespawnObstacle(GameObject obstacle)
    {
        StartCoroutine(RespawnObstacleCo(obstacle, 5f));
    } 

    private IEnumerator RespawnObstacleCo(GameObject obstacle, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        obstacle.SetActive(true);
    }

    public void StartNextWave()
    {
        ResetObstacleValuesForNextWave();
        StartCoroutine(SpawnRefugee());
        StartCoroutine(PlayNewWaveNotification());
    }

    public void SaveEscapeGamesData()
    {
        string newGameData = "{";
        newGameData += InsertNewLineTabs(1);
        newGameData += "\"EscapeGames\": [";

        string dataToJson = File.ReadAllText(_escapeGamesJsonFile);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);

        for (int i = 0; i < puzzlesJsonData["EscapeGames"].Count; i++)
        {
            newGameData += InsertNewLineTabs(2);
            newGameData += "{";

            if (puzzlesJsonData["EscapeGames"][i]["Name"].ToString() == Name ||
                puzzlesJsonData["EscapeGames"][i]["NameDutch"].ToString() == NameDutch)
            {
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"Name\": " + "\"" + Name + "\",";
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"NameDutch\": " + "\"" + NameDutch + "\",";
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"Completed\": " + (Completed ? " true" : " false") + ",";
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"TotalPoints\": " + TotalPoints + ",";
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"RefugeesSaved\": " + RefugeesSaved;
            }
            else
            {
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"Name\": " + "\"" + puzzlesJsonData["EscapeGames"][i]["Name"].ToString() + "\",";
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"NameDutch\": " + "\"" + puzzlesJsonData["EscapeGames"][i]["NameDutch"].ToString() + "\",";
                newGameData += InsertNewLineTabs(3);
                if (puzzlesJsonData["EscapeGames"][i]["Completed"].ToString() == "True")
                {
                    newGameData += "\"Completed\": true,";
                }
                else if (puzzlesJsonData["EscapeGames"][i]["Completed"].ToString() == "False")
                {
                    newGameData += "\"Completed\": false,";
                }
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"TotalPoints\": " + int.Parse(puzzlesJsonData["EscapeGames"][i]["TotalPoints"].ToString()) + ",";
                newGameData += InsertNewLineTabs(3);
                newGameData += "\"RefugeesSaved\": " + int.Parse(puzzlesJsonData["EscapeGames"][i]["RefugeesSaved"].ToString());
            }
            newGameData += InsertNewLineTabs(2);
            newGameData += "},";
        }

        newGameData = newGameData.Substring(0, newGameData.Length - 1);
        newGameData += InsertNewLineTabs(1) + "]" + Environment.NewLine + "}";
        File.WriteAllText(_escapeGamesJsonFile, newGameData);

        if (QuestForObjective != string.Empty)
        {
            Character.Instance.CompleteObjectiveInQuest(ObjectiveToCompleteID, QuestForObjective);
        }
    }

    public IEnumerator PlayNewWaveNotification()
    {
        for (int i = 0; i < 3; i++)
        {
            NewWaveNotification.SetActive(true);
            yield return new WaitForSeconds(1f);
            NewWaveNotification.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }
 
    public void EndGame()
    {
        EndGamePopUp.SetActive(true);

        StartCoroutine(ShowStars());
    }

    public void EndGameLose()
    {
        EndGameLosePopUp.SetActive(true);

        StartCoroutine(ShowStars());
    }

    private IEnumerator ReactivateItemCO(GameObject itemObj)
    {
        yield return new WaitForSeconds(1f);
        itemObj.SetActive(true);
    }

    public void ReactivateItem(GameObject itemObj)
    {
        StartCoroutine(ReactivateItemCO(itemObj));
    }

    private IEnumerator ShowStars()
    {
        Character.Instance.UpdateAreaStatus(AreaToUnlock, "Unlocked");

        if (RefugeesSavedInThisSession > 2)
        {
            for (int i = 0; i < Stars.Count; i++)
            {
                yield return new WaitForSeconds(0.5f);
                Stars[i].SetActive(true);
            }
        }
    }
    private string InsertNewLineTabs(int numberOfTabs)
    {
        string whiteSpace = Environment.NewLine;
        for (int i = 0; i < numberOfTabs; i++)
        {
            whiteSpace += "\t";
        }

        return whiteSpace;
    }
    
    private void ResetObstacleValuesForNextWave()
    {
        _cannonInterface.CurrentNumberOfsalvosShot = 0;
        _cannonInterface.CanShoot = true;
        ObstacleIntercationElement.ResetValuesForNextWave();
    }


    public int GetLayerMultiplier()
    {
        return LayerMultiplier;
    }
}