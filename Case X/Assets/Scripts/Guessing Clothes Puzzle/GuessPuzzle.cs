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
    [Tooltip("This checkbox either allows or disallows loading clothes from the editor by hand instead of loading the default ones.")]
    public bool Customizable;
    public int PuzzleTimeInSeconds;
    [Header("If puzzle is customizable, use these lists of clothing instead.")]
    public List<GuessClothing> HeadList = new List<GuessClothing>();
    private int _currentHeadIndex = 0;
    public List<GuessClothing> TorsoList = new List<GuessClothing>();
    private int _currentTorsoIndex = 0;
    public List<GuessClothing> BottomList = new List<GuessClothing>();
    private int _currentBottomIndex = 0;
    public List<GuessClothing> ShoesList = new List<GuessClothing>();
    private int _currentShoesIndex = 0;

    private GameObject _headPart;
    private GameObject _torsoPart;
    private GameObject _bottomPart;
    private GameObject _shoesPart;

    public TextMeshProUGUI finalScore;

    public int TotalScore;
    private string _defaultsJsonPath;
    private string _guessingPuzzlesPath;

    public GameObject PuzzleFinishedWindow;
    private bool _isGameStarted = false;
    public TextMeshProUGUI Timer;
    private float _timer = 0;

    void Awake()
    {
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
            _headPart = gameObject.transform.GetChild(1).transform.GetChild(0).gameObject;
            _torsoPart = gameObject.transform.GetChild(1).transform.GetChild(1).gameObject;
            _bottomPart = gameObject.transform.GetChild(1).transform.GetChild(2).gameObject;
            _shoesPart = gameObject.transform.GetChild(1).transform.GetChild(3).gameObject;
        } else
        {
            _headPart = gameObject.transform.GetChild(0).transform.GetChild(0).gameObject;
            _torsoPart = gameObject.transform.GetChild(0).transform.GetChild(1).gameObject;
            _bottomPart = gameObject.transform.GetChild(0).transform.GetChild(2).gameObject;
            _shoesPart = gameObject.transform.GetChild(0).transform.GetChild(3).gameObject;
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
                OpenCompletePuzzleWindow();
                SavePuzzleScore();
                _isGameStarted = false;
            }
            Timer.text = ((int)_timer).ToString();
        }
    }

    public void NextClothing(string bodyPart)
    {
        switch (bodyPart)
        {
            case "Head":
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
            case "Torso":
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
            case "Bottom":
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
        switch (bodyPart)
        {
            case "Head":
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
            case "Torso":
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
            case "Bottom":
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
            case "Head":
                Sprite headSprite = Resources.Load<Sprite>("Guessing Clothing/" + HeadList[_currentHeadIndex].Sprite);
                _headPart.GetComponent<Image>().sprite = headSprite;
                break;
            case "Torso":
                Sprite torsoSprite = Resources.Load<Sprite>("Guessing Clothing/" + TorsoList[_currentTorsoIndex].Sprite);
                _torsoPart.GetComponent<Image>().sprite = torsoSprite;
                break;
            case "Bottom":
                Sprite bottomSprite = Resources.Load<Sprite>("Guessing Clothing/" + BottomList[_currentBottomIndex].Sprite);
                _bottomPart.GetComponent<Image>().sprite = bottomSprite;
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
                case "Head":
                    HeadList.Add(new GuessClothing(sprite, points));
                    break;
                case "Torso":
                    TorsoList.Add(new GuessClothing(sprite, points));
                    break;
                case "Bottom":
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
        puzzle.SetActive(true);
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

        LoadNewClothing("Head");
        LoadNewClothing("Torso");
        LoadNewClothing("Bottom");
        LoadNewClothing("Shoes");
    }

    public void ClosePopup(Object obj)
    {
        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(false);
    }

    public void OpenPopup(Object obj)
    {
        GameObject popupObject = obj as GameObject;
        popupObject.SetActive(true);
    }

    private void OpenCompletePuzzleWindow()
    {

        finalScore.text = "Time is up. Your final score is " +
            TotalScore + ". Congratulations!";
        OpenPopup(PuzzleFinishedWindow);
    }
}
