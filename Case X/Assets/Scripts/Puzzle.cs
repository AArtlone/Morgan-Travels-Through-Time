using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    public GameObject PuzzleToLaunch;
    public enum PuzzleType { HiddenObjects };
    public PuzzleType TypeOfPuzzle;

    private string _jsonPuzzlesPath;

    private void Start()
    {
        _jsonPuzzlesPath = Application.persistentDataPath + "/Puzzles.json";

        if (TypeOfPuzzle == PuzzleType.HiddenObjects)
        {
            SetupHiddenObjectsPuzzle();
            DialogueManager.Instance.HiddenObjectsPuzzles.Add(PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>());
        }
    }

    public void InitiateHiddenObjectsPuzzle()
    {
        PuzzleToLaunch.SetActive(true);
        PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>().ItemsFound.Clear();
        PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>().StartCoroutine(
        PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>().CountDown());
    }

    private void SetupHiddenObjectsPuzzle()
    {
        if (File.Exists(_jsonPuzzlesPath))
        {
            string dataToJson = File.ReadAllText(_jsonPuzzlesPath);
            JsonData puzzlesData = JsonMapper.ToObject(dataToJson);

            for (int i = 0; i < puzzlesData["Puzzles"].Count; i++)
            {
                HiddenObjectsPuzzle puzzleScript = PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>();
                if (puzzlesData["Puzzles"][i]["Name"].ToString() == puzzleScript.PuzzleName)
                {
                    puzzleScript.Score = int.Parse(puzzlesData["Puzzles"][i]["Score"].ToString());

                    if (puzzlesData["Puzzles"][i]["Completed"].ToString() == "True")
                    {
                        puzzleScript.IsItemEarned = true;
                    } else if (puzzlesData["Puzzles"][i]["Completed"].ToString() == "False")
                    {
                        puzzleScript.IsItemEarned = false;
                    }
                }
            }
        }
    }

    public void RefreshHiddenObjectsPuzzle()
    {
        HiddenObjectsPuzzle puzzleScript = PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>();

        string newPuzzleData = string.Empty;
        newPuzzleData += "{\"Puzzles\":[";

        for (int i = 0; i < DialogueManager.Instance.HiddenObjectsPuzzles.Count; i++)
        {
            HiddenObjectsPuzzle currentPuzzle = DialogueManager.Instance.HiddenObjectsPuzzles[i];

            newPuzzleData += "{";
            newPuzzleData += "\"Name\":\"" + currentPuzzle.PuzzleName + "\",";
            newPuzzleData += "\"Score\":" + currentPuzzle.Score + ",";
            if (currentPuzzle.IsItemEarned)
            {
                newPuzzleData += "\"Completed\":true";
            } else
            {
                newPuzzleData += "\"Completed\":false";
            }
            newPuzzleData += "},";
        }
        newPuzzleData = newPuzzleData.Substring(0, newPuzzleData.Length - 1);
        newPuzzleData += "]}";

        File.WriteAllText(_jsonPuzzlesPath, newPuzzleData);
    }
}
