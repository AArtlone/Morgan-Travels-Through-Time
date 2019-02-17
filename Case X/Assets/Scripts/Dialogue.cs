using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public string DialogueTitle;
    public string LeftCharacterTitle;
    public Sprite LeftCharacterPortrait;
    public string RightCharacterTitle;
    public Sprite RightCharacterPortrait;
    public Sprite DialogueBoxBackground;
    public string DialogueText;
    public string[] OptionsMenu;
}
