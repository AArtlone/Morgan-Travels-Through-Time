using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    // Its useful to have these variables besides the dialogue because
    // if we want to extend that information to the main map, it would
    // prove to be useful and helpful to the player.
    public string Name;
    [SerializeField]
    public List<Dialogue> Dialogue;

    // Tracks the current dialogue index in the canvas and whether or not
    // the player is currently in a dialogue or not.
    private int _currentDialogueIndex = -1;
    private bool _isDialogueOngoing;

    // We use this list to filter in every branch of dialogue that is
    // available based on their conditions for the player and then we
    // use that list to pick the one with the highest priority.
    private List<DialogueBranch> _availableBranches = new List<DialogueBranch>();
    private DialogueBranch _finalDialogueBranch = new DialogueBranch();
    private Character _playerCharacterScript;

    private void Start()
    {
        DialogueManager.Instance.ToggleDialogue(false);
        _playerCharacterScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
    }

    private void Update()
    {
        // This will have to be a tap eventually to progress the player in the
        // dialogue every time he taps somewhere on the screen.
        if (_isDialogueOngoing == true && Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentDialogueIndex < Dialogue.Count - 1)
            {
                NextDialogue();
            }
            else
            {
                // If we reach past the dialogue, then we reset the
                // current dialogue index counter and we hide the dialogue
                // until it is once again triggered to show up.
                _currentDialogueIndex = -1;
                DialogueManager.Instance.ToggleDialogue(false);
                _isDialogueOngoing = false;
            }
        }

        // This will also be a tap eventually, meant to initialize the
        // dialogue once the player has tapped on top of an npc icon.
        if (_isDialogueOngoing == false && Input.GetKeyDown(KeyCode.X))
        {
            // Once a dialogue is initiated, we display the dialogue template
            // and give it the starting dialogue object's data. We also make sure
            // to set _isDialogueOngoing to true because the player is still able
            // to increment the current dialogue index number without having the
            // dialogue visualized in the first place by clicking space.
            DialogueManager.Instance.ToggleDialogue(true);
            NextDialogue();
            _isDialogueOngoing = true;
        }
    }

    public void NextDialogue()
    {
        // Loading the new set of dialogue data.
        _currentDialogueIndex++;

        // We now change both sides of the dialogue and all of its
        // elements to match the new dialogue object's data.
        DialogueManager.Instance.ChangeTitle(
            "left", Dialogue[_currentDialogueIndex].LeftCharacterTitle);
        DialogueManager.Instance.ChangeTitle(
            "right", Dialogue[_currentDialogueIndex].RightCharacterTitle);

        DialogueManager.Instance.ChangePortrait("left", Dialogue[_currentDialogueIndex].LeftCharacterPortrait);
        DialogueManager.Instance.ChangePortrait("right", Dialogue[_currentDialogueIndex].RightCharacterPortrait);

        // Changing the dialogue box background
        DialogueManager.Instance.ChangeDialogueBoxBackground(Dialogue[_currentDialogueIndex].DialogueBoxBackground);

        // First we clear the previously selected dialogue branch, otherwise they
        // would add on top of each other and get wrong output to the dialogue sequence.
        _availableBranches.Clear();
        _finalDialogueBranch = new DialogueBranch();

        // Here we loop through every branch of dialogue in the current dialogue
        // sequence and we filter out the ones that are available so we can pick
        // the one with the highest priority for displaying.
        for (int j = 0; j < Dialogue[_currentDialogueIndex].DialogueBranches.Count; j++)
        {
            // We check if every condition is met by the player before adding this dialogue branch as one for sorting by priority later on.
            if (
                _playerCharacterScript.Reputation >= Dialogue[_currentDialogueIndex].DialogueBranches[j].ReputationMinimum &&
                _playerCharacterScript.Stamina >= Dialogue[_currentDialogueIndex].DialogueBranches[j].StaminaMinimum &&
                _playerCharacterScript.Knowledge >= Dialogue[_currentDialogueIndex].DialogueBranches[j].KnowledgeMinimum &&
                _playerCharacterScript.Fitness >= Dialogue[_currentDialogueIndex].DialogueBranches[j].FitnessMinimum &&
                _playerCharacterScript.Charisma >= Dialogue[_currentDialogueIndex].DialogueBranches[j].CharismaMinimum &&
                _playerCharacterScript.Currency >= Dialogue[_currentDialogueIndex].DialogueBranches[j].CurrencyMinimum)
            {
                _availableBranches.Add(Dialogue[_currentDialogueIndex].DialogueBranches[j]);
            }
        }

        // After all the available branches of the new dialogue sequence are stored
        // we now pick the one with the highest priority.
        int highestPriority = _availableBranches[0].Priority;
        _finalDialogueBranch = _availableBranches[0];

        for (int i = 0; i < _availableBranches.Count; i++)
        {
            if (_availableBranches[i].Priority > highestPriority)
            {
                highestPriority = _availableBranches[i].Priority;
                // We set the branch with the highest priority as our final branch
                // for the new dialogue sequence to display.
                _finalDialogueBranch = _availableBranches[i];
            }
        }
        // And here we display the text of the final selected branch. 
        DialogueManager.Instance.ChangeDialogueText(_finalDialogueBranch.DialogueText);

        // We clear the options menu before we populate it with data, because if
        // there is no data, we still want to clear it from its previous dialogue
        // options menu data
        DialogueManager.Instance.ClearOptionsMenu();
        // Changing the dialogue options menu buttons
        DialogueManager.Instance.ChangeOptionsMenu(_finalDialogueBranch.OptionsMenu);

        //Debug.LogWarning("New dialogue is loaded!");
    }
}
