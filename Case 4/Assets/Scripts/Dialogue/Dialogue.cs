using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string DialogueTitle;
    [Range(0, 10)]
    public int PriorityIndex;
    [Space(10)]
    public string LeftCharacterTitle;
    public Sprite LeftCharacterPortrait;
    public bool LeftPortraitTalking;
    [Space(10)]
    public string RightCharacterTitle;
    public Sprite RightCharacterPortrait;
    public bool RightPortraitTalking;
    [Space(10)]
    public Sprite DialogueStageBackground;
    public Sprite DialogueBoxBackground;
    public List<DialogueBranch> DialogueBranches;
    public List<Objective> ObjectivesToMeet;
    public List<string> RequiredItems;
}
