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

    public Sprite CorrectHairElement;
    public Sprite CorrectTorsoElement;
    public Sprite CorrectBottomElement;
    public Sprite CorrectShoesElement;
    private float _correctClothes;
    public Slider ProgressSlider;

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
    private Puzzle _puzzle;

    private CameraBehavior _cameraBehaviour;

    void Awake()
    {
        _cameraBehaviour = FindObjectOfType<CameraBehavior>();
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        SavePuzzleScore();
        OpenCompletePuzzleWindow();
        _cameraBehaviour.IsInteracting = false;
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
        Sprite newClothingElementSprite;
        switch (bodyPart)
        {
            case "Hair":
                Sprite headSprite = Resources.Load<Sprite>("Clothing/New Clothing/" + HeadList[_currentHeadIndex].Sprite);
                _hairPart.GetComponent<Image>().sprite = headSprite;
                newClothingElementSprite = headSprite;
                break;
            case "Top":
                Sprite torsoSprite = Resources.Load<Sprite>("Clothing/New Clothing/" + TorsoList[_currentTorsoIndex].Sprite);
                _topPart.GetComponent<Image>().sprite = torsoSprite;
                newClothingElementSprite = torsoSprite;
                break;
            case "Bot":
                Sprite bottomSprite = Resources.Load<Sprite>("Clothing/New Clothing/" + BottomList[_currentBottomIndex].Sprite);
                _botPart.GetComponent<Image>().sprite = bottomSprite;
                newClothingElementSprite = bottomSprite;
                break;
            case "Shoes":
                Sprite shoesSprite = Resources.Load<Sprite>("Clothing/New Clothing/" + ShoesList[_currentShoesIndex].Sprite);
                _shoesPart.GetComponent<Image>().sprite = shoesSprite;
                newClothingElementSprite = shoesSprite;
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
        if (_hairPart.GetComponent<Image>().sprite == CorrectHairElement)
        {
            _correctClothes++;
        }
        if (_topPart.GetComponent<Image>().sprite == CorrectTorsoElement)
        {
            _correctClothes++;
        }
        if (_botPart.GetComponent<Image>().sprite == CorrectBottomElement)
        {
            _correctClothes++;
        }
        if (_shoesPart.GetComponent<Image>().sprite == CorrectShoesElement)
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        _isGameStarted = true;
    }

    private void SavePuzzleScore()
    {
        string newScoreData = "{";
        newScoreData += InsertNewLineTabs(1);
        newScoreData += "\"GuessingPuzzles\": [";

        string dataToJson = File.ReadAllText(_guessingPuzzlesPath);
        JsonData puzzlesJsonData = JsonMapper.ToObject(dataToJson);

        for (int i = 0; i < puzzlesJsonData["GuessingPuzzles"].Count; i++)
        {
            newScoreData += InsertNewLineTabs(2);
            newScoreData += "{";
            if (puzzlesJsonData["GuessingPuzzles"][i]["Name"].ToString() == Name)
            {
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Name\": " + "\"" + Name + "\",";
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Score\": " + CalculateTotalScore();
            } else
            {
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Name\": " + "\"" + puzzlesJsonData["GuessingPuzzles"][i]["Name"].ToString() + "\",";
                newScoreData += InsertNewLineTabs(3);
                newScoreData += "\"Score\": " + int.Parse(puzzlesJsonData["GuessingPuzzles"][i]["Score"].ToString());
            }
            newScoreData += InsertNewLineTabs(2);
            newScoreData += "},";
        }

        newScoreData = newScoreData.Substring(0, newScoreData.Length - 1);
        newScoreData += InsertNewLineTabs(1) + "]" + System.Environment.NewLine + "}";
        File.WriteAllText(_guessingPuzzlesPath, newScoreData);

        foreach (GameObject objToActivate in EntitiesToActivate)
        {
            if (objToActivate != null)
            {
                objToActivate.SetActive(true);
            }
        }

        Character.Instance.CompleteObjectiveInQuest(ObjectiveToCompleteID, QuestForObjective);

        AudioManager.Instance.PlaySound(AudioManager.Instance.SoundPuzzleCompleted);

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
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

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
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

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
        
        OpenPopup(PuzzleFinishedWindow);
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
