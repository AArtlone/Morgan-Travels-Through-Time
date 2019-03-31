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
    [Space(10)]
    public string RightCharacterTitle;
    public Sprite RightCharacterPortrait;
    [Space(10)]
    public Sprite DialogueStageBackground;
    public Sprite DialogueBoxBackground;
    public List<DialogueBranch> DialogueBranches;
    public List<Objective> ObjectivesToMeet;
}
