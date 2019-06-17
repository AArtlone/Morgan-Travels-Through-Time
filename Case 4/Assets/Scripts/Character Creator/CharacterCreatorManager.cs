using LitJson;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

public class CharacterCreatorManager : MonoBehaviour
{
    #region Character creation references
    public GameObject CharacterCompletionPopup;
    public GameObject CharacterCreationMenu;
    public GameObject CharacterNameMenu;
    public GameObject CharacterNamePopupWindow;
    public GameObject CharacterNameErrorPopupWindow;
    public GameObject CharacterClothesSelectionErrorPopup;
    public GameObject CharacterClothesChangedErrorPopop;
    public GameObject SaveCharacterPopup;

    public TextMeshProUGUI CharacterSelectionErrorMessage;
    private string errorString;

    private string _currentBody;
    private string _currentFace;
    private string _currentHair;
    private string _currentTop;
    private string _currentBot;
    private string _currentShoes;

    private string _jsonWordsFilter;
    private List<string> _wordsFilter = new List<string>();
    #endregion

    [Space(10)]
    public GameObject LoadingScreen;
    private DHManager _DHManager;
    
    void Start()
    {
        _DHManager = FindObjectOfType<DHManager>();

        if (SceneManager.GetActiveScene().name == "Beginning Character Creation")
        {
            if (_DHManager != null)
            {
                _DHManager.LoadSequence("Teach Settings");
            }
        }

        // When the game starts we extract all the bad words that we want to
        // filter out whenever the player is deciding on a character name.
        TextAsset filterWordsToJson = Resources.Load<TextAsset>("Default World Data/BadWords");
        JsonData filterWordsData = JsonMapper.ToObject(filterWordsToJson.text);

        for (int i = 0; i < filterWordsData["BadWords"].Count; i++)
        {
            _wordsFilter.Add(filterWordsData["BadWords"][i].ToString());
        }

        DefineCurrentWearables();
    }

    //defining current player's appearances in order to later check if any has changed
    public void DefineCurrentWearables()
    {
        if (SceneManager.GetActiveScene().name == "Character Customization")
        {
            foreach (Clothing clothing in Character.Instance.Wearables)
            {
                if (clothing.Selected == true)
                {
                    if (clothing.BodyPart == "Body")
                    {
                        _currentBody = clothing.Name;
                    }
                    if (clothing.BodyPart == "Face")
                    {
                        _currentFace = clothing.Name;
                    }
                    else if (clothing.BodyPart == "Hair")
                    {
                        _currentHair = clothing.Name;
                    }
                    else if (clothing.BodyPart == "Top")
                    {
                        _currentTop = clothing.Name;
                    }
                    else if (clothing.BodyPart == "Bot")
                    {
                        _currentBot = clothing.Name;
                    }
                    else if (clothing.BodyPart == "Shoes")
                    {
                        _currentShoes = clothing.Name;
                    }
                }
            }
        }
    }

    #region Character creation functions
    public void ConfirmCharacter()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        // Close/Open popup is meant to be used mainly for in-game popups but since it
        // does the same thing as closing one, we can reuse it for other UI as well.
        OpenWindow(CharacterNameMenu);
        CloseWindow(CharacterCompletionPopup);
        CloseWindow(CharacterCreationMenu);

        Character.Instance.RefreshWearables();
    }

    //function for the Character Customization scene only
    public void ConfirmCharacterChanges()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        Character.Instance.RefreshWearables();
        DefineCurrentWearables();
    }

    // Receives a bodypart and checks whether it is selected or not
    private bool CheckIfBodypartIsSelected(string Bodypart)
    {
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == Bodypart)
            {
                if (clothing.PortraitImage == "top000" && clothing.Selected == true)
                {
                    return false;
                } else if (clothing.PortraitImage == "bot000" && clothing.Selected == true)
                {
                    return false;
                } else if (clothing.Selected)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Receives a bodypart and checks whether it was changed or not based on the initially selected bodypart
    private bool CheckIfBodypartChanged(string Bodypart, string ClothingToCheck)
    {
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if(clothing.Selected == true)
            {
                if(clothing.BodyPart == Bodypart)
                {
                    if(clothing.Name != ClothingToCheck)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void SaveAndCheckMandatoryClothing()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        if (CheckIfBodypartIsSelected("Top") == false ||
            CheckIfBodypartIsSelected("Bot") == false)
        {
            LanguageController.Language language = LanguageController.Language.Dutch;
            switch (SettingsManager.Instance.Language)
            {
                case "English":
                    language = LanguageController.Language.English;
                    break;
                case "Dutch":
                    language = LanguageController.Language.Dutch;
                    break;
            }
            if (CheckIfBodypartIsSelected("Top") == false)
            {
                switch (language)
                {
                    case LanguageController.Language.English:
                        errorString = errorString + "top, ";
                        break;
                    case LanguageController.Language.Dutch:
                        errorString = errorString + "de top, ";
                        break;
                }
            }
            if (CheckIfBodypartIsSelected("Bot") == false)
            {
                switch (language)
                {
                    case LanguageController.Language.English:
                        errorString = errorString + "bot, ";
                        break;
                    case LanguageController.Language.Dutch:
                        errorString = errorString + "de broek, ";
                        break;
                }
            }

            switch (language)
            {
                case LanguageController.Language.English:
                    CharacterSelectionErrorMessage.text = "You have not selected " + errorString.Substring(0, errorString.Length - 2) + ".";
                    break;
                case LanguageController.Language.Dutch:
                    CharacterSelectionErrorMessage.text = "Je bent " + errorString.Substring(0, errorString.Length - 2) + " vergeten.";
                    break;
            }

            OpenWindow(CharacterClothesSelectionErrorPopup);
            errorString = string.Empty;
        } else
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

            Character.Instance.RefreshWearables();
        }
    }

    // Checks whether the player selected all mandatory to slect clothing elements. If he did then it progresses to the next step. If he did not then it shows the error msg
    public void CheckForMandatoryClothing()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        if (CheckIfBodypartIsSelected("Top") == false ||
            CheckIfBodypartIsSelected("Bot") == false)
        {
            LanguageController.Language language = LanguageController.Language.Dutch;
            switch (SettingsManager.Instance.Language)
            {
                case "English":
                    language = LanguageController.Language.English;
                    break;
                case "Dutch":
                    language = LanguageController.Language.Dutch;
                    break;
            }
            if (CheckIfBodypartIsSelected("Top") == false)
            {
                switch (language)
                {
                    case LanguageController.Language.English:
                        errorString = errorString + "top, ";
                        break;
                    case LanguageController.Language.Dutch:
                        errorString = errorString + "de top, ";
                        break;
                }
            }
            if (CheckIfBodypartIsSelected("Bot") == false)
            {
                switch (language)
                {
                    case LanguageController.Language.English:
                        errorString = errorString + "bot, ";
                        break;
                    case LanguageController.Language.Dutch:
                        errorString = errorString + "de broek, ";
                        break;
                }
            }

            switch (language)
            {
                case LanguageController.Language.English:
                    CharacterSelectionErrorMessage.text = "You have not selected " + errorString.Substring(0, errorString.Length - 2) + ".";
                    break;
                case LanguageController.Language.Dutch:
                    CharacterSelectionErrorMessage.text = "Je bent " + errorString.Substring(0, errorString.Length - 2) + " vergeten.";
                    break;
            }

            OpenWindow(CharacterClothesSelectionErrorPopup);
            errorString = string.Empty;
        } else
        {
            if (SceneManager.GetActiveScene().name == "Character Customization")
            {
                ReturnToMainMap();
            }
            else
            {
                OpenWindow(CharacterNamePopupWindow);
            }
        }
    }

    // Function that checks if any of the body part has changed
    public void CheckIfClothingChanged()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.PortraitImage == "top000" || clothing.PortraitImage == "bot000")
            {
                OpenWindow(CharacterClothesChangedErrorPopop);
                return;
            }
        }

        if(CheckIfBodypartChanged("Top", _currentTop) == true ||
            CheckIfBodypartChanged("Bot", _currentBot) == true ||
            CheckIfBodypartChanged("Hair", _currentHair) == true ||
            CheckIfBodypartChanged("Shoes", _currentShoes) == true ||
            CheckIfBodypartChanged("Body", _currentBody) == true ||
            CheckIfBodypartChanged("Face", _currentFace) == true)
        {
            OpenWindow(CharacterClothesChangedErrorPopop);
        } else
        {
            ReturnToMainMap();
        }
    }

    public void ReturnToMainMap()
    {
        StartCoroutine(ReturnToMainMapCo());
    }

    // Simply return to main map without any additional checks
    public IEnumerator ReturnToMainMapCo()
    {
        Character.Instance.CharacterCreation = true;
        Character.Instance.RefreshJsonData();

        LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(Character.Instance.LastScene);

        yield return new WaitForEndOfFrame();
    }

    public IEnumerator ConfirmCharacterNameCo(Object obj)
    {
        GameObject inputField = obj as GameObject;
        string nameInput = inputField.GetComponent<InputField>().text;

        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        bool foundMatch = false;
        //for (int i = 0; i < _wordsFilter.Count; i++)
        //{
        //    // We look for a match in the current name the player has picked for his character
        //    // and if there is one, then an error will later be visualized and wont let him continue
        //    // until he corrects his name.
        //    Match match = Regex.Match(nameInput, @"(\b" + _wordsFilter[i] + @"|\B" + _wordsFilter[i] + @")",
        //        RegexOptions.IgnoreCase);

        //    if (match.Success && match.Length > 1)
        //    {
        //        foundMatch = true;
        //    }

        if (foundMatch == false && nameInput.Length > 2 && nameInput.Length < 25)
        {
            Character.Instance.Name = nameInput;
            Character.Instance.CharacterCreation = true;

            // We update the player's name now that its confirmed as valid and then we call SetupWorldData
            // because once we load to a new scene with new gameo objects such as the
            // inventory and Quests diary, we want to re-initialize the data since the
            // scene before that did not contain those elements to put the data in.
            Character.Instance.RefreshWearables();
            Character.Instance.RefreshJsonData();
            Character.Instance.SetupWorldData();
            if (SceneManager.GetActiveScene().name == "Beginning Character Creation")
            {
                Character.Instance.LastScene = "Tutorial Map Area";
                Character.Instance.RefreshJsonData();

                LoadingScreen.SetActive(true);

                AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Tutorial Map Area");
            }
            else
            {
                GameObject.Find("Character Name Menu").SetActive(false);
            }
        }
        else
        {
            OpenWindow(CharacterNameErrorPopupWindow);
        }

        yield return new WaitForEndOfFrame();
    }

    public void ConfirmCharacterName(Object obj)
    {
        StartCoroutine(ConfirmCharacterNameCo(obj));
    }

    public void ShowKeyboard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, true);
    }

    public void OpenWindow(Object obj)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        GameObject windowObj = (GameObject)obj;
        windowObj.SetActive(true);
    }

    public void CloseWindow(Object obj)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        GameObject windowObj = (GameObject)obj;
        windowObj.SetActive(false);
    }
    #endregion

    public void ResetTheGame()
    {
        StartCoroutine(ResetTheGameCo());
    }

    public IEnumerator ResetTheGameCo()
    {
        if (SceneManager.GetActiveScene().name == "Beginning Character Creation")
        {
            SettingsManager.Instance.RemoveSingletonInstance();
            Character.Instance.RemoveSingletonInstance();
            SceneManagement.Instance.RemoveSingletonInstance();
            DeleteJsonDataFromStorage();
        }
        else
        {
            SettingsManager.Instance.RemoveSingletonInstance();
            Character.Instance.RemoveSingletonInstance();
            SceneManagement.Instance.RemoveSingletonInstance();
            DeleteJsonDataFromStorage();
        }

        LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Logo Introduction");

        yield return new WaitForEndOfFrame();
    }

    public void DeleteJsonDataFromStorage()
    {
        string path = Application.persistentDataPath;
        DirectoryInfo gameDataDirectory = new DirectoryInfo(path);
        FileInfo[] gameData = gameDataDirectory.GetFiles();

        List<FileInfo> filesToRemove = new List<FileInfo>();

        // Here we retrieve the relevant files from the game's data directory
        // and store a list of them for removal.
        foreach (FileInfo file in gameData)
        {
            string fileTypeString = string.Empty;
            for (int i = 0; i < file.Name.Length; i++)
            {
                if (file.Name[i] == '.')
                {
                    fileTypeString = file.Name.Substring(i, file.Name.Length - i);

                    if (fileTypeString == ".json")
                    {
                        filesToRemove.Add(file);
                    }
                }
            }
        }

        // Then we use the list of files to remove to delete the ones that match
        // in the game data directory.
        foreach (FileInfo file in gameData)
        {
            foreach (FileInfo fileToRemove in filesToRemove)
            {
                if (file.Name == fileToRemove.Name)
                {
                    File.Delete(path + "/" + fileToRemove.Name);
                }
            }
        }
    }
}
