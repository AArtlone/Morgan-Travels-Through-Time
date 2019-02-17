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

    // Tracks the current dialogue object in the canvas.
    private int _currentDialogueIndex = -1;
    private bool _isDialogueOngoing;

    private void Start()
    {
        DialogueManager.Instance.ToggleDialogue(false);
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

        // Changing the dialogue text content
        DialogueManager.Instance.ChangeDialogueText(Dialogue[_currentDialogueIndex].DialogueText);

        // We clear the options menu before we populate it with data, because if
        // there is no data, we still want to clear it from its previous dialogue
        // options menu data
        DialogueManager.Instance.ClearOptionsMenu();
        // Changing the dialogue options menu buttons
        DialogueManager.Instance.ChangeOptionsMenu(Dialogue[_currentDialogueIndex].OptionsMenu);

        //Debug.LogWarning("New dialogue is loaded!");
    }
}
