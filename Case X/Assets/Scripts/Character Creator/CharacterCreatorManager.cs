using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreatorManager : MonoBehaviour
{
    #region Character creation references
    public GameObject CharacterCompletionPopup;
    public GameObject CharacterCreationMenu;
    public GameObject CharacterNameMenu;
    public GameObject CharacterNamePopupWindow;
    public GameObject CharacterNameErrorPopupWindow;

    private string _jsonWordsFilter;
    private List<string> _wordsFilter = new List<string>();
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        TextAsset filterWordsToJson = Resources.Load<TextAsset>("Default World Data/BadWords");
        JsonData filterWordsData = JsonMapper.ToObject(filterWordsToJson.text);

        for (int i = 0; i < filterWordsData["BadWords"].Count; i++)
        {
            _wordsFilter.Add(filterWordsData["BadWords"][i].ToString());
            //Debug.Log(_wordsFilter[i]);
        }

        if (Character.Instance.CharacterCreation)
        {
            SceneManager.LoadScene("Main Map");
        }
    }

    #region Character creation functions
    public void ConfirmCharacter()
    {
        // Close/Open popup is meant to be used mainly for in-game popups but since it
        // does the same thing as closing one, we can reuse it for other UI as well.
        InterfaceManager.Instance.OpenPopup(CharacterNameMenu);
        InterfaceManager.Instance.ClosePopup(CharacterCompletionPopup);
        InterfaceManager.Instance.ClosePopup(CharacterCreationMenu);

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
            //InterfaceManager.Instance.(CharacterNameMenu);
            //InterfaceManager.Instance.(CharacterNamePopupWindow);
            //InterfaceManager.Instance.(StartMenu);
            SceneManager.LoadScene("Main Map");

            Character.Instance.SetupWorldData();
            //Debug.Log("Character name has been chosen!");Debug.Log
        }
        else
        {
            InterfaceManager.Instance.OpenPopup(CharacterNameErrorPopupWindow);
        }
    }

    public void ShowKeyboard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, true);
    }

    public void OpenWindow(Object obj)
    {
        GameObject windowObj = (GameObject)obj;
        windowObj.SetActive(true);
    }

    public void CloseWindow(Object obj)
    {
        GameObject windowObj = (GameObject)obj;
        windowObj.SetActive(false);
    }
    #endregion
}
