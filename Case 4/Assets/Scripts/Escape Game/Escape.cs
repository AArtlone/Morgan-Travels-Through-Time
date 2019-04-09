using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;

public class Escape : MonoBehaviour
{
    public string Name;
    public int CurrentWave;
    public int RefugeesSaved;
    public int DelayBetweenWaves;
    public int TotalPoints;
    public GameObject RefugeePrefab;
    public List<Refugee> CurrentRefugees = new List<Refugee>();
    public List<Checkpoint> Checkpoints = new List<Checkpoint>();
    public List<RefugeeWaves> RefugeeWaves = new List<RefugeeWaves>();
    public string QuestForObjective;
    public int ObjectiveToCompleteID;
    public string AreaToUnlock;
    public GameObject EndGamePopUp;
    public List<GameObject> Stars;
    public int RefugeesSavedInThisSession;

    private string _escapeGamesJsonFile;

    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _escapeGamesJsonFile = Application.persistentDataPath + "/EscapeGames.json";

        string dataToJson = File.ReadAllText(_escapeGamesJsonFile);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);

        for (int i = 0; i < puzzlesJsonData["EscapeGames"].Count; i++)
        {
            if (puzzlesJsonData["EscapeGames"][i]["Name"].ToString() == Name)
            {
                TotalPoints = int.Parse(puzzlesJsonData["EscapeGames"][i]["TotalPoints"].ToString());
                RefugeesSaved = int.Parse(puzzlesJsonData["EscapeGames"][i]["RefugeesSaved"].ToString());
            }
        }
    }

    private void Start()
    {
        StartNextWave();
    }

    private IEnumerator SpawnRefugee()
    {
        if (_audioManager != null)
        {
            _audioManager.PlaySound(_audioManager.RefugeeSaved);
        }

        if (CurrentWave > 0)
        {
            yield return new WaitForSeconds(DelayBetweenWaves);
        }
        for (int i = 0; i < RefugeeWaves[CurrentWave].Wave.Count; i++)
        {
            foreach(Checkpoint checkPoint in Checkpoints)
            {
                checkPoint.CreateNewQueueElement();
            }
            GameObject newRefugee = Instantiate(RefugeeWaves[CurrentWave].Wave[i].gameObject, GameObject.FindGameObjectWithTag("Refugees Container").transform);
            newRefugee.GetComponent<Refugee>().RefugeeIndex = CurrentRefugees.Count;
            CurrentRefugees.Add(newRefugee.GetComponent<Refugee>());
            yield return new WaitForSeconds(2f);
        }
    }

    public void StartNextWave()
    {
        StartCoroutine(SpawnRefugee());
    }

    public void SaveEscapeGamesData()
    {
        string newGameData = "{\"EscapeGames\": [";

        string dataToJson = File.ReadAllText(_escapeGamesJsonFile);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);

        for (int i = 0; i < puzzlesJsonData["EscapeGames"].Count; i++)
        {
            newGameData += "{";
            if (puzzlesJsonData["EscapeGames"][i]["Name"].ToString() == Name)
            {
                newGameData +=
                    "\"Name\":" + "\"" + Name + "\"," +
                    "\"TotalPoints\":" + TotalPoints + "," +
                    "\"RefugeesSaved\":" + RefugeesSaved;
            }
            else
            {
                newGameData += "\"Name\":" + "\"" + puzzlesJsonData["EscapeGames"][i]["Name"].ToString() + "\",";
                newGameData += "\"TotalPoints\":" + int.Parse(puzzlesJsonData["EscapeGames"][i]["TotalPoints"].ToString()) + ",";
                newGameData += "\"RefugeesSaved\":" + int.Parse(puzzlesJsonData["EscapeGames"][i]["RefugeesSaved"].ToString());
            }
            newGameData += "},";
        }

        newGameData = newGameData.Substring(0, newGameData.Length - 1);
        newGameData += "]}";
        File.WriteAllText(_escapeGamesJsonFile, newGameData);

        if (QuestForObjective != string.Empty)
        {
            Character.Instance.CompleteObjectiveInQuest(ObjectiveToCompleteID, QuestForObjective);
        }
    }

    public void EndGame()
    {
        EndGamePopUp.SetActive(true);

        StartCoroutine(ShowStars());
    }

    private IEnumerator ShowStars()
    {
        if (_audioManager != null)
        {
            _audioManager.PlaySound(_audioManager.SoundPuzzleCompleted);
        }

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
}