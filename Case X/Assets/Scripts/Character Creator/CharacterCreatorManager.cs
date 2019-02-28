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
    public GameObject CharacterClothesSelectionErrorPopup;

    //public bool allPartsSelected;

    private string _jsonWordsFilter;
    private List<string> _wordsFilter = new List<string>();
    #endregion
    
    void Start()
    {
        // When the game starts we extract all the bad words that we want to
        // filter out whenever the player is deciding on a character name.
        TextAsset filterWordsToJson = Resources.Load<TextAsset>("Default World Data/BadWords");
        JsonData filterWordsData = JsonMapper.ToObject(filterWordsToJson.text);

        for (int i = 0; i < filterWordsData["BadWords"].Count; i++)
        {
            _wordsFilter.Add(filterWordsData["BadWords"][i].ToString());
            //Debug.Log(_wordsFilter[i]);
        }

        // If the player has already created a character then 
        // we just start the main menu instead.
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
        OpenWindow(CharacterNameMenu);
        CloseWindow(CharacterCompletionPopup);
        CloseWindow(CharacterCreationMenu);

        Character.Instance.RefreshWearables();
        //Debug.Log("Character has been created!");
    }

    public void CheckForSelectedClothing()
    {
        bool hairSelected = false;
        bool faceSelected = false;
        bool outfitSelected = false;
        bool genderSelected = false;
        bool skinSelected = false;

        foreach(Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == "Hair" &&
                clothing.Selected)
            {
                hairSelected = true;
            }
            if (clothing.BodyPart == "Face" &&
                clothing.Selected)
            {
                faceSelected = true;
            }
            if (clothing.BodyPart == "Gender" &&
                clothing.Selected)
            {
                genderSelected = true;
            }
            if (clothing.BodyPart == "Skin Color" &&
                clothing.Selected)
            {
                skinSelected = true;
            }
            if (clothing.BodyPart == "Outfit" &&
                clothing.Selected)
            {
                outfitSelected = true;
            }
        }

        if (hairSelected == false ||
            faceSelected == false ||
            genderSelected == false ||
            skinSelected == false ||
            outfitSelected == false)
        {
            OpenWindow(CharacterClothesSelectionErrorPopup);
        } else
        {
            OpenWindow(CharacterNamePopupWindow);
        }
    }

    public void ConfirmCharacterName(Object obj)
    {
        GameObject inputField = obj as GameObject;
        string nameInput = inputField.GetComponent<InputField>().text;

        bool foundMatch = false;
        for (int i = 0; i < _wordsFilter.Count; i++)
        {
            // We look for a match in the current name the player has picked for his character
            // and if there is one, then an error will later be visualized and wont let him continue
            // until he corrects his name.
            Match match = Regex.Match(nameInput, @"(\b" + _wordsFilter[i] + @"|\B" + _wordsFilter[i] + @")",
                RegexOptions.IgnoreCase);

            if (match.Success && match.Length > 1)
            {
                foundMatch = true;
                //Debug.Log(nameInput);
                //Debug.Log(_wordsFilter[i]);
            }
        }

        if (foundMatch == false && nameInput.Length > 2 && nameInput.Length < 25)
        {
            Character.Instance.Name = nameInput;
            Character.Instance.CharacterCreation = true;

            // We update the player's name now that its confirmed as valid and then we call SetupWorldData
            // because once we load to a new scene with new gameo objects such as the
            // inventory and Quests diary, we want to re-initialize the data since the
            // scene before that did not contain those elements to put the data in.
            Character.Instance.RefreshJsonData();
            Character.Instance.SetupWorldData();
            SceneManager.LoadScene("Main Map");
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

    //public void ConfirmCharacterOutfit(Object obj)
    //{
    //    if (allPartsSelected)
    //    {
    //        GameObject windowObj = (GameObject)obj;
    //        windowObj.SetActive(true);
    //    }
    //}
    #endregion
}
