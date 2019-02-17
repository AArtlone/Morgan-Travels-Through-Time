using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // The manager is a singleton used throughout the game
    // to manage the dialogue boxes of npc and interactables.
    public static DialogueManager Instance;

    public GameObject DialogueTemplate;
    public Text LeftCharacterTitle;
    public Image LeftCharacterPortrait;
    public Text RightCharacterTitle;
    public Image RightCharacterPortrait;
    public Image DialogueBoxBackground;
    public Text DialogueText;
    public Text[] OptionsMenu = new Text[4];

    private void Awake()
    {
        // If we have an instance of this singleton somewhere else
        // in the game, then we want to destroy the existing one.
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
            // We want to be able to access the dialogue information from any scene.
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ChangePortrait(string side, Sprite newPortrait)
    {
        if (side == "left")
        {
            LeftCharacterPortrait.sprite = newPortrait;
        } else if (side == "right")
        {
            RightCharacterPortrait.sprite = newPortrait;
        }

        //Debug.Log("Changed " + side + " portrait!");
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
}
