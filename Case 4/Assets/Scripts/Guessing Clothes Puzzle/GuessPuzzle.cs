using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class GuessPuzzle : MonoBehaviour
{
    public Puzzle CanvasElementOfPuzzle;
    [Tooltip("Necessary to seperate the puzzles in storage!")]
    public string Name;
    [Tooltip("This checkbox either allows or disallows loading clothes from the editor by hand instead of loading the default ones.")]
    public bool Customizable;
    public int PuzzleTimeInSeconds;
    public string QuestForObjective;
    public int ObjectiveToCompleteID;
    public List<GameObject> EntitiesToActivate = new List<GameObject>();
    [Header("If puzzle is customizable, use these lists of clothing instead.")]
    public List<GuessClothing> HeadList = new List<GuessClothing>();
    private int _currentHeadIndex = 0;
    public List<GuessClothing> TorsoList = new List<GuessClothing>();
    private int _currentTorsoIndex = 0;
    public List<GuessClothing> BottomList = new List<GuessClothing>();
    private int _currentBottomIndex = 0;
    public List<GuessClothing> ShoesList = new List<GuessClothing>();
    private int _currentShoesIndex = 0;

    private GameObject _bodyPart;
    private GameObject _facePart;
    private GameObject _hairPart;
    private GameObject _topPart;
    private GameObject _botPart;
    private GameObject _shoesPart;

    public TextMeshProUGUI finalScore;

    public int TotalScore;
    private string _defaultsJsonPath;
    private string _guessingPuzzlesPath;

    public GameObject PuzzleFinishedWindow;
    private bool _isGameStarted = false;
    public TextMeshProUGUI Timer;
    private float _timer = 0;
    private SwipeController _swipeController;
    private Puzzle _puzzle;

    private CameraBehavior _cameraBehaviour;
    private AudioManager _audioManager;

    void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _cameraBehaviour = FindObjectOfType<CameraBehavior>();
        _swipeController = FindObjectOfType<SwipeController>();
        _timer = PuzzleTimeInSeconds;
        _defaultsJsonPath = Application.persistentDataPath + "/GuessClothingDefaults.json";
        _guessingPuzzlesPath = Application.persistentDataPath + "/GuessingPuzzles.json";

        #region Loading the guessing clothes puzzle file data
        string dataToJson = File.ReadAllText(_guessingPuzzlesPath);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);
        
        for (int i = 0; i < puzzlesJsonData["GuessingPuzzles"].Count; i++)
        {
            if (puzzlesJsonData["GuessingPuzzles"][i]["Name"].ToString() == Name)
            {
                TotalScore = int.Parse(puzzlesJsonData["GuessingPuzzles"][i]["Score"].ToString());
            }
        }
        #endregion

        #region Assigning body parts to the game objects
        if (SceneManager.GetActiveScene().name == "Main Map")
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
        }
        #endregion

        if (Customizable == false)
        {
            LoadDefaults();
        }
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
                OpenCompletePuzzleWindow();
                _cameraBehaviour.IsInteracting = false;
                _isGameStarted = false;
            }
            Timer.text = ((int)_timer).ToString();
        }
    }

    public void ManuallyEndGame()
    {
        _audioManager.PlaySound(_audioManager.ButtonPress);

        SavePuzzleScore();
        OpenCompletePuzzleWindow();
        _cameraBehaviour.IsInteracting = false;
        _isGameStarted = false;
    }

    public void NextClothing(string bodyPart)
    {
        _audioManager.PlaySound(_audioManager.CloseWindow);

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
                else if (_currentHeadIndex >= HeadList.Count - 1)
                {
                    _currentHeadIndex = HeadList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Top":
                _currentTorsoIndex++;
                if (_currentTorsoIndex <= 0)
                {
                    _currentTorsoIndex = 0;
                }
                else if (_currentTorsoIndex >= TorsoList.Count - 1)
                {
                    _currentTorsoIndex = TorsoList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Bot":
                _currentBottomIndex++;
                if (_currentBottomIndex <= 0)
                {
                    _currentBottomIndex = 0;
                }
                else if (_currentBottomIndex >= BottomList.Count - 1)
                {
                    _currentBottomIndex = BottomList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Shoes":
                _currentShoesIndex++;
                if (_currentShoesIndex <= 0)
                {
                    _currentShoesIndex = 0;
                }
                else if (_currentShoesIndex >= ShoesList.Count - 1)
                {
                    _currentShoesIndex = ShoesList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
        }
    } 

    public void PreviousClothing(string bodyPart)
    {
        _audioManager.PlaySound(_audioManager.CloseWindow);

        switch (bodyPart)
        {
            case "Hair":
                _currentHeadIndex--;
                if (_currentHeadIndex <= 0)
                {
                    _currentHeadIndex = 0;
                }
                else if (_currentHeadIndex >= HeadList.Count - 1)
                {
                    _currentHeadIndex = HeadList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Top":
                _currentTorsoIndex--;
                if (_currentTorsoIndex <= 0)
                {
                    _currentTorsoIndex = 0;
                }
                else if (_currentTorsoIndex >= TorsoList.Count - 1)
                {
                    _currentTorsoIndex = TorsoList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Bot":
                _currentBottomIndex--;
                if (_currentBottomIndex <= 0)
                {
                    _currentBottomIndex = 0;
                }
                else if (_currentBottomIndex >= BottomList.Count - 1)
                {
                    _currentBottomIndex = BottomList.Count - 1;
                }
                LoadNewClothing(bodyPart);
                break;
            case "Shoes":
                _currentShoesIndex--;
                if (_currentShoesIndex <= 0)
                {
                    _currentShoesIndex = 0;
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
                Sprite headSprite = Resources.Load<Sprite>("Guessing Clothing/" + HeadList[_currentHeadIndex].Sprite);
                _hairPart.GetComponent<Image>().sprite = headSprite;
                break;
            case "Top":
                Sprite torsoSprite = Resources.Load<Sprite>("Guessing Clothing/" + TorsoList[_currentTorsoIndex].Sprite);
                _topPart.GetComponent<Image>().sprite = torsoSprite;
                break;
            case "Bot":
                Sprite bottomSprite = Resources.Load<Sprite>("Guessing Clothing/" + BottomList[_currentBottomIndex].Sprite);
                _botPart.GetComponent<Image>().sprite = bottomSprite;
                break;
            case "Shoes":
                Sprite shoesSprite = Resources.Load<Sprite>("Guessing Clothing/" + ShoesList[_currentShoesIndex].Sprite);
                _shoesPart.GetComponent<Image>().sprite = shoesSprite;
                break;
        }
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
    }

    public void StartPuzzle(GameObject puzzle)
    {
        ResetPuzzle();

        Sprite[]  spritesFromStorage = Resources.LoadAll<Sprite>("Clothing/New Clothing");
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == "Body" && clothing.Selected == true)
            {
                foreach (Sprite sprite in spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _bodyPart.GetComponent<Image>().sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Face" && clothing.Selected == true)
            {
                foreach (Sprite sprite in spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _facePart.GetComponent<Image>().sprite = sprite;
                    }
                }
            }
        }

        puzzle.SetActive(true);
    }

    public void StartTimer()
    {
        _audioManager.PlaySound(_audioManager.ButtonPress);

        _isGameStarted = true;
    }

    private void SavePuzzleScore()
    {
        string newScoreData = "{\"GuessingPuzzles\": [";

        string dataToJson = File.ReadAllText(_guessingPuzzlesPath);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);

        for (int i = 0; i < puzzlesJsonData["GuessingPuzzles"].Count; i++)
        {
            newScoreData += "{";
            if (puzzlesJsonData["GuessingPuzzles"][i]["Name"].ToString() == Name)
            {
                newScoreData +=
                    "\"Name\":" + "\"" + Name + "\"," +
                    "\"Score\":" + CalculateTotalScore();
            } else
            {
                newScoreData += "\"Name\":" + "\"" + puzzlesJsonData["GuessingPuzzles"][i]["Name"].ToString() + "\",";
                newScoreData += "\"Score\":" + int.Parse(puzzlesJsonData["GuessingPuzzles"][i]["Score"].ToString());
            }
            newScoreData += "},";
        }

        newScoreData = newScoreData.Substring(0, newScoreData.Length - 1);
        newScoreData += "]}";
        File.WriteAllText(_guessingPuzzlesPath, newScoreData);

        foreach (GameObject objToActivate in EntitiesToActivate)
        {
            objToActivate.SetActive(true);
        }

        Character.Instance.CompleteObjectiveInQuest(ObjectiveToCompleteID, QuestForObjective);

        _audioManager.PlaySound(_audioManager.SoundPuzzleCompleted);

        CanvasElementOfPuzzle.LoadScore();
    }

    // Returns the score based on the clothing currently on the character.
    private int CalculateTotalScore()
    {
        return TotalScore =
            HeadList[_currentHeadIndex].Points +
            TorsoList[_currentTorsoIndex].Points +
            BottomList[_currentBottomIndex].Points +
            ShoesList[_currentShoesIndex].Points;
    }

    public void ResetPuzzle()
    {
        _currentHeadIndex = 0;
        _currentTorsoIndex = 0;
        _currentBottomIndex = 0;
        _currentShoesIndex = 0;
        _timer = PuzzleTimeInSeconds;

        LoadNewClothing("Hair");
        LoadNewClothing("Top");
        LoadNewClothing("Bot");
        LoadNewClothing("Shoes");
    }

    public void ClosePopup(Object obj)
    {
        _audioManager.PlaySound(_audioManager.ButtonPress);

        GameObject popupObject = obj as GameObject;

        if (popupObject == gameObject)
        {
            Character.Instance.InitiateInteraction();
        }

        popupObject.SetActive(false);

        Character.Instance.InitiateInteraction();
    }

    public void OpenPopup(Object obj)
    {
        _audioManager.PlaySound(_audioManager.ButtonPress);

        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(true);
    }

    public void ToggleIcons()
    {
        CanvasElementOfPuzzle.FinishPuzzleIconToggle();
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

        if (_swipeController != null)
        {
            _swipeController.enabled = true;
        }
        OpenPopup(PuzzleFinishedWindow);
    }
}
