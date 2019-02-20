using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    private Character _characterScript;

    #region Character creation references
    public GameObject CharacterCompletionPopup;
    public GameObject CharacterCreationMenu;
    public GameObject CharacterNameMenu;
    public GameObject CharacterNamePopupWindow;
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
    private List<Case> _allCases = new List<Case>();
    private Case _currentlySelectedCase;
    #endregion

    #region Gameplay start references
    public GameObject StartMenu;
    #endregion

    private void Start()
    {
        _characterScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();

        if (_characterScript.CharacterCreation)
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

        _allCases.Clear();
        foreach (Case loadedCase in _characterScript.CurrentCases)
        {
            _allCases.Add(loadedCase);

            GameObject newCaseButton = Instantiate(CaseButtonPrefab, CurrentCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<Text>().text = loadedCase.Name;
        }

        foreach (Case loadedCase in _characterScript.AvailableCases)
        {
            _allCases.Add(loadedCase);

            GameObject newCaseButton = Instantiate(CaseButtonPrefab, AvailableCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<Text>().text = loadedCase.Name;
        }

        foreach (Case loadedCase in _characterScript.CompletedCases)
        {
            _allCases.Add(loadedCase);

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
        ClosePopup(CharacterCompletionPopup);
        // Closepopup is meant to be used mainly for in-game popups but since it
        // does the same thing as closing one, we can reuse it for other UI as well.
        ClosePopup(CharacterCreationMenu);

        _characterScript.RefreshWearables();
        //Debug.Log("Character has been created!");
    }

    public void ConfirmCharacterName(Object obj)
    {
        GameObject inputFieldText = obj as GameObject;

        if (inputFieldText.GetComponent<Text>().text.Length > 2)
        {
            _characterScript.Name = inputFieldText.GetComponent<Text>().text;
            _characterScript.CharacterCreation = true;

            _characterScript.RefreshJsonData();
            ClosePopup(CharacterNameMenu);
            ClosePopup(CharacterNamePopupWindow);
            OpenPopup(StartMenu);

            //Debug.Log("Character name has been chosen!");
        } else
        {
            // TODO: Add feedback for name error handling.

            //Debug.LogWarning("Character name is too short!");
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
        foreach (Case loadedCase in _allCases)
        {
            if (loadedCase.Name == button.GetComponentInChildren<Text>().text)
            {
                _currentlySelectedCase = loadedCase;

                SelectedCaseTitle.GetComponent<Text>().text = "Title: " + loadedCase.Name;
                SelectedCaseDescription.GetComponent<Text>().text = "Description: " + loadedCase.Description;
                SelectedCaseStatus.GetComponent<Text>().text = "Completion Status: " + loadedCase.CompletionStatus;

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
        foreach (Case completedCase in _characterScript.CompletedCases)
        {
            if (completedCase.Name == _currentlySelectedCase.Name)
            {
                _characterScript.MoveCaseStageStatus(_currentlySelectedCase, "Completed Cases", "CurrentCases");
                break;
            }
        }
        foreach (Case availableCase in _characterScript.AvailableCases)
        {
            if (availableCase.Name == _currentlySelectedCase.Name)
            {
                _characterScript.MoveCaseStageStatus(_currentlySelectedCase, "Available Cases", "Current Cases");
                break;
            }
        }

        _currentlySelectedCase = null;

        SetupCasesInDiary();
    }

    public void AbandonCase()
    {
        _characterScript.MoveCaseStageStatus(_currentlySelectedCase, "Current Cases", "Available Cases");

        _currentlySelectedCase = null;

        SetupCasesInDiary();
    }
    #endregion
}
