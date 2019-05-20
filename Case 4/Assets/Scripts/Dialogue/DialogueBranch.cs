using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogueBranch
{
    public string BranchTitle;
    public int ID;
    public List<string> PreviousResponses = new List<string>();
    //[Header("Defined below are fields that the character must meet in order to see this dialogue branch.")]
    [Range(0, 10)]
    [Tooltip("Priority decides whether this branch should be displayed compared to others that falls into the same player requirements.")]
    public int Priority;
    [Space(10)]
    [Tooltip("Defines the minimum requirement for player reputation.")]
    [NonSerialized]
    public int ReputationMinimum;
    [Tooltip("Defines the minimum requirement for player stamina.")]
    [NonSerialized]
    public int StaminaMinimum;
    [Tooltip("Defines the minimum requirement for player knowledge.")]
    [NonSerialized]
    public int KnowledgeMinimum;
    [Tooltip("Defines the minimum requirement for player fitness.")]
    [NonSerialized]
    public int FitnessMinimum;
    [Tooltip("Defines the minimum requirement for player charisma.")]
    [NonSerialized]
    public int CharismaMinimum;
    [Tooltip("Defines the minimum requirement for player currency.")]
    [NonSerialized]
    public int CurrencyMinimum;
    [Tooltip("To pick an item correctly, you use the Resources -> Items folder for items and write down an item's name without its file type in the field.")]
    public List<string> ItemsRequired = new List<string>();
    [Tooltip("This shows the items the player has dragged and dropped into the current dialogue sequence.")]
    public List<string> ItemsDropped = new List<string>();
    [TextArea(0, 100)]
    [Space(10)]
    [Tooltip("Text that will be displayed on the current dialogue sequence if that branch's requirements are met.")]
    public string DialogueText;
    [Tooltip("Do not define more than four option fields or an error will occur!")]
    public string[] OptionsMenu;

    [Tooltip("At the end of this dialogue, the player is going to earn the following items")]
    public Item[] ItemsEarned;

    [Tooltip("You can list a number of objectives that the branch will have to be filtered for before it is selected as the one to visualize.")]
    public List<Objective> ObjectivesRequired;
    [Tooltip("So far, you may complete one objective and potentially the quest of all of its objectives are completed before that one.")]
    public string QuestOfDialogue;
    public int ObjectiveToComplete;

    [Tooltip("After the dialogue is reached, it will activate the game objects you have placed in this list, so as to progress the game.")]
    public List<GameObject> EntitiesToActivate;
    [Tooltip("After the dialogue is reached, it will DEactivate the game objects you have placed in this list, so as to progress the game.")]
    public List<GameObject> EntitiesToDeactive;
    [Tooltip("A name of the scene that needs to be loaded after this dialogue branch.")]
    public string SceneToLoad;
}
