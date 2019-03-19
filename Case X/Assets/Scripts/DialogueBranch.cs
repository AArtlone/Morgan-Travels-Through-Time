using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueBranch
{
    public string BranchTitle;
    public List<string> PreviousResponses = new List<string>();
    [Range(0, 10)]
    [Tooltip("Priority decides whether this branch should be displayed compared to others that falls into the same player requirements.")]
    public int Priority;
    [Space(10)]
    [Tooltip("Defines the minimum requirement for player reputation.")]
    public int ReputationMinimum;
    [Tooltip("Defines the minimum requirement for player stamina.")]
    public int StaminaMinimum;
    [Tooltip("Defines the minimum requirement for player knowledge.")]
    public int KnowledgeMinimum;
    [Tooltip("Defines the minimum requirement for player fitness.")]
    public int FitnessMinimum;
    [Tooltip("Defines the minimum requirement for player charisma.")]
    public int CharismaMinimum;
    [Tooltip("Defines the minimum requirement for player currency.")]
    public int CurrencyMinimum;
    [Tooltip("To pick an item correctly, you use the Resources -> Items folder for items and write down an item's name without its file type in the field.")]
    public List<string> ItemsRequired = new List<string>();
    [Tooltip("This shows the items the player has dragged and dropped into the current dialogue sequence.")]
    public List<string> ItemsDropped = new List<string>();
    [TextArea(0, 100)]
    [Space(10)]
    [Tooltip("Text that will be displayed on the current dialogue sequence if t hat branch's requirements are met.")]
    public string DialogueText;
    [Tooltip("Do not define more than four option fields or an error will occur!")]
    public string[] OptionsMenu;

    [Tooltip("At the end of this dialogue, the player is going to earn the following items")]
    public Item[] ItemsEarned;
}
