using LitJson;
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
        string newItemsData = "{\"DialogueResponses\":[";

        HashSet<string> uniqueResponses = new HashSet<string>(DialogueResponses);

        foreach (string response in uniqueResponses)
        {
            newItemsData += "\"" + response + "\"" + ",";
        }
        // This removes the last comma at the last item in the array, so
        // that we wont get an error when getting the data later on.
        newItemsData = newItemsData.Substring(0, newItemsData.Length - 1);
        // This closes the wrapper of the json file made from the beginning.
        newItemsData += "]}";
        File.WriteAllText(_dialogueResponsesPath, newItemsData);
    }

    public void ChangePortrait(string side, Sprite newPortrait)
    {
        if (side == "left")
        {
            //LeftCharacterPortrait.GetComponent<Image>().sprite = newPortrait;
        } else if (side == "right")
        {
            RightCharacterPortrait.sprite = newPortrait;
        }

        //Debug.Log("Changed " + side + " portrait!");
    }

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

        //Debug.Log("Changed " + side + " title!");
    }

    public void ChangeDialogueStageBackground(Sprite newStage)
    {
        DialogueStageBackground.sprite = newStage;
    }

    public void ChangeDialogueBoxBackground(Sprite newBackground)
    {
        DialogueBoxBackground.sprite = newBackground;

        //Debug.Log("Changed dialogue background!");
    }

    public void ChangeDialogueText(string newText)
    {
        DialogueText.text = newText;

        //Debug.Log("Changed dialogue text!");
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

        //Debug.Log("Changed dialogue options!");
    }

    public void ClearOptionsMenu()
    {
        for (int i = 0; i < OptionsMenu.Length; i++)
        {
            OptionsMenu[i].text = string.Empty;
        }

        //Debug.Log("Cleared the dialogue options menu!");
    }

    public void ToggleDialogue(bool display)
    {
        DialogueTemplate.SetActive(display);
    }

    public void SelectOption(GameObject obj)
    {
        string textInOption = obj.GetComponent<TextMeshProUGUI>().text;
        if (!DialogueResponses.Contains(textInOption))
        {
            DialogueResponses.Add(textInOption);
        }

        RefreshDialogueResponses();
        CurrentNPCDialogue.ContinueDialogue();
        //Debug.Log(string.Format("Added ({0}) to the responses list!", textInOption));
    }
}
