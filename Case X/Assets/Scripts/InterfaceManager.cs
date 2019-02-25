using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    #region Diary and cases interface references
    [Space(10)]
    public GameObject CaseButtonPrefab;
    public GameObject CurrentCasesDisplay;
    public GameObject AvailableCasesDisplay;
    public GameObject CompletedCasesDisplay;
    [Space(10)]
    public GameObject CaseObjectivePrefab;
    public TextMeshProUGUI SelectedCaseTitle;
    public TextMeshProUGUI SelectedCaseDescription;
    public TextMeshProUGUI SelectedCaseStatus;
    public GameObject SelectedCaseObjectivesDisplay;
    private Case _currentlySelectedCase;
    #endregion

    #region Gameplay start references
    [Space(10)]
    public GameObject StartMenu;
    #endregion

    #region Item details for bottom UI inventory
    [Space(10)]
    public GameObject BottomUIInventory;
    public GameObject ItemDetailsWindow;
    public Image ItemDetailsPortrait;
    public TextMeshProUGUI ItemDetailsName;
    public TextMeshProUGUI ItemDetailsDescription;
    public TextMeshProUGUI ItemDetailsActives;
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
        SetupCasesInDiary();
    }

    public void SetupCasesInDiary()
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
            newCaseButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedCase.Name;
        }

        foreach (Case loadedCase in Character.Instance.AvailableCases)
        {
            GameObject newCaseButton = Instantiate(CaseButtonPrefab, AvailableCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedCase.Name;
        }

        foreach (Case loadedCase in Character.Instance.CompletedCases)
        {
            GameObject newCaseButton = Instantiate(CaseButtonPrefab, CompletedCasesDisplay.transform.GetChild(0).transform);
            newCaseButton.GetComponentInChildren<TextMeshProUGUI>().text = loadedCase.Name;
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
    

    // ****************************
    #region Cases and diary functions
    public void DisplayCaseDetails(Object obj)
    {
        GameObject button = obj as GameObject;

        _currentlySelectedCase = null;
        foreach (Case loadedCase in Character.Instance.AllCases)
        {
            if (loadedCase.Name == button.GetComponentInChildren<TextMeshProUGUI>().text)
            {
                _currentlySelectedCase = loadedCase;

                SelectedCaseTitle.text = "Title: " + loadedCase.Name;
                SelectedCaseDescription.text = "Description: " + loadedCase.Description;
                SelectedCaseStatus.text = "Completion Status: " + loadedCase.ProgressStatus;

                for (int i = 0; i < SelectedCaseObjectivesDisplay.transform.childCount; i++)
                {
                    Destroy(SelectedCaseObjectivesDisplay.transform.GetChild(i).gameObject);
                }

                foreach (Objective objective in loadedCase.Objectives)
                {
                    GameObject newObjective = Instantiate(CaseObjectivePrefab,
                    SelectedCaseObjectivesDisplay.transform);
                    newObjective.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = objective.Name;

                    if (objective.CompletedStatus)
                    {
                        newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Done: No";
                    }
                    else
                    {
                        newObjective.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Done: Yes";
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
        SelectedCaseTitle.text = string.Empty;
        SelectedCaseDescription.text = string.Empty;
        SelectedCaseStatus.text = string.Empty;

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
