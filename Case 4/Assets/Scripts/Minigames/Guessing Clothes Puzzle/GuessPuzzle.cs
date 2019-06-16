using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class GuessPuzzle : MonoBehaviour
{
    [Tooltip("Necessary to seperate the puzzles in storage!")]
    public string Name;
    [System.NonSerialized]
    public string NameDutch;
    [System.NonSerialized]
    public bool Completed;
    [Space(15)]
    public GameObject InstructionManual;
    [Tooltip("This checkbox either allows or disallows loading clothes from the editor by hand instead of loading the default ones.")]
    public bool Customizable;
    public int TimeToComplete;
    [Space(15)]
    public string QuestForObjective;
    public int ObjectiveToCompleteID;
    public string SceneName;
    public GameObject LoadingScreen;
    public List<string> MilestonesToComplete = new List<string>();

    #region Lists of custom clothes for gameplay instead of random ones by default
    [Header("If puzzle is customizable, use these lists of clothing instead.")]
    public List<GuessClothing> HeadList = new List<GuessClothing>();
    private int _currentHeadIndex = 0;
    public List<GuessClothing> TorsoList = new List<GuessClothing>();
    private int _currentTorsoIndex = 0;
    public List<GuessClothing> BottomList = new List<GuessClothing>();
    private int _currentBottomIndex = 0;
    public List<GuessClothing> ShoesList = new List<GuessClothing>();
    private int _currentShoesIndex = 0;
    #endregion

    #region End-game clothes and completion references
    [Header("These are the elements needed for the game to be finished successfully.")]
    [Space(15)]
    public Sprite CorrectHairElement;
    public Sprite CorrectTorsoElement;
    public Sprite CorrectBottomElement;
    public Sprite CorrectShoesElement;
    private float _correctClothes;
    [Space(15)]
    public Slider ProgressSlider;
    #endregion

    #region Body parts of the empty canvas body used for the game
    public GameObject _bodyPart;
    public GameObject _facePart;
    public GameObject _hairPart;
    public GameObject _topPart;
    public GameObject _botPart;
    public GameObject _shoesPart;
    #endregion

    #region Score references
    public TextMeshProUGUI finalScore;
    public TextMeshProUGUI ManualFinalScore;
    private int _stars;
    private string _defaultsJsonPath;
    private string _guessingPuzzlesPath;
    #endregion

    public GameObject PuzzleFinishedWindow;
    public GameObject ManuallyPuzzleFinishedWindow;
    private bool _isGameStarted = false;
    public TextMeshProUGUI Timer;
    private float _timer = 0;

    private Sprite _tempHairPart;
    private Sprite _tempTopPart;
    private Sprite _tempBotPart;
    private Sprite _tempShoesPart;

    void Awake()
    {
        _timer = TimeToComplete;
        _defaultsJsonPath = Application.persistentDataPath + "/GuessClothingDefaults.json";
        _guessingPuzzlesPath = Application.persistentDataPath + "/GuessingPuzzles.json";

        #region Loading the guessing clothes puzzle file data
        string dataToJson = File.ReadAllText(_guessingPuzzlesPath);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);
        
        for (int i = 0; i < puzzlesJsonData["Puzzles"].Count; i++)
        {
            if (puzzlesJsonData["Puzzles"][i]["Name"].ToString() == Name ||
                puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString() == NameDutch)
            {
                _stars = int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString());
                SceneName = puzzlesJsonData["Puzzles"][i]["SceneName"].ToString();
                if (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True")
                {
                    Completed = true;
                }
                else if (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "False")
                {
                    Completed = false;
                }
            }
        }
        #endregion

        #region Assigning body parts to the game objects
        /*if (SceneManager.GetActiveScene().name == "Main Map")
        {
            _bodyPart = gameObject.transform.GetChild(1).transform.GetChild(1).gameObject;
            _facePart = gameObject.transform.GetChild(1).transform.GetChild(2).gameObject;
            _hairPart = gameObject.transform.GetChild(1).transform.GetChild(5).gameObject;
            _topPart = gameObject.transform.GetChild(1).transform.GetChild(4).gameObject;
            _botPart = gameObject.transform.GetChild(1).transform.GetChild(3).gameObject;
            _shoesPart = gameObject.transform.GetChild(1).transform.GetChild(6).gameObject;
        } else
        {
            _bodyPart = gameObject.transform.GetChild(0).transform.GetChild(1).gameObject;
            _facePart = gameObject.transform.GetChild(0).transform.GetChild(2).gameObject;
            _hairPart = gameObject.transform.GetChild(0).transform.GetChild(5).gameObject;
            _topPart = gameObject.transform.GetChild(0).transform.GetChild(4).gameObject;
            _botPart = gameObject.transform.GetChild(0).transform.GetChild(3).gameObject;
            _shoesPart = gameObject.transform.GetChild(0).transform.GetChild(6).gameObject;
        }*/
        #endregion

        if (Customizable == false)
        {
            LoadDefaults();
        }
    }

    private void Start()
    {
        if (Character.Instance.TutorialCompleted == false)
        {
            InterfaceManager.Instance.OpenPopup(InstructionManual);
        }
        _isGameStarted = true;
        StartPuzzle();
    }

    private void Update()
    {
        if (_isGameStarted)
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
            }
            else
            {
                _timer = 0;
                SavePuzzleScore();
                SetMilestones();
                OpenCompletePuzzleWindow();
                Completed = true;
                _isGameStarted = false;
            }
            Timer.text = ((int)_timer).ToString();
        }
    }

    private void SetMilestones()
    {
        foreach (string milestoneToComplete in MilestonesToComplete)
        {
            foreach (ProgressEntry milestone in ProgressLog.Instance.Log)
            {
                if (milestoneToComplete == milestone.Milestone)
                {
/*                    Debug.Log(milestoneToComplete);
                    Debug.Log(milestone.Milestone);
                    Debug.Log(milestone.Completed);*/
                    ProgressLog.Instance.SetEntry(milestone.Milestone, true);
                }
            }
        }
    }

    public void ManuallyEndGame()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        Completed = true;
        SavePuzzleScore();
        SetMilestones();
        OpenManuallyCompletePuzzleWindow();
        _isGameStarted = false;
    }

    public void NextClothing(string bodyPart)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.CloseWindow);

        switch (bodyPart)
        {
            case "Hair":
                _currentHeadIndex++;
                // Error-handling the counter so the lists dont cause an error by trying
                // to access an index out of range.
                if (_currentHeadIndex <= 0)
                {
                    _currentHeadIndex = 0;
                }
                else if (_currentHeadIndex > HeadList.Count - 1)
                {
                    _currentHeadIndex = 0;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Top":
                _currentTorsoIndex++;
                if (_currentTorsoIndex <= 0)
                {
                    _currentTorsoIndex = 0;
                }
                else if (_currentTorsoIndex > TorsoList.Count - 1)
                {
                    _currentTorsoIndex = 0;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Bot":
                _currentBottomIndex++;
                if (_currentBottomIndex <= 0)
                {
                    _currentBottomIndex = 0;
                }
                else if (_currentBottomIndex > BottomList.Count - 1)
                {
                    _currentBottomIndex = 0;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Shoes":
                _currentShoesIndex++;
                if (_currentShoesIndex <= 0)
                {
                    _currentShoesIndex = 0;
                }
                else if (_currentShoesIndex > ShoesList.Count - 1)
                {
                    _currentShoesIndex = 0;
                }
                LoadNewClothing(bodyPart);
                break;
        }
    } 

    public void PreviousClothing(string bodyPart)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.CloseWindow);

        switch (bodyPart)
        {
            case "Hair":
                _currentHeadIndex--;
                if (_currentHeadIndex < 0)
                {
                    _currentHeadIndex = HeadList.Count - 1;
                }
                else if (_currentHeadIndex >= HeadList.Count - 1)
                {
                    _currentHeadIndex = HeadList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Top":
                _currentTorsoIndex--;
                if (_currentTorsoIndex < 0)
                {
                    _currentTorsoIndex = TorsoList.Count - 1;
                }
                else if (_currentTorsoIndex >= TorsoList.Count - 1)
                {
                    _currentTorsoIndex = TorsoList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Bot":
                _currentBottomIndex--;
                if (_currentBottomIndex < 0)
                {
                    _currentBottomIndex = BottomList.Count - 1;
                }
                else if (_currentBottomIndex >= BottomList.Count - 1)
                {
                    _currentBottomIndex = BottomList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Shoes":
                _currentShoesIndex--;
                if (_currentShoesIndex < 0)
                {
                    _currentShoesIndex = ShoesList.Count - 1;
                }
                else if (_currentShoesIndex >= ShoesList.Count - 1)
                {
                    _currentShoesIndex = ShoesList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
        }
    }

    public void LoadNewClothing(string bodyPart)
    {
        switch (bodyPart)
        {
            case "Hair":
                Sprite headSprite = Resources.Load<Sprite>("Clothing/New Clothing/" + HeadList[_currentHeadIndex].Sprite);
                _hairPart.GetComponent<Image>().overrideSprite = headSprite;
                _tempHairPart = headSprite;
                break;
            case "Top":
                Sprite torsoSprite = Resources.Load<Sprite>("Clothing/New Clothing/" + TorsoList[_currentTorsoIndex].Sprite);
                _topPart.GetComponent<Image>().overrideSprite = torsoSprite;
                _tempTopPart = torsoSprite;
                break;
            case "Bot":
                Sprite bottomSprite = Resources.Load<Sprite>("Clothing/New Clothing/" + BottomList[_currentBottomIndex].Sprite);
                _botPart.GetComponent<Image>().overrideSprite = bottomSprite;
                _tempBotPart = bottomSprite;
                break;
            case "Shoes":
                Sprite shoesSprite = Resources.Load<Sprite>("Clothing/New Clothing/" + ShoesList[_currentShoesIndex].Sprite);
                _shoesPart.GetComponent<Image>().overrideSprite = shoesSprite;
                _tempShoesPart = shoesSprite;
                break;
        }

        CheckIfClothingIsCorrect();
    }

    /// <summary>
    /// Checks whether the newly selected clothing element is mathicng the preset/correct clothing element
    /// </summary>
    private void CheckIfClothingIsCorrect()
    {
        _correctClothes = 0;
        if (_tempHairPart == CorrectHairElement)
        {
            _correctClothes++;
        }
        if (_tempTopPart == CorrectTorsoElement)
        {
            _correctClothes++;
        }
        if (_tempBotPart == CorrectBottomElement)
        {
            _correctClothes++;
        }
        if (_tempShoesPart == CorrectShoesElement)
        {
            _correctClothes++;
        }
        ProgressSlider.value = (_correctClothes * (100 / 4)) / 100;
    }

    private void LoadDefaults()
    {
        // This prevents the items from the editor which are accidentally placed
        // for a puzzle not meant to be customizable to clear the list in case it
        // keeps the old one.
        HeadList.Clear();
        TorsoList.Clear();
        BottomList.Clear();
        ShoesList.Clear();

        TextAsset puzzleData = Resources.Load<TextAsset>("Default World Data/GuessClothingDefaults");
        JsonData puzzlesJsonData = JsonMapper.ToObject(puzzleData.text);

        for (int i = 0; i < puzzlesJsonData["GuessingClothes"].Count; i++)
        {
            string sprite = puzzlesJsonData["GuessingClothes"][i]["Sprite"].ToString();
            int points = int.Parse(puzzlesJsonData["GuessingClothes"][i]["Points"].ToString());

            switch (puzzlesJsonData["GuessingClothes"][i]["BodyPart"].ToString())
            {
                case "Hair":
                    HeadList.Add(new GuessClothing(sprite, points));
                    break;
                case "Top":
                    TorsoList.Add(new GuessClothing(sprite, points));
                    break;
                case "Bot":
                    BottomList.Add(new GuessClothing(sprite, points));
                    break;
                case "Shoes":
                    ShoesList.Add(new GuessClothing(sprite, points));
                    break;
            }
        }
        RandomizeClothing();
    }

    
    private void RandomizeClothing()
    {
        RandomizeClothingList(HeadList);
        RandomizeClothingList(TorsoList);
        RandomizeClothingList(BottomList);
        RandomizeClothingList(ShoesList);
    }

    //The function takes a list of clothing elements and randomizes the order of those elements in a list
    private void RandomizeClothingList(List<GuessClothing> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GuessClothing temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void ToggleLoadingScreen()
    {
        LoadingScreen.SetActive(!LoadingScreen.activeSelf);
    }

    public void StartPuzzle()
    {
        ResetPuzzle();
        ToggleLoadingScreen();
        Invoke("ToggleLoadingScreen", 1f);


        _bodyPart.GetComponent<Canvas>().sortingOrder = 1;
        Sprite[]  spritesFromStorage = Resources.LoadAll<Sprite>("Clothing/New Clothing");
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == "Body" && clothing.Selected == true)
            {
                foreach (Sprite sprite in spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _bodyPart.GetComponent<Image>().overrideSprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Face" && clothing.Selected == true)
            {
                foreach (Sprite sprite in spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _facePart.GetComponent<Image>().overrideSprite = sprite;
                    }
                }
            }
        }
    }

    public void StartTimer()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        _isGameStarted = true;
    }

    private void SavePuzzleScore()
    {
        string newScoreData = "{";
        newScoreData += InsertNewLineTabs(1);
        newScoreData += "\"Puzzles\": [";

        string dataToJson = File.ReadAllText(_guessingPuzzlesPath);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);

        for (int i = 0; i < puzzlesJsonData["Puzzles"].Count; i++)
        {
            newScoreData += InsertNewLineTabs(2);
            newScoreData += "{";
            if (puzzlesJsonData["Puzzles"][i]["Name"].ToString() == Name ||
                puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString() == NameDutch)
            {
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Name\": " + "\"" + Name + "\",";
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"NameDutch\": " + "\"" + NameDutch + "\",";
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"SceneName\": " + "\"" + SceneName + "\",";
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Completed\": " + (Completed ? "true" : "false") + ",";
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Stars\": " + CalculateTotalScore();
            } else
            {
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Name\": " + "\"" + puzzlesJsonData["Puzzles"][i]["Name"].ToString() + "\",";
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"NameDutch\": " + "\"" + puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString() + "\",";
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"SceneName\": " + "\"" + SceneName + "\",";
                newScoreData += InsertNewLineTabs(3);
                if (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True")
                {
                    newScoreData += "\"Completed\": true,";
                }
                else if (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "False")
                {
                    newScoreData += "\"Completed\": false,";
                }
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Stars\": " + int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString());
            }
            newScoreData += InsertNewLineTabs(2);
            newScoreData += "},";
        }

        newScoreData = newScoreData.Substring(0, newScoreData.Length - 1);
        newScoreData += InsertNewLineTabs(1) + "]" + System.Environment.NewLine + "}";
        File.WriteAllText(_guessingPuzzlesPath, newScoreData);

        //TODO: Use the progress log list of entities
        // to refresh their hierarchy activation status

        Character.Instance.CompleteObjectiveInQuest(ObjectiveToCompleteID, QuestForObjective);

        AudioManager.Instance.PlaySound(AudioManager.Instance.SoundPuzzleCompleted);
    }

    // Returns the score based on the clothing currently on the character.
    private int CalculateTotalScore()
    {
        int score =
               HeadList[_currentHeadIndex].Points +
               TorsoList[_currentTorsoIndex].Points +
               BottomList[_currentBottomIndex].Points +
               ShoesList[_currentShoesIndex].Points;

        if (score > 50)
        {
            return 3;
        } else
        {
            return 1;
        }
    }

    public void ResetPuzzle()
    {
        _currentHeadIndex = 0;
        _currentTorsoIndex = 0;
        _currentBottomIndex = 0;
        _currentShoesIndex = 0;
        _timer = TimeToComplete;

        LoadNewClothing("Hair");
        LoadNewClothing("Top");
        LoadNewClothing("Bot");
        LoadNewClothing("Shoes");
    }

    public void ClosePopup(Object obj)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(false);
    }

    public void OpenPopup(Object obj)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(true);
    }

    private void OpenCompletePuzzleWindow()
    {
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                finalScore.text = "Time is up. You have successfully completed this puzzle!";
                break;
            case "Dutch":
                finalScore.text = "De tijd is om!. Je bent er in geslaagd de puzzel te voltooien!";
                break;
        }
        
        OpenPopup(PuzzleFinishedWindow);
    }
    private void OpenManuallyCompletePuzzleWindow()
    {
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                ManualFinalScore.text = "You have successfully completed this puzzle!";
                break;
            case "Dutch":
                ManualFinalScore.text = "Je bent er in geslaagd de puzzel te voltooien!";
                break;
        }

        OpenPopup(ManuallyPuzzleFinishedWindow);
    }
    private string InsertNewLineTabs(int numberOfTabs)
    {
        string whiteSpace = System.Environment.NewLine;
        for (int i = 0; i < numberOfTabs; i++)
        {
            whiteSpace += "\t";
        }

        return whiteSpace;
    }
}
