﻿using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
    public GameObject CharacterNameErrorPopupWindow;
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
    public GameObject StartMenu;
    #endregion

    private string _jsonWordsFilter;
    private List<string> _wordsFilter = new List<string>();

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

        //_characterScript.AllCases.Clear();
        foreach (Case loadedCase in _characterScript.CurrentCases)
        {
            GameObject newCaseButton = Instantiate(CaseButtonPrefab, CurrentCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<Text>().text = loadedCase.Name;
        }

        foreach (Case loadedCase in _characterScript.AvailableCases)
        {
            GameObject newCaseButton = Instantiate(CaseButtonPrefab, AvailableCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<Text>().text = loadedCase.Name;
        }

        foreach (Case loadedCase in _characterScript.CompletedCases)
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

        _characterScript.RefreshWearables();
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
            _characterScript.Name = nameInput;
            _characterScript.CharacterCreation = true;

            _characterScript.RefreshJsonData();
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
        foreach (Case loadedCase in _characterScript.AllCases)
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
            foreach (Case completedCase in _characterScript.CompletedCases)
            {
                if (completedCase.Name == _currentlySelectedCase.Name)
                {
                    _characterScript.MoveCaseStageStatus(_currentlySelectedCase, "Completed Cases", "CurrentCases");
                    completedCase.ProgressStatus = "Ongoing";
                    _currentlySelectedCase.CompletionStatus = false;
                    break;
                }
            }
            foreach (Case availableCase in _characterScript.AvailableCases)
            {
                if (availableCase.Name == _currentlySelectedCase.Name)
                {
                    _characterScript.MoveCaseStageStatus(_currentlySelectedCase, "Available Cases", "Current Cases");
                    availableCase.ProgressStatus = "Ongoing";
                    _currentlySelectedCase.CompletionStatus = false;
                    break;
                }
            }

            _currentlySelectedCase = null;

            _characterScript.RefreshAllCases();
            _characterScript.SetupCases();
            SetupCasesInDiary();

            ResetFields();
        }
    }

    public void AbandonCase()
    {
        if (_currentlySelectedCase.ProgressStatus == "Ongoing")
        {
            _characterScript.MoveCaseStageStatus(_currentlySelectedCase, "Current Cases", "Available Cases");
            _currentlySelectedCase.ProgressStatus = "Available";
            _currentlySelectedCase.CompletionStatus = false;

            _currentlySelectedCase = null;

            _characterScript.RefreshAllCases();
            _characterScript.SetupCases();
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
