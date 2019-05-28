using LitJson;
using System;
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
    public Image LetterBackgroud;
    public TextMeshProUGUI LetterText;
    public TMP_FontAsset LetterFont;
    public GameObject InstructionManual;
    [Header("Drag & Drop the fields in the letter, here, that will be used for the puzzle!")]
    public List<WordField> FieldsOfWords = new List<WordField>();
    public GameObject OptionsMenu;
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

        LetterText.font = Resources.Load<TMP_FontAsset>("Fonts/" + LetterFont.ToString() + " SDF");

        for (int i = 0; i < FieldsOfWords.Count; i++)
        {
            OptionsMenu.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = FieldsOfWords[i].ExpectedWord;
            FieldsOfWords[i].TextField.GetComponentInChildren<WordToFind>().ExpectedWord = FieldsOfWords[i].ExpectedWord;
        }
    }

    public void ToggleOptionsMenu(UnityEngine.Object button)
    {
        GameObject buttonObj = (GameObject)button;

        bool openMenu = buttonObj.transform.parent.transform.parent.name == "Options Menu" ? true : false;

        // If it's false then a button was picked to fill in the blank field selected.
        if (openMenu)
        {
            _selectedField.GetComponentInChildren<TextMeshProUGUI>().text = buttonObj.GetComponentInChildren<TextMeshProUGUI>().text;

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
                if (_selectedField.GetComponentInChildren<WordToFind>().ExpectedWord == FieldsOfWords[i].ExpectedWord)
                {
                    FieldsOfWords[i].Options.SetActive(true);
                    _selectedOptions = FieldsOfWords[i].Options;
                }
            }
        }
    }

    public void CheckForCompletion()
    {
        bool isPuzzleCompleted = true;
        for (int i = 0; i < FieldsOfWords.Count; i++)
        {
            if (FieldsOfWords[i].TextField.GetComponentInChildren<TextMeshProUGUI>().text != FieldsOfWords[i].ExpectedWord)
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
                    puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false,
                    int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString()));

            _puzzles.Add(
                new SpellingPuzzleObject(
                    puzzlesJsonData["Puzzles"][i]["Name"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString(),
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
}
