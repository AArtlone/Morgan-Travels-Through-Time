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
    [Tooltip("You can list a number of objectives here that the player has to have completed in order for this dialogue to happen.")]
    public List<Objective> ObjectivesToMeet;
    [Tooltip("You can list a number of items here that the player has to have acquired in order for this dialogue to happen.")]
    public List<string> RequiredItems;
}
