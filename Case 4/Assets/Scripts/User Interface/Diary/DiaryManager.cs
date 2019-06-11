using LitJson;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    // References to the buttons and pages related to the diary and toggling.
    public GameObject PuzzleDiaryDisplay;
    public GameObject EscapeGameDiaryDisplay;
    public GameObject GuessClothingDiaryDisplay;
    public GameObject SpellingPuzzleDiaryDisplay;
    public GameObject DialogueLogDisplay;
    [SerializeField]
    private List<GameObject> _buttonsForPages = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _pages = new List<GameObject>();

    // Risky, but I want to try it
    private List<dynamic> AllPuzzles = new List<dynamic>();
    private List<KeyValuePair<int, List<dynamic>>> PagesOfPuzzles = new List<KeyValuePair<int, List<dynamic>>>();
    public GameObject GridOfPuzzles;
    private int _currentPage;
    private const int _maxNumberOfPuzzlesAPage = 6;
    private int _numberOfPagesOfPuzzles;
    private int _numberOfPuzzles;
    private int _currentIndexInPuzzles;
    

    private void Start()
    {
        GetPuzzlesFromStorage();
    }

    public void CloseAllPages()
    {
        foreach (GameObject page in _pages)
        {
            page.SetActive(false);
        }

        DialogueLogDisplay.SetActive(true);
    }

    public void SelectPage(Object selectedButton)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.NewPageInDiary);

        GameObject buttonObj = (GameObject)selectedButton;

        foreach (GameObject button in _buttonsForPages)
        {
            if (button == buttonObj)
            {
                DialogueLogDisplay.SetActive(false);

                button.SetActive(true);
                button.GetComponentInChildren<Image>().color = new Color(255, 255, 255, 1);

                foreach (GameObject page in _pages)
                {
                    if (button.name == "Quest Button")
                    {
                        if (page.name == "Quests Page")
                        {
                            page.SetActive(true);
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    }
                    else if (button.name == "Blueprints Button")
                    {
                        if (page.name == "Blueprints Page")
                        {
                            page.SetActive(true);
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    }
                    else if (button.name == "Achievements Button")
                    {
                        if (page.name == "Achievements Log Page")
                        {
                            page.SetActive(true);
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    }
                    else if (button.name == "Help Button")
                    {
                        if (page.name == "Help Page")
                        {
                            page.SetActive(true);
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    }
                    else if (button.name == "Dialogue Log Button")
                    {
                        if (page.name == "Dialogue Log Page")
                        {
                            for (int i = 0; i < page.transform.GetChild(1).transform. childCount; i++)
                            {
                                Destroy(page.transform.GetChild(1).transform.GetChild(i).gameObject);
                            }

                            page.SetActive(true);
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    }
                    else if (button.name == "Puzzles Button")
                    {
                        if (page.name == "Puzzles Section")
                        {
                            page.SetActive(true);

                            _currentPage = 1;
                            LoadNewPuzzlesPage("Backward");
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                button.GetComponentInChildren<Image>().color = new Color(255, 255, 255, 0.5f);
            }
        }
    }

    public void LoadNewPuzzlesPage(string direction)
    {
        GetPuzzlesFromStorage();

        if (direction == "Backward" || direction == "Backwards")
        {
            _currentPage--;
        }
        else if (direction == "Forward" || direction == "Forwards")
        {
            _currentPage++;
        }

        if (_currentPage > _numberOfPagesOfPuzzles)
        {
            _currentPage = 0;
        }
        else if (_currentPage < 0)
        {
            _currentPage = _numberOfPagesOfPuzzles;
        }

        int startRange = ((_currentPage + 1) * 6) - 5;
        int endRange = (_currentPage + 1) * 6;

        // Appending new puzzles into the page.
        for (int i = 0; i < AllPuzzles.Count; i++)
        {
            if (AllPuzzles[i].Completed == true)
            {
                GameObject newPuzzleDisplay = null;
                if (AllPuzzles[i].Type == "Hidden Objects Puzzle")
                {
                    newPuzzleDisplay = Instantiate(PuzzleDiaryDisplay, GridOfPuzzles.transform);

                    for (int j = 0; j < AllPuzzles[i].Stars; j++)
                    {
                        newPuzzleDisplay.transform.GetChild(3).transform.GetChild(j).gameObject.SetActive(true);
                    }
                }
                else if (AllPuzzles[i].Type == "Guess Clothing Puzzle")
                {
                    newPuzzleDisplay = Instantiate(GuessClothingDiaryDisplay, GridOfPuzzles.transform);
                    newPuzzleDisplay.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Score: " + AllPuzzles[i].Score;
                }
                else if (AllPuzzles[i].Type == "Escape Game")
                {
                    newPuzzleDisplay = Instantiate(EscapeGameDiaryDisplay, GridOfPuzzles.transform);
                    newPuzzleDisplay.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Score: " + AllPuzzles[i].Score;
                    newPuzzleDisplay.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Refugees Saved:" + AllPuzzles[i].RefugeesSaved;
                }
                else if (AllPuzzles[i].Type == "Spelling Puzzle")
                {
                    newPuzzleDisplay = Instantiate(SpellingPuzzleDiaryDisplay, GridOfPuzzles.transform);

                    for (int j = 0; j < AllPuzzles[i].Stars; j++)
                    {
                        newPuzzleDisplay.transform.GetChild(3).transform.GetChild(j).gameObject.SetActive(true);
                    }
                }

                LanguageController languageComponent = newPuzzleDisplay.transform.GetChild(1).GetComponent<LanguageController>();

                switch (SettingsManager.Instance.Language)
                {
                    case "English":
                        newPuzzleDisplay.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = AllPuzzles[i].Name;
                        break;
                    case "Dutch":
                        newPuzzleDisplay.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = AllPuzzles[i].NameDutch;
                        break;
                }

                languageComponent.English = AllPuzzles[i].Name;
                languageComponent.Dutch = AllPuzzles[i].NameDutch;
            }
        }
    }

    private void GetPuzzlesFromStorage()
    {
        // Clearing the pages.
        for (int i = 0; i < GridOfPuzzles.transform.childCount; i++)
        {
            Destroy(GridOfPuzzles.transform.GetChild(i).transform.gameObject);
        }

        AllPuzzles.Clear();
        PagesOfPuzzles.Clear();
        JsonData HOPPuzzlesJsonData = null;
        string HOPPuzzlesJsonDataPath = Application.persistentDataPath + "/HiddenObjectPuzzles.json";

        JsonData guessClothingPuzzlesJsonData = null;
        string guessClothingPuzzlesJsonDataPath = Application.persistentDataPath + "/GuessingPuzzles.json";

        JsonData escapeGamesJsonData = null;
        string escapeGamesJsonDataPath = Application.persistentDataPath + "/EscapeGames.json";

        JsonData spellingPuzzlesJsonData = null;
        string spellingPuzzlesPath = Application.persistentDataPath + "/SpellingPuzzles.json";
        if (File.Exists(HOPPuzzlesJsonDataPath))
        {
            HOPPuzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(HOPPuzzlesJsonDataPath));
        } else
        {
            Debug.LogWarning($"File {HOPPuzzlesJsonDataPath} was not valid or found!");
        }
        if (File.Exists(guessClothingPuzzlesJsonDataPath))
        {
            guessClothingPuzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(guessClothingPuzzlesJsonDataPath));
        } else
        {
            Debug.LogWarning($"File {guessClothingPuzzlesJsonDataPath} was not valid or found!");
        }
        if (File.Exists(escapeGamesJsonDataPath))
        {
            escapeGamesJsonData = JsonMapper.ToObject(File.ReadAllText(escapeGamesJsonDataPath));
        } else
        {
            Debug.LogWarning($"File {escapeGamesJsonDataPath} was not valid or found!");
        }
        if (File.Exists(spellingPuzzlesPath))
        {
            spellingPuzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(spellingPuzzlesPath));
        } else
        {
            Debug.LogWarning($"File {spellingPuzzlesPath} was not valid or found!");
        }

        _numberOfPuzzles += 
            (HOPPuzzlesJsonData != null ? HOPPuzzlesJsonData["Puzzles"].Count : 0) +
            (guessClothingPuzzlesJsonData != null ? guessClothingPuzzlesJsonData["Puzzles"].Count : 0) +
            (escapeGamesJsonData != null ? escapeGamesJsonData["EscapeGames"].Count : 0) + 
            (spellingPuzzlesJsonData != null ? spellingPuzzlesJsonData["Puzzles"].Count : 0);

        // Put all the puzzles in a list.
        if (File.Exists(HOPPuzzlesJsonDataPath))
        {
            for (int i = 0; i < HOPPuzzlesJsonData["Puzzles"].Count; i++)
            {
                bool isItComplete = false;
                if (HOPPuzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True")
                {
                    isItComplete = true;
                }
                else if (HOPPuzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "False")
                {
                    isItComplete = false;
                }

                var puzzleObj = new
                {
                    Type = "Hidden Objects Puzzle",
                    Name = HOPPuzzlesJsonData["Puzzles"][i]["Name"].ToString(),
                    NameDutch = HOPPuzzlesJsonData["Puzzles"][i]["NameDutch"].ToString(),
                    Completed = isItComplete,
                    Stars = int.Parse(HOPPuzzlesJsonData["Puzzles"][i]["Stars"].ToString())
                };

                AllPuzzles.Add(puzzleObj);
            }
        }

        if (File.Exists(guessClothingPuzzlesJsonDataPath))
        {
            for (int i = 0; i < guessClothingPuzzlesJsonData["Puzzles"].Count; i++)
            {
                bool isItComplete = false;
                if (guessClothingPuzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True")
                {
                    isItComplete = true;
                }
                else if (guessClothingPuzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "False")
                {
                    isItComplete = false;
                }

                var puzzleObj = new
                {
                    Type = "Guess Clothing Puzzle",
                    Name = guessClothingPuzzlesJsonData["Puzzles"][i]["Name"].ToString(),
                    NameDutch = guessClothingPuzzlesJsonData["Puzzles"][i]["NameDutch"].ToString(),
                    Completed = isItComplete,
                    Score = int.Parse(guessClothingPuzzlesJsonData["Puzzles"][i]["Stars"].ToString())
                };

                AllPuzzles.Add(puzzleObj);
            }
        }

        if (File.Exists(escapeGamesJsonDataPath))
        {
            for (int i = 0; i < escapeGamesJsonData["EscapeGames"].Count; i++)
            {
                bool isItComplete = false;
                if (escapeGamesJsonData["EscapeGames"][i]["Completed"].ToString() == "True")
                {
                    isItComplete = true;
                }
                else if (escapeGamesJsonData["EscapeGames"][i]["Completed"].ToString() == "False")
                {
                    isItComplete = false;
                }

                var puzzleObj = new
                {
                    Type = "Escape Game",
                    Name = escapeGamesJsonData["EscapeGames"][i]["Name"].ToString(),
                    NameDutch = escapeGamesJsonData["EscapeGames"][i]["NameDutch"].ToString(),
                    Score = int.Parse(escapeGamesJsonData["EscapeGames"][i]["TotalPoints"].ToString()),
                    Completed = isItComplete,
                    RefugeesSaved = int.Parse(escapeGamesJsonData["EscapeGames"][i]["RefugeesSaved"].ToString())
                };

                AllPuzzles.Add(puzzleObj);
            }
        }

        if (File.Exists(spellingPuzzlesPath))
        {
            for (int i = 0; i < spellingPuzzlesJsonData["Puzzles"].Count; i++)
            {
                bool isItComplete = false;
                if (spellingPuzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True")
                {
                    isItComplete = true;
                }
                else if (spellingPuzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "False")
                {
                    isItComplete = false;
                }

                var puzzleObj = new
                {
                    Type = "Spelling Puzzle",
                    Name = spellingPuzzlesJsonData["Puzzles"][i]["Name"].ToString(),
                    NameDutch = spellingPuzzlesJsonData["Puzzles"][i]["NameDutch"].ToString(),
                    Stars = int.Parse(spellingPuzzlesJsonData["Puzzles"][i]["Stars"].ToString()),
                    Completed = isItComplete
                };

                AllPuzzles.Add(puzzleObj);
            }
        }
    }
}
