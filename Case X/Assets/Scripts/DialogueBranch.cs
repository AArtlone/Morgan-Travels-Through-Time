using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueBranch
{
    public string BranchTitle;
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
    [TextArea(0, 100)]
    [Space(10)]
    [Tooltip("Text that will be displayed on the current dialogue sequence if t hat branch's requirements are met.")]
    public string DialogueText;
    [Tooltip("Do not define more than four option fields or an error will occur!")]
    public string[] OptionsMenu;
}
