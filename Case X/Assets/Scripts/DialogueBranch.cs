using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueBranch
{
    public string BranchTitle;
    [Range(0, 10)]
    public int Priority;
    public int ReputationMinimum;
    public int StaminaMinimum;
    public int KnowledgeMinimum;
    public int FitnessMinimum;
    public int CharismaMinimum;
    public int CurrencyMinimum;
    public string DialogueText;
    public string[] OptionsMenu;
}
