using LitJson;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    #region Character creation references
    public GameObject CharacterCompletionPopup;
    public GameObject CharacterCreationMenu;
    public GameObject CharacterNameMenu;
    public GameObject CharacterNamePopupWindow;
    public GameObject CharacterNameErrorPopupWindow;

    private string _jsonWordsFilter;
    private List<string> _wordsFilter = new List<string>();
    #endregion

    #region Diary and cases interface references
    [Space(10)]
    public GameObject CaseButtonPrefab;
    public GameObject CurrentCasesDisplay;
    public GameObject AvailableCasesDisplay;
    public GameObject CompletedCasesDisplay;
    [Space(10)]
    public GameObject CaseObjectivePrefab;
    public GameObject SelectedCaseTitle;
    public GameObject SelectedCaseDescription;
    public GameObject SelectedCaseStatus;
    public GameObject SelectedCaseObjectivesDisplay;
    private Case _currentlySelectedCase;
    #endregion

    #region Gameplay start references
    [Space(10)]
    public GameObject StartMenu;
    #endregion

    #region Item details for bottom UI inventory
    [Space(10)]
    public GameObject ItemDetailsWindow;
    public Image ItemDetailsPortrait;
    public Text ItemDetailsName;
    public Text ItemDetailsDescription;
    public Text ItemDetailsActives;
    #endregion

    #region Area references
    [Space(10)]
    public GameObject AreaLockedErrorPopup;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        _jsonWordsFilter = Application.persistentDataPath + "/BadWords.json";

        string filterWordsToJson = File.ReadAllText(_jsonWordsFilter);
        JsonData filterWordsData = JsonMapper.ToObject(filterWordsToJson);

        for (int i = 0; i < filterWordsData["BadWords"].Count; i++)
        {
            _wordsFilter.Add(filterWordsData["BadWords"][i].ToString());
            //Debug.Log(_wordsFilter[i]);
        }

        if (Character.Instance.CharacterCreation)
        {
            StartGame();
        }

        SetupCasesInDiary();
    }

    private void SetupCasesInDiary()
    {
        for (int i = 0; i < CurrentCasesDisplay.transform.GetChild(0).transform.childCount; i++)
        {
            Destroy(CurrentCasesDisplay.transform.GetChild(0).transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < AvailableCasesDisplay.transform.GetChild(0).transform.childCount; i++)
        {
            Destroy(AvailableCasesDisplay.transform.GetChild(0).transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < CompletedCasesDisplay.transform.GetChild(0).transform.childCount; i++)
        {
            Destroy(CompletedCasesDisplay.transform.GetChild(0).transform.GetChild(i).gameObject);
        }

        //Character.Instance.AllCases.Clear();
        foreach (Case loadedCase in Character.Instance.CurrentCases)
        {
            GameObject newCaseButton = Instantiate(CaseButtonPrefab, CurrentCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<Text>().text = loadedCase.Name;
        }

        foreach (Case loadedCase in Character.Instance.AvailableCases)
        {
            GameObject newCaseButton = Instantiate(CaseButtonPrefab, AvailableCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<Text>().text = loadedCase.Name;
        }

        foreach (Case loadedCase in Character.Instance.CompletedCases)
        {
            GameObject newCaseButton = Instantiate(CaseButtonPrefab, CompletedCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<Text>().text = loadedCase.Name;
        }
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

    // ****************************
    #region Character creation functions
    public void ConfirmCharacter()
    {
        // Close/Open popup is meant to be used mainly for in-game popups but since it
        // does the same thing as closing one, we can reuse it for other UI as well.
        OpenPopup(CharacterNameMenu);
        ClosePopup(CharacterCompletionPopup);
        ClosePopup(CharacterCreationMenu);

        Character.Instance.RefreshWearables();
        //Debug.Log("Character has been created!");
    }

    public void ConfirmCharacterName(Object obj)
    {
        GameObject inputField = obj as GameObject;
        string nameInput = inputField.GetComponent<InputField>().text;

        bool foundMatch = false;
        for (int i = 0; i < _wordsFilter.Count; i++)
        {
            Match match = Regex.Match(nameInput, @"(\b" + _wordsFilter[i] + @"|\B" + _wordsFilter[i] + @")",
                RegexOptions.IgnoreCase);
            
            if (match.Success && match.Length > 1)
            {
                //Debug.Log(nameInput);
                //Debug.Log(_wordsFilter[i]);
                foundMatch = true;
            }
        }

        if (foundMatch == false && nameInput.Length > 2 && nameInput.Length < 25)
        {
            Character.Instance.Name = nameInput;
            Character.Instance.CharacterCreation = true;

            Character.Instance.RefreshJsonData();
            ClosePopup(CharacterNameMenu);
            ClosePopup(CharacterNamePopupWindow);
            OpenPopup(StartMenu);

            //Debug.Log("Character name has been chosen!");
        } else
        {
            OpenPopup(CharacterNameErrorPopupWindow);
        }
    }

    public void ShowKeyboard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, true);
    }

    private void StartGame()
    {
        ClosePopup(CharacterCreationMenu);
        ClosePopup(CharacterCompletionPopup);
        ClosePopup(CharacterNameMenu);
        ClosePopup(CharacterNamePopupWindow);
        OpenPopup(StartMenu);
    }
    #endregion

    // ****************************
    #region Cases and diary functions
    public void DisplayCaseDetails(Object obj)
    {
        GameObject button = obj as GameObject;

        _currentlySelectedCase = null;
        foreach (Case loadedCase in Character.Instance.AllCases)
        {
            if (loadedCase.Name == button.GetComponentInChildren<Text>().text)
            {
                _currentlySelectedCase = loadedCase;

                SelectedCaseTitle.GetComponent<Text>().text = "Title: " + loadedCase.Name;
                SelectedCaseDescription.GetComponent<Text>().text = "Description: " + loadedCase.Description;
                SelectedCaseStatus.GetComponent<Text>().text = "Completion Status: " + loadedCase.ProgressStatus;

                for (int i = 0; i < SelectedCaseObjectivesDisplay.transform.childCount; i++)
                {
                    Destroy(SelectedCaseObjectivesDisplay.transform.GetChild(i).gameObject);
                }

                foreach (Objective objective in loadedCase.Objectives)
                {
                    GameObject newObjective = Instantiate(CaseObjectivePrefab,
                    SelectedCaseObjectivesDisplay.transform);
                    newObjective.transform.GetChild(0).GetComponent<Text>().text = objective.Name;

                    if (objective.CompletedStatus)
                    {
                        newObjective.transform.GetChild(1).GetComponent<Text>().text = "Completed: No";
                    }
                    else
                    {
                        newObjective.transform.GetChild(1).GetComponent<Text>().text = "Completed: Yes";
                    }
                }
            }
        }
    }

    public void TakeOnCase()
    {
        if (_currentlySelectedCase.ProgressStatus == "Available")
        {
            foreach (Case completedCase in Character.Instance.CompletedCases)
            {
                if (completedCase.Name == _currentlySelectedCase.Name)
                {
                    Character.Instance.MoveCaseStageStatus(_currentlySelectedCase, "Completed Cases", "CurrentCases");
                    completedCase.ProgressStatus = "Ongoing";
                    _currentlySelectedCase.CompletionStatus = false;
                    break;
                }
            }
            foreach (Case availableCase in Character.Instance.AvailableCases)
            {
                if (availableCase.Name == _currentlySelectedCase.Name)
                {
                    Character.Instance.MoveCaseStageStatus(_currentlySelectedCase, "Available Cases", "Current Cases");
                    availableCase.ProgressStatus = "Ongoing";
                    _currentlySelectedCase.CompletionStatus = false;
                    break;
                }
            }

            _currentlySelectedCase = null;

            Character.Instance.RefreshAllCases();
            Character.Instance.SetupCases();
            SetupCasesInDiary();

            ResetFields();
        }
    }

    public void AbandonCase()
    {
        if (_currentlySelectedCase.ProgressStatus == "Ongoing")
        {
            Character.Instance.MoveCaseStageStatus(_currentlySelectedCase, "Current Cases", "Available Cases");
            _currentlySelectedCase.ProgressStatus = "Available";
            _currentlySelectedCase.CompletionStatus = false;

            _currentlySelectedCase = null;

            Character.Instance.RefreshAllCases();
            Character.Instance.SetupCases();
            SetupCasesInDiary();

            ResetFields();
        }
    }

    public void ResetFields()
    {
        SelectedCaseTitle.GetComponent<Text>().text = string.Empty;
        SelectedCaseDescription.GetComponent<Text>().text = string.Empty;
        SelectedCaseStatus.GetComponent<Text>().text = string.Empty;

        for (int i = 0; i < SelectedCaseObjectivesDisplay.transform.childCount; i++)
        {
            Destroy(SelectedCaseObjectivesDisplay.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < SelectedCaseObjectivesDisplay.transform.childCount; i++)
        {
            Destroy(SelectedCaseObjectivesDisplay.transform.GetChild(i));
        }
    }
    #endregion
}
