using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // The manager is a singleton used throughout the game
    // to manage the dialogue boxes of npc and interactables.
    public static DialogueManager Instance;
    public NPC CurrentNPCDialogue;

    public GameObject DialogueTemplate;
    [Space(10)]
    public TextMeshProUGUI LeftCharacterTitle;
    public GameObject LeftCharacterPortrait;
    [Space(10)]
    public TextMeshProUGUI RightCharacterTitle;
    public Image RightCharacterPortrait;
    [Space(10)]
    public Image DialogueProgressionBox;
    public Image DialogueStageBackground;
    public Image DialogueBoxBackground;
    public TextMeshProUGUI DialogueText;
    public TextMeshProUGUI[] OptionsMenu = new TextMeshProUGUI[4];
    public List<string> DialogueResponses;

    private string _dialogueResponsesPath;
    private float _offSet = 100f;
    private Vector3 LeftPortraitInitialPos;
    private Vector3 RightPortraitInitialPos;

    private void Awake()
    {
        _dialogueResponsesPath = Application.persistentDataPath + "/DialogueResponses.json";
        // If we have an instance of this singleton somewhere else
        // in the game, then we want to destroy the existing one.
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
            // We want to be able to access the dialogue information from any scene.
            //DontDestroyOnLoad(gameObject);

            if (!File.Exists(_dialogueResponsesPath))
            {
                TextAsset responsesData = Resources.Load<TextAsset>("Default World Data/DialogueResponses");
                JsonData responsesJsonData = JsonMapper.ToObject(responsesData.text);

                DialogueResponses.Clear();
                for (int i = 0; i < responsesJsonData["DialogueResponses"].Count; i++)
                {
                    DialogueResponses.Add(responsesJsonData["DialogueResponses"][i].ToString());
                }

                File.WriteAllText(_dialogueResponsesPath, "");
                RefreshDialogueResponses();
            }
        }
    }

    private void Start()
    {
        SetupDialogueResponses();

        RightPortraitInitialPos = RightCharacterPortrait.rectTransform.position;
        LeftPortraitInitialPos = LeftCharacterPortrait.transform.position;

        RefreshDialogueResponses();
    }

    public void DeleteJsonFile()
    {
        if(File.Exists(_dialogueResponsesPath))
        {
            File.Delete(_dialogueResponsesPath);
            Destroy(Instance);
            Destroy(gameObject);
        }
    }

    private void SetupDialogueResponses()
    {
        string responsesToJson = File.ReadAllText(_dialogueResponsesPath);
        JsonData responsesData = JsonMapper.ToObject(responsesToJson);
        
        DialogueResponses.Clear();
        for (int i = 0; i < responsesData["DialogueResponses"].Count; i++)
        {
            DialogueResponses.Add(responsesData["DialogueResponses"][i].ToString());
        }
    }

    public void RefreshDialogueResponses()
    {
        File.WriteAllText(_dialogueResponsesPath, "");

        // This creates the starting wrapper of the json file.
        string newItemsData = "{";
        newItemsData += Environment.NewLine + "\t";
        newItemsData += "\"DialogueResponses\": [";

        HashSet<string> uniqueResponses = new HashSet<string>(DialogueResponses);

        foreach (string response in uniqueResponses)
        {
            newItemsData += Environment.NewLine + "\t" + "\t";
            newItemsData += "\"" + response + "\"" + ",";
        }
        // This removes the last comma at the last item in the array, so
        // that we wont get an error when getting the data later on.
        newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += Environment.NewLine + "\t" + "]" + Environment.NewLine + "}";
        File.WriteAllText(_dialogueResponsesPath, newItemsData);
    }

    /// <summary>
    /// This function moves the currently speaking character in the dialogue up in
    /// order to provide clarity as to who is speaking.
    /// </summary>
    /// <param name="side"></param>
    public void OffSetPortrait(string side)
    {
        if (side == "left")
        {
            RightCharacterPortrait.rectTransform.position = RightPortraitInitialPos;
            LeftCharacterPortrait.transform.position = new Vector3(LeftCharacterPortrait.transform.position.x, LeftCharacterPortrait.transform.position.y + _offSet, 0f);
        } else if(side == "right")
        {
            LeftCharacterPortrait.transform.position = LeftPortraitInitialPos;
            if (RightCharacterPortrait.sprite.name != "UncleBen001")
            {
                if (RightCharacterPortrait.rectTransform.position == RightPortraitInitialPos)
                {
                    RightCharacterPortrait.rectTransform.position = new Vector3(RightCharacterPortrait.rectTransform.position.x, RightCharacterPortrait.rectTransform.position.y + _offSet, 0f);
                }
            }
        }
    }

    #region The following functions serve to display the upcoming dialogues to the screen.
    public void ChangeTitle(string side, string newTitle)
    {
        if (side == "left")
        {
            LeftCharacterTitle.text = newTitle;
        }
        else if (side == "right")
        {
            RightCharacterTitle.text = newTitle;
        }
    }

    public void ChangePortrait(string side, Sprite newPortrait)
    {
        if (side == "left")
        {
            // Not necessary due to the fact that the player can customize his
            // appearance which is then displayed, so he does not require a portrait.
            //LeftCharacterPortrait.GetComponent<Image>().sprite = newPortrait;
        }
        else if (side == "right")
        {
            RightCharacterPortrait.sprite = newPortrait;
        }
    }

    public void ChangeDialogueStageBackground(Sprite newStage)
    {
        DialogueStageBackground.sprite = newStage;
    }

    public void ChangeDialogueBoxBackground(Sprite newBackground)
    {
        DialogueBoxBackground.sprite = newBackground;
    }

    public void ChangeDialogueText(string newText)
    {
        DialogueText.text = newText;
    }

    public void ChangeOptionsMenu(string[] newOptions)
    {
        for (int i = 0; i < newOptions.Length; i++)
        {
            if (newOptions[i] != null)
            {
                OptionsMenu[i].text = newOptions[i];
            }
        }
    }

    public void ClearOptionsMenu()
    {
        for (int i = 0; i < OptionsMenu.Length; i++)
        {
            OptionsMenu[i].text = string.Empty;
        }
    }

    public void ToggleDialogue(bool display)
    {
        DialogueTemplate.SetActive(display);
    }
    #endregion

    /// <summary>
    /// This function runs whenever the player has tapped on an options from the
    /// dialogue's options menu.
    /// </summary>
    /// <param name="obj"></param>
    public void SelectOption(GameObject obj)
    {
        string textInOption = obj.GetComponent<TextMeshProUGUI>().text;
        if (!DialogueResponses.Contains(textInOption))
        {
            DialogueResponses.Add(textInOption);
        }

        RefreshDialogueResponses();
        CurrentNPCDialogue.ContinueDialogue();
    }
}
