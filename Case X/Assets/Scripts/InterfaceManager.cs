using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
