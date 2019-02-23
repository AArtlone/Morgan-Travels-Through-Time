using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [NonSerialized]
    public int CurrentDialogueIndex = -1;
    private bool _isDialogueOngoing;

    // We use this list to filter in every branch of dialogue that is
    // available based on their conditions for the player and then we
    // use that list to pick the one with the highest priority.
    private List<DialogueBranch> _availableBranches = new List<DialogueBranch>();
    [NonSerialized]
    public DialogueBranch FinalDialogueBranch = new DialogueBranch();
    private Character _playerCharacterScript;
    private Image _dialogueProgressionTrigger;
    private Image _imageComponent;

    private void Start()
    {
        DialogueManager.Instance.ToggleDialogue(false);
        _playerCharacterScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        _dialogueProgressionTrigger = transform.GetChild(0).GetComponent<Image>();
        _dialogueProgressionTrigger.raycastTarget = false;

        _imageComponent = GetComponent<Image>();
    }

    public void NextDialogue()
    {
        // Loading the new set of dialogue data.
        CurrentDialogueIndex++;

        // We now change both sides of the dialogue and all of its
        // elements to match the new dialogue object's data.
        DialogueManager.Instance.ChangeTitle(
            "left", Dialogue[CurrentDialogueIndex].LeftCharacterTitle);
        DialogueManager.Instance.ChangeTitle(
            "right", Dialogue[CurrentDialogueIndex].RightCharacterTitle);

        DialogueManager.Instance.ChangePortrait("left", Dialogue[CurrentDialogueIndex].LeftCharacterPortrait);
        DialogueManager.Instance.ChangePortrait("right", Dialogue[CurrentDialogueIndex].RightCharacterPortrait);

        // Changing the dialogue box background
        DialogueManager.Instance.ChangeDialogueBoxBackground(Dialogue[CurrentDialogueIndex].DialogueBoxBackground);

        // First we clear the previously selected dialogue branch, otherwise they
        // would add on top of each other and get wrong output to the dialogue sequence.
        _availableBranches.Clear();
        FinalDialogueBranch = new DialogueBranch();

        // Here we loop through every branch of dialogue in the current dialogue
        // sequence and we filter out the ones that are available so we can pick
        // the one with the highest priority for displaying.
        for (int j = 0; j < Dialogue[CurrentDialogueIndex].DialogueBranches.Count; j++)
        {
            // We check if every condition is met by the player before adding this dialogue branch as one for sorting by priority later on.
            if (
                _playerCharacterScript.Reputation >= Dialogue[CurrentDialogueIndex].DialogueBranches[j].ReputationMinimum &&
                _playerCharacterScript.Stamina >= Dialogue[CurrentDialogueIndex].DialogueBranches[j].StaminaMinimum &&
                _playerCharacterScript.Knowledge >= Dialogue[CurrentDialogueIndex].DialogueBranches[j].KnowledgeMinimum &&
                _playerCharacterScript.Fitness >= Dialogue[CurrentDialogueIndex].DialogueBranches[j].FitnessMinimum &&
                _playerCharacterScript.Charisma >= Dialogue[CurrentDialogueIndex].DialogueBranches[j].CharismaMinimum &&
                _playerCharacterScript.Currency >= Dialogue[CurrentDialogueIndex].DialogueBranches[j].CurrencyMinimum)
            {
                // AND we check if we have matching previous responses for that
                // dialogue sequence to be appropriate for use.
                int elementsThatMatch = 0;
                foreach (string requiredResponse in Dialogue[CurrentDialogueIndex].DialogueBranches[j].PreviousResponses)
                {
                    foreach (string response in DialogueManager.Instance.DialogueResponses)
                    {
                        if (requiredResponse == response)
                        {
                            //Debug.Log(string.Format("Match between {0} and {1}", response, requiredResponse));

                            elementsThatMatch++;
                            break;
                        }
                    }
                }

                if (elementsThatMatch == Dialogue[CurrentDialogueIndex].DialogueBranches[j].PreviousResponses.Count)
                {
                    _availableBranches.Add(Dialogue[CurrentDialogueIndex].DialogueBranches[j]);

                    //Debug.Log("Number of elements match!");
                }
            }
        }

        // After all the available branches of the new dialogue sequence are stored
        // we now pick the one with the highest priority.
        int highestPriority = _availableBranches[0].Priority;
        FinalDialogueBranch = _availableBranches[0];

        for (int i = 0; i < _availableBranches.Count; i++)
        {
            if (_availableBranches[i].Priority > highestPriority)
            {
                highestPriority = _availableBranches[i].Priority;
                // We set the branch with the highest priority as our final branch
                // for the new dialogue sequence to display.
                FinalDialogueBranch = _availableBranches[i];
            }
        }

        // And here we display the text of the final selected branch. 
        DialogueManager.Instance.ChangeDialogueText(FinalDialogueBranch.DialogueText);

        // We clear the options menu before we populate it with data, because if
        // there is no data, we still want to clear it from its previous dialogue
        // options menu data
        DialogueManager.Instance.ClearOptionsMenu();
        // Changing the dialogue options menu buttons
        DialogueManager.Instance.ChangeOptionsMenu(FinalDialogueBranch.OptionsMenu);

        //Debug.LogWarning("New dialogue is loaded!");
    }

    public void ContinueDialogue()
    {
        // When the player taps on the npc or anywhere on the dialogue box, it
        // will progress the dialogue further.
        if (_isDialogueOngoing)
        {
            if (CurrentDialogueIndex < Dialogue.Count - 1)
            {
                foreach (DialogueBranch branch in Dialogue[CurrentDialogueIndex].DialogueBranches)
                {
                    if (branch.ItemsRequired.Count == branch.ItemsDropped.Count)
                    {
                        NextDialogue();
                        return;
                    } else
                    {
                        // If we reach past the dialogue, then we reset the
                        // current dialogue index counter and we hide the dialogue
                        // until it is once again triggered to show up.
                        CurrentDialogueIndex = -1;
                        DialogueManager.Instance.ToggleDialogue(false);
                        // We only update the player's inventory AFTER he has exited
                        // the dialogue, otherwise if he had used any items in it and
                        // quit or restarted the game, he would lose them forever.
                        _playerCharacterScript.RefreshItems();
                        _dialogueProgressionTrigger.raycastTarget = false;
                        _imageComponent.raycastTarget = true;

                        _isDialogueOngoing = false;
                        DialogueManager.Instance.CurrentNPCDialogue = null;
                        return;
                    }
                }
            } else
            {
                CurrentDialogueIndex = -1;
                DialogueManager.Instance.ToggleDialogue(false);
                _playerCharacterScript.RefreshItems();
                _dialogueProgressionTrigger.raycastTarget = false;
                _imageComponent.raycastTarget = true;

                _isDialogueOngoing = false;
                return;
            }
        } else 
        // This is meant to initialize the dialogue once the player 
        // has tapped on top of an npc icon.
        {

            _imageComponent.raycastTarget = false;
            // Once a dialogue is initiated, we display the dialogue template
            // and give it the starting dialogue object's data. We also make sure
            // to set _isDialogueOngoing to true because the player is still able
            // to increment the current dialogue index number without having the
            // dialogue visualized in the first place by clicking space.
            DialogueManager.Instance.ToggleDialogue(true);
            NextDialogue();

            // We also want to store which NPC has been responsible for initiating
            // the dialogue, so that we can later access its branching data for
            // storing dragged and dropped items
            DialogueManager.Instance.CurrentNPCDialogue = this;

            // The npc child is used as a field that encapsulates the screen once
            // the dialogue is initiated, because we want to player to progress
            // further by clicking anywhere on the screen, not just on the NPC icon.
            // Once the dialogue is finished we disable this child's interaction.
            _dialogueProgressionTrigger.raycastTarget = true;

            _isDialogueOngoing = true;
        }
    }
}
