using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellingPuzzle : MonoBehaviour
{
    public string Name;
    public string NameDutch;
    public bool Completed;
    public int Stars;
    public string SceneName;
    [Space(15)]
    public Image LetterBackgroud;
    public GameObject LetterFieldsEnglish;
    public GameObject LetterFieldsDutch;
    public TextMeshProUGUI LetterTextEnglish;
    public TextMeshProUGUI LetterTextDutch;
    public TMP_FontAsset LetterFont;
    [Space(15)]
    public GameObject InstructionManual;
    [Header("Drag & Drop the fields in the letter, here, that will be used for the puzzle!")]
    public List<WordField> FieldsOfWords = new List<WordField>();
    [Space(15)]
    public GameObject OptionsMenu;
    public GameObject PuzzleEndWindow;
    public List<GameObject> StarsList;
    public List<string> MilestonesToComplete = new List<string>();
    // Counts how many times the user has selected
    // the wrong word for the corresponding field.
    private int _numberOfMistakes;
    private GameObject _selectedField;
    private GameObject _selectedOptions;

    // Data in storage functionality related references
    private List<SpellingPuzzleObject> _puzzles = new List<SpellingPuzzleObject>();
    private string _pathToPuzzles;

    private void Awake()
    {
        _pathToPuzzles = Application.persistentDataPath + "/SpellingPuzzles.json";

        if (!File.Exists(_pathToPuzzles))
        {
            TextAsset SpellingPuzzleObjectsData = Resources.Load<TextAsset>("Default World Data/SpellingPuzzles");
            File.WriteAllText(_pathToPuzzles, SpellingPuzzleObjectsData.text);
        }
    }

    private void Start()
    {
        SetupData();

        // Changing the font proves to be very annoying as we will need fields
        // that compensate not just for different text languages, but font as
        // well, so I am leaving it at one font only for now.
        //LetterTextEnglish.font = Resources.Load<TMP_FontAsset>("Fonts/" + LetterFont.ToString() + " SDF");
        //LetterTextDutch.font = Resources.Load<TMP_FontAsset>("Fonts/" + LetterFont.ToString() + " SDF");

        switch (SettingsManager.Instance.Language)
        {
            case "English":
                LetterTextEnglish.gameObject.SetActive(true);
                LetterFieldsEnglish.SetActive(true);

                for (int i = 0; i < FieldsOfWords.Count; i++)
                {
                    OptionsMenu.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = FieldsOfWords[i].ExpectedWordEnglish;
                    //FieldsOfWords[i].TextFieldEnglish.GetComponentInChildren<WordToFind>().ExpectedWord = FieldsOfWords[i].ExpectedWordEnglish;
                }
                break;
            case "Dutch":
                LetterTextDutch.gameObject.SetActive(true);
                LetterFieldsDutch.SetActive(true);

                for (int i = 0; i < FieldsOfWords.Count; i++)
                {
                    OptionsMenu.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = FieldsOfWords[i].ExpectedWordDutch;
                    //FieldsOfWords[i].TextFieldDutch.GetComponentInChildren<WordToFind>().ExpectedWord = FieldsOfWords[i].ExpectedWordDutch;
                }
                break;
        }
    }

    public void ToggleOptionsMenu(UnityEngine.Object button)
    {
        GameObject buttonObj = (GameObject)button;

        bool openMenu = buttonObj.transform.parent.transform.parent.name == "Options Menu" ? true : false;

        // If it's false then a button was picked to fill in the blank field selected.
        if (openMenu)
        {
            _selectedField.GetComponentInChildren<WordToFind>().SelectedWord = buttonObj.GetComponentInChildren<TextMeshProUGUI>().text;

            _selectedField.GetComponentInChildren<TextMeshProUGUI>().text = buttonObj.GetComponentInChildren<TextMeshProUGUI>().text.Substring(0, buttonObj.GetComponentInChildren<TextMeshProUGUI>().text.Length > 10 ? 11 : buttonObj.GetComponentInChildren<TextMeshProUGUI>().text.Length);

            _selectedOptions.SetActive(false);

            _selectedOptions = null;
            OptionsMenu.SetActive(false);
        } else
        {
            if (_selectedField != null && _selectedOptions != null)
            {
                _selectedOptions.SetActive(false);
            }

            _selectedField = buttonObj;
            OptionsMenu.SetActive(true);

            for (int i = 0; i < FieldsOfWords.Count; i++)
            {
                if (_selectedField.GetComponentInChildren<WordToFind>().ExpectedWord == FieldsOfWords[i].ExpectedWordEnglish || _selectedField.GetComponentInChildren<WordToFind>().ExpectedWord == FieldsOfWords[i].ExpectedWordDutch)
                {
                    FieldsOfWords[i].Options.SetActive(true);
                    _selectedOptions = FieldsOfWords[i].Options;
                }
            }
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
                    ProgressLog.Instance.SetEntry(milestone.Milestone, true);
                }
            }
        }
    }

    public void CheckForCompletion()
    {
        bool isPuzzleCompleted = true;
        for (int i = 0; i < FieldsOfWords.Count; i++)
        {
            if (FieldsOfWords[i].TextFieldEnglish.GetComponentInChildren<WordToFind>().SelectedWord != FieldsOfWords[i].ExpectedWordEnglish && FieldsOfWords[i].TextFieldDutch.GetComponentInChildren<WordToFind>().SelectedWord != FieldsOfWords[i].ExpectedWordDutch)
            {
                isPuzzleCompleted = false;
            }
        }
        
        if (isPuzzleCompleted == false)
        {
            InstructionManual.SetActive(true);
            _numberOfMistakes++;
        } else
        {
            // TODO:
            // Depending on the performance give different number of stars!
            Stars = 3;

            foreach (SpellingPuzzleObject puzzle in _puzzles)
            {
                if (puzzle.Name == Name)
                {
                    puzzle.Stars = Stars;
                    puzzle.Completed = true;
                }
            }

            PuzzleEndWindow.SetActive(true);
            StartCoroutine(ShowStars());
            SetMilestones();
            RefreshData();
        }
    }

    private void SetupData()
    {
        string puzzlesData = File.ReadAllText(_pathToPuzzles);
        JsonData puzzlesJsonData = JsonMapper.ToObject(puzzlesData);

        for (int i = 0; i < puzzlesJsonData["Puzzles"].Count; i++)
        {
            SpellingPuzzleObject puzzle = new SpellingPuzzleObject(
                    puzzlesJsonData["Puzzles"][i]["Name"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["SceneName"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false,
                    int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString()));

            _puzzles.Add(
                new SpellingPuzzleObject(
                    puzzlesJsonData["Puzzles"][i]["Name"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["SceneName"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false,
                    int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString())));

            if (Name == puzzlesJsonData["Puzzles"][i]["Name"].ToString() ||
                NameDutch == puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString())
            {
                Completed = puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false;
                Stars = int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString());
            }
        }
    }

    private void RefreshData()
    {
        string newJsonData = "{";
        newJsonData += InsertNewLineTabs(1);
        newJsonData += "\"Puzzles\": [";

        foreach (SpellingPuzzleObject puzzle in _puzzles)
        {
            newJsonData += InsertNewLineTabs(2);
            newJsonData += "{";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Name\": \"" + puzzle.Name + "\",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"NameDutch\": \"" + puzzle.NameDutch + "\",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"SceneName\": \"" + puzzle.SceneName + "\",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Completed\": " + (puzzle.Completed == true ? "true" : "false") + ",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Stars\": " + puzzle.Stars + "";
            newJsonData += InsertNewLineTabs(2);
            newJsonData += "},";
        }
        newJsonData = newJsonData.Substring(0, newJsonData.Length - 1);
        newJsonData += InsertNewLineTabs(1);
        newJsonData += "]";

        newJsonData += Environment.NewLine + "}";
        File.WriteAllText(_pathToPuzzles, newJsonData);
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

    private IEnumerator ShowStars()
    {
        for (int i = 0; i < StarsList.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            StarsList[i].SetActive(true);
        }
    }
}
