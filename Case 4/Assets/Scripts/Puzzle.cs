using LitJson;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    public GameObject PuzzleToLaunch;
    public GameObject StarsDisplay;
    public TextMeshProUGUI ScoreText;
    public enum PuzzleType { HiddenObjects, CannonGame, GuessClothes };
    public PuzzleType TypeOfPuzzle;
    private HiddenObjectsPuzzle _puzzleScript;
    [Header("Only necessary if player is in tutorial mode!")]
    public GameObject InstructionManual;

    private string _jsonPuzzlesPath;

    private void Start()
    {
        _jsonPuzzlesPath = Application.persistentDataPath + "/Puzzles.json";

        _puzzleScript = PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>();

        // Depending on the puzzle we want to use in this prefab, we will run
        // different puzzle startup code, since the puzzles are entirely different
        // from eachother and we cant just slap the same code for them, so instead we
        // use functions made specifically for the puzzle we want in the PuzzleType field.
        if (TypeOfPuzzle == PuzzleType.HiddenObjects)
        {
            SetupHiddenObjectsPuzzle();
            Character.Instance.HiddenObjectsPuzzles.Add(PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>());
        } else if (TypeOfPuzzle == PuzzleType.CannonGame)
        {
            //Debug.Log("aa");
            //SetupHiddenObjectsPuzzle();
            //Character.Instance.HiddenObjectsPuzzles.Add(PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>());
        } else if (TypeOfPuzzle == PuzzleType.GuessClothes)
        {
            LoadScore();
        }
    }

    public void InitiateGuessClothesPuzzle()
    {
        PuzzleToLaunch.GetComponentInParent<GuessPuzzle>().StartPuzzle(PuzzleToLaunch);
    }

    public void InitiateHiddenObjectsPuzzle()
    {
        if (transform.GetComponent<Image>() && transform.GetComponent<Image>().raycastTarget == true)
        {
            // This centers the puzzle to the middle of the canvas because the actual puzzle
            // object is a child of the puzzle prefab and that changes its position to the
            // center of that prefab instead of the canvas instead.
            PuzzleToLaunch.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            PuzzleToLaunch.SetActive(true);
            PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>().ItemsFound.Clear();
            PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>().StartTimer();
            transform.GetComponent<Image>().raycastTarget = false;
        } else
        {
            PuzzleToLaunch.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            PuzzleToLaunch.SetActive(true);
            PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>().ItemsFound.Clear();
            PuzzleToLaunch.GetComponent<HiddenObjectsPuzzle>().StartTimer();
        }

        // Shows the instruction manual to the player if it is his first time
        // playing the game/tutorial.
        if (Character.Instance.TutorialCompleted == false)
        {
            InterfaceManager.Instance.OpenPopup(InstructionManual);
        }
    }

    public void InitiateCannonGame()
    {
        if (transform.GetComponent<Image>().raycastTarget == true)
        {
            // This centers the puzzle to the middle of the canvas because the actual puzzle
            // object is a child of the puzzle prefab and that changes its position to the
            // center of that prefab instead of the canvas instead.
            PuzzleToLaunch.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            PuzzleToLaunch.SetActive(true);
            transform.GetComponent<Image>().raycastTarget = false;
        }
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
                    puzzleScript.Stars = int.Parse(puzzlesData["Puzzles"][i]["Stars"].ToString());

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

        LoadStars();
    }

    public void RefreshHiddenObjectsPuzzle()
    {
        string newPuzzleData = string.Empty;
        newPuzzleData += "{\"Puzzles\":[";

        for (int i = 0; i < Character.Instance.HiddenObjectsPuzzles.Count; i++)
        {
            HiddenObjectsPuzzle currentPuzzle = Character.Instance.HiddenObjectsPuzzles[i];

            newPuzzleData += "{";
            newPuzzleData += "\"Name\":\"" + currentPuzzle.PuzzleName + "\",";
            newPuzzleData += "\"Stars\":" + currentPuzzle.Stars + ",";
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

        LoadStars();
    }

    // This part loads the stars on top of the puzzle on the minimap
    public void LoadStars()
    {
        for (int i = 0; i < StarsDisplay.transform.childCount; i++)
        {
            StarsDisplay.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < _puzzleScript.Stars; i++)
        {
            StarsDisplay.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void LoadScore()
    {
        ScoreText.text = PuzzleToLaunch.GetComponentInParent<GuessPuzzle>().TotalScore.ToString() + " points";
    }
}
