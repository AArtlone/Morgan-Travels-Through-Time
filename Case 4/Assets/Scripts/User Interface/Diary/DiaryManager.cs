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

    public void SelectPage(Object selectedButton)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.NewPageInDiary);

        GameObject buttonObj = (GameObject)selectedButton;
        
        foreach (GameObject button in _buttonsForPages)
        {
            if (button == buttonObj)
            {
                button.SetActive(true);
                button.GetComponentInChildren<Image>().color = new Color(255, 255, 255, 1);

                foreach (GameObject page in _pages)
                {
                    if (button.name == "Quest Button")
                    {
                        if (page.name == "Quests Page")
                        {
                            page.SetActive(true);
                        } else
                        {
                            page.SetActive(false);
                        }
                    } else if (button.name == "Blueprints Button")
                    {
                        if (page.name == "Blueprints Page")
                        {
                            page.SetActive(true);
                        }
                        else
                        {
                            page.SetActive(false);
                        }
                    } else if (button.name == "Help Button")
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
            } else
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
        } else if (direction == "Forward" || direction == "Forwards")
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
        for (int i = startRange - 1; i < endRange; i++)
        {
            //if (AllPuzzles[i].Completed == true)
            //{
                GameObject newPuzzleDisplay = null;
                if (AllPuzzles[i].Type == "Hidden Objects Puzzle")
                {
                    newPuzzleDisplay = Instantiate(PuzzleDiaryDisplay, GridOfPuzzles.transform);

                    for (int j = 0; j < AllPuzzles[j].Stars; j++)
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
        //}
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
        JsonData HOPPuzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(Application.persistentDataPath + "/Puzzles.json"));
        JsonData GuessClothingPuzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(Application.persistentDataPath + "/GuessingPuzzles.json"));
        JsonData EscapeGamesJsonData = JsonMapper.ToObject(File.ReadAllText(Application.persistentDataPath + "/EscapeGames.json"));

        _numberOfPuzzles += HOPPuzzlesJsonData.Count + GuessClothingPuzzlesJsonData.Count + EscapeGamesJsonData.Count;

        // Put all the puzzles in a list.
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

        for (int i = 0; i < GuessClothingPuzzlesJsonData["GuessingPuzzles"].Count; i++)
        {
            bool isItComplete = false;
            if (GuessClothingPuzzlesJsonData["GuessingPuzzles"][i]["Completed"].ToString() == "True")
            {
                isItComplete = true;
            }
            else if (GuessClothingPuzzlesJsonData["GuessingPuzzles"][i]["Completed"].ToString() == "False")
            {
                isItComplete = false;
            }

            var puzzleObj = new
            {
                Type = "Guess Clothing Puzzle",
                Name = GuessClothingPuzzlesJsonData["GuessingPuzzles"][i]["Name"].ToString(),
                NameDutch = GuessClothingPuzzlesJsonData["GuessingPuzzles"][i]["NameDutch"].ToString(),
                Completed = isItComplete,
                Score = int.Parse(GuessClothingPuzzlesJsonData["GuessingPuzzles"][i]["Score"].ToString())
            };

            AllPuzzles.Add(puzzleObj);
        }

        for (int i = 0; i < EscapeGamesJsonData["EscapeGames"].Count; i++)
        {
            bool isItComplete = false;
            if (EscapeGamesJsonData["EscapeGames"][i]["Completed"].ToString() == "True")
            {
                isItComplete = true;
            }
            else if (EscapeGamesJsonData["EscapeGames"][i]["Completed"].ToString() == "False")
            {
                isItComplete = false;
            }

            var puzzleObj = new
            {
                Type = "Escape Game",
                Name = EscapeGamesJsonData["EscapeGames"][i]["Name"].ToString(),
                NameDutch = EscapeGamesJsonData["EscapeGames"][i]["NameDutch"].ToString(),
                Score = int.Parse(EscapeGamesJsonData["EscapeGames"][i]["TotalPoints"].ToString()),
                Completed = isItComplete,
                RefugeesSaved = int.Parse(EscapeGamesJsonData["EscapeGames"][i]["RefugeesSaved"].ToString())
            };

            AllPuzzles.Add(puzzleObj);
        }
    }
}
