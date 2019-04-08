using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    // Its useful to have these variables besides the dialogue because
    // if we want to extend that information to the main map, it would
    // prove to be useful and helpful to the player.
    public string Name;
    // Used for detecting clicks on that object's image (region of space on camera)
    public Image DialogueProgressionTrigger2D;
    public GameObject SpeechBubble;
    public Vector2 _posToMoveTo;
    private bool _canWalkToNextPosition;
    private bool _canInteractWithPlayer = true;
    [SerializeField]
    public List<DialogueLanguages> DialogueFormats;

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
    private Image _dialogueProgressionTrigger;
    private Image _imageComponent;
    private SpriteRenderer _spriteComponent;
    List<Dialogue> dialoguesToPickFrom = new List<Dialogue>();
    List<Dialogue> FinalSequence = new List<Dialogue>();
    private SwipeController _swipeController;
    private string SceneToLoadAfterDialogue;

    private void Start()
    {
        _swipeController = FindObjectOfType<SwipeController>();
        DialogueManager.Instance.ToggleDialogue(false);
        if (DialogueProgressionTrigger2D == null)
        {
            _dialogueProgressionTrigger = transform.GetChild(0).GetComponent<Image>();
            _dialogueProgressionTrigger.raycastTarget = false;
        } else
        {
            _dialogueProgressionTrigger = DialogueProgressionTrigger2D;
        }

        _spriteComponent = GetComponent<SpriteRenderer>();
        _imageComponent = GetComponent<Image>();

        CheckForNotCompletedObjective();
    }

    private void Update()
    {
        foreach(Quest quest in Character.Instance.AllQuests)
        {
            if(quest.Name == "1672???")
            {
                foreach(Objective objective in quest.Objectives)
                {
                    if(objective.ID == 0)
                    {
                        if(objective.CompletedStatus == true)
                        {
                            _canWalkToNextPosition = true;
                        }
                    }
                }
            }
        }
        if(_canWalkToNextPosition && !_isDialogueOngoing)
        {
            _canInteractWithPlayer = false;
            SpeechBubble.SetActive(false);
            transform.position = Vector2.MoveTowards(transform.position, _posToMoveTo, 15f * .5f * Time.deltaTime);
            if(Vector2.Distance(transform.position, _posToMoveTo) < 1f)
            {
                _canInteractWithPlayer = true;
                SpeechBubble.SetActive(true);
            }
        }
    }

    public void NextDialogue()
    {
        // Loading the new set of dialogue data.
        CurrentDialogueIndex++;

        //Dialogue[CurrentDialogueIndex].Passed = true;

        // We now change both sides of the dialogue and all of its
        // elements to match the new dialogue object's data.
        DialogueManager.Instance.ChangeTitle(
            "left", Character.Instance.Name);
        DialogueManager.Instance.ChangeTitle(
            "right", FinalSequence[CurrentDialogueIndex].RightCharacterTitle);

        DialogueManager.Instance.ChangePortrait("left", FinalSequence[CurrentDialogueIndex].LeftCharacterPortrait);
        DialogueManager.Instance.ChangePortrait("right", FinalSequence[CurrentDialogueIndex].RightCharacterPortrait);
        
        //Offsetting portraits based on who is talking
        if(FinalSequence[CurrentDialogueIndex].LeftPortraitTalking)
        {
            DialogueManager.Instance.OffSetPortrait("left");
        } else if (FinalSequence[CurrentDialogueIndex].RightPortraitTalking)
        {
            DialogueManager.Instance.OffSetPortrait("right");
        }

        // Changing the dialogue box background
        DialogueManager.Instance.ChangeDialogueBoxBackground(FinalSequence[CurrentDialogueIndex].DialogueBoxBackground);

        DialogueManager.Instance.ChangeDialogueStageBackground(FinalSequence[CurrentDialogueIndex].DialogueStageBackground);

        // First we clear the previously selected dialogue branch, otherwise they
        // would add on top of each other and get wrong output to the dialogue sequence.
        _availableBranches.Clear();
        FinalDialogueBranch = new DialogueBranch();

        // Here we loop through every branch of dialogue in the current dialogue
        // sequence and we filter out the ones that are available so we can pick
        // the one with the highest priority for displaying.
        for (int j = 0; j < FinalSequence[CurrentDialogueIndex].DialogueBranches.Count; j++)
        {
            // We check if every condition is met by the player before adding this dialogue branch as one for sorting by priority later on.
            if (
                Character.Instance.Reputation >= FinalSequence[CurrentDialogueIndex].DialogueBranches[j].ReputationMinimum &&
                Character.Instance.Stamina >= FinalSequence[CurrentDialogueIndex].DialogueBranches[j].StaminaMinimum &&
                Character.Instance.Knowledge >= FinalSequence[CurrentDialogueIndex].DialogueBranches[j].KnowledgeMinimum &&
                Character.Instance.Fitness >= FinalSequence[CurrentDialogueIndex].DialogueBranches[j].FitnessMinimum &&
                Character.Instance.Charisma >= FinalSequence[CurrentDialogueIndex].DialogueBranches[j].CharismaMinimum &&
                Character.Instance.Currency >= FinalSequence[CurrentDialogueIndex].DialogueBranches[j].CurrencyMinimum)
            {
                // AND we check if we have matching previous responses for that
                // dialogue sequence to be appropriate for use.
                int elementsThatMatch = 0;
                foreach (string requiredResponse in FinalSequence[CurrentDialogueIndex].DialogueBranches[j].PreviousResponses)
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

                int objectivesThatMatch = 0;
                foreach (Objective objective in FinalSequence[CurrentDialogueIndex].DialogueBranches[j].ObjectivesRequired)
                {
                    foreach (Quest quest in Character.Instance.AllQuests)
                    {
                        foreach (Objective objOfCharacter in quest.Objectives)
                        {
                            if (objOfCharacter.Name == objective.Name &&
                                objOfCharacter.CompletedStatus == true)
                            {
                                objectivesThatMatch++;
                            }
                        }
                    }
                    foreach (Quest quest in Character.Instance.AllQuestsDutch)
                    {
                        foreach (Objective objOfCharacter in quest.Objectives)
                        {
                            if (objOfCharacter.Name == objective.Name &&
                                objOfCharacter.CompletedStatus == true)
                            {
                                objectivesThatMatch++;
                            }
                        }
                    }
                }

                int itemsMatching = 0;
                foreach (string itemRequired in FinalSequence[CurrentDialogueIndex].DialogueBranches[j].ItemsRequired)
                {
                    //Debug.Log("Required: " + itemRequired);
                    string itemsEngData = File.ReadAllText(Application.persistentDataPath + "/Items.json");
                    JsonData data = JsonMapper.ToObject(itemsEngData);

                    for (int l = 0; l < data["Items"].Count; l++)
                    {
                        //Debug.Log("Have " + (data["Items"][j]["Name"].ToString()));
                        if (data["Items"][l]["Name"].ToString() == itemRequired)
                        {
                            itemsMatching++;
                            //Debug.Log("MATCH");
                        }
                    }
                    
                    string itemsDutchData = File.ReadAllText(Application.persistentDataPath + "/ItemsDutch.json");
                    data = JsonMapper.ToObject(itemsDutchData);

                    for (int k = 0; k < data["Items"].Count; k++)
                    {
                        if (data["Items"][k]["Name"].ToString() == itemRequired)
                        {
                            itemsMatching++;
                        }
                    }
                }

                if ((elementsThatMatch == FinalSequence[CurrentDialogueIndex].DialogueBranches[j].PreviousResponses.Count) && (objectivesThatMatch == FinalSequence[CurrentDialogueIndex].DialogueBranches[j].ObjectivesRequired.Count) &&
                itemsMatching == FinalSequence[CurrentDialogueIndex].DialogueBranches[j].ItemsRequired.Count)
                {
                    _availableBranches.Add(FinalSequence[CurrentDialogueIndex].DialogueBranches[j]);

                    //Debug.Log(FinalSequence[CurrentDialogueIndex].DialogueBranches[j].BranchTitle);
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

        // We also activate all the game objects in this branch once it is visualized.
        foreach (GameObject entity in FinalDialogueBranch.EntitiesToActivate)
        {
            if (entity != null)
            {
                entity.SetActive(true);
            }
        }
        foreach (GameObject entity in FinalDialogueBranch.EntitiesToDeactive)
        {
            if (entity != null)
            {
                entity.SetActive(false);
            }
        }

        // We clear the options menu before we populate it with data, because if
        // there is no data, we still want to clear it from its previous dialogue
        // options menu data
        DialogueManager.Instance.ClearOptionsMenu();
        // Changing the dialogue options menu buttons
        DialogueManager.Instance.ChangeOptionsMenu(FinalDialogueBranch.OptionsMenu);
        
        bool areItemsEarnedAlready = false;

        string dataToJson = File.ReadAllText(Application.persistentDataPath + "/Items.json");
        JsonData itemsJsonData = JsonMapper.ToObject(dataToJson);

        for (int i = 0; i < itemsJsonData["Items"].Count; i++)
        {
            foreach (Item dialogueItem in FinalDialogueBranch.ItemsEarned)
            {
                if (itemsJsonData["Items"][i]["Name"].ToString() == dialogueItem.Name)
                {
                    //Debug.Log("Item " + dialogueItem.Name + " is already in possession!");
                    areItemsEarnedAlready = true;
                    break;
                }
            }

            if (areItemsEarnedAlready)
            {
                break;
            }
        }

        if (areItemsEarnedAlready == false)
        {
            foreach (Item dialogueItem in FinalDialogueBranch.ItemsEarned)
            {
                Character.Instance.AddItem(dialogueItem);
                areItemsEarnedAlready = true;
                Debug.Log("Item " + dialogueItem.Name + " received!");
                if(dialogueItem.Name == "Map")
                {
                    Character.Instance.HasMap = true;
                    Character.Instance.RefreshJsonData();
                    FindObjectOfType<MapEnvironmentManager>().LoadObjectsFromSequence();
                }
                if (dialogueItem.Name == "Diary")
                {
                    Character.Instance.HasDiary = true;
                    Character.Instance.RefreshJsonData();
                    FindObjectOfType<MapEnvironmentManager>().LoadObjectsFromSequence();
                }
            }
        }

        if (FinalDialogueBranch.OptionsMenu.Length > 0)
        {
            DialogueProgressionTrigger2D.raycastTarget = false;
            _dialogueProgressionTrigger.raycastTarget = false;
        } else
        {
            DialogueProgressionTrigger2D.raycastTarget = true;
            _dialogueProgressionTrigger.raycastTarget = true;
        }
        
        Character.Instance.CompleteObjectiveInQuest(FinalDialogueBranch.ObjectiveToComplete, FinalDialogueBranch.QuestOfDialogue);

        if (FinalDialogueBranch.SceneToLoad != string.Empty)
        {
            SceneToLoadAfterDialogue = FinalDialogueBranch.SceneToLoad;
        }
        
        //Debug.LogWarning("New dialogue is loaded!");
    }

    public void ContinueDialogue()
    {
        if (_canInteractWithPlayer)
        {
            int index = 0;
            for (int i = 0; i < DialogueFormats.Count; i++)
            {
                if (DialogueFormats[i].Language == SettingsManager.Instance.Language)
                {
                    index = i;
                    break;
                }
            }

            //Debug.Log(DialogueFormats[index].Language);

            dialoguesToPickFrom.Clear();
            for (int i = 0; i < DialogueFormats[index].Dialogue.Count; i++)
            {
                int objectivesCompleted = 0;
                foreach (Objective objective in DialogueFormats[index].Dialogue[i].ObjectivesToMeet)
                {
                    foreach (Quest quest in Character.Instance.AllQuests)
                    {
                        foreach (Objective characterObjective in quest.Objectives)
                        {
                            if (objective.Name == characterObjective.Name &&
                                characterObjective.CompletedStatus == true)
                            {
                                objectivesCompleted++;
                                //Debug.Log(objective.Name);
                            }
                        }
                    }
                    foreach (Quest quest in Character.Instance.AllQuestsDutch)
                    {
                        foreach (Objective characterObjective in quest.Objectives)
                        {
                            if (objective.Name == characterObjective.Name &&
                                characterObjective.CompletedStatus == true)
                            {
                                objectivesCompleted++;
                                //Debug.Log(objective.Name);
                            }
                        }
                    }
                }

                int itemsMatching = 0;
                foreach (string itemRequired in DialogueFormats[index].Dialogue[i].RequiredItems)
                {
                    //Debug.Log("Required: " + itemRequired);
                    string dataToJson = File.ReadAllText(Application.persistentDataPath + "/Items.json");
                    JsonData data = JsonMapper.ToObject(dataToJson);

                    for (int j = 0; j < data["Items"].Count; j++)
                    {
                        //Debug.Log("Have " + (data["Items"][j]["Name"].ToString()));
                        if (data["Items"][j]["Name"].ToString() == itemRequired)
                        {
                            itemsMatching++;
                            //Debug.Log("MATCH");
                        }
                    }

                    string itemsDutchData = File.ReadAllText(Application.persistentDataPath + "/ItemsDutch.json");
                    data = JsonMapper.ToObject(itemsDutchData);

                    for (int k = 0; k < data["Items"].Count; k++)
                    {
                        if (data["Items"][k]["Name"].ToString() == itemRequired)
                        {
                            itemsMatching++;
                        }
                    }
                }

                //Debug.Log(objectivesCompleted + " | " + (Dialogue[i].ObjectivesToMeet.Count));
                if (objectivesCompleted == DialogueFormats[index].Dialogue[i].ObjectivesToMeet.Count &&
                    itemsMatching == DialogueFormats[index].Dialogue[i].RequiredItems.Count)
                {
                    dialoguesToPickFrom.Add(DialogueFormats[index].Dialogue[i]);
                }
            }

            FinalSequence.Clear();
            int highestPriorityOutOfDialogues = 0;
            foreach (Dialogue dialogue in dialoguesToPickFrom)
            {
                if (dialogue.PriorityIndex > highestPriorityOutOfDialogues)
                {
                    highestPriorityOutOfDialogues = dialogue.PriorityIndex;
                }
            }

            foreach (Dialogue dialogue in dialoguesToPickFrom)
            {
                if (dialogue.PriorityIndex == highestPriorityOutOfDialogues)
                {
                    FinalSequence.Add(dialogue);
                }
            }

            foreach (Dialogue dia in FinalSequence)
            {
                //Debug.Log("Dialogue matched: " + dia.DialogueTitle);
            }

            // PRIORITY

            // When the player taps on the npc or anywhere on the dialogue box, it
            // will progress the dialogue further.
            if (_isDialogueOngoing)
            {
                if (CurrentDialogueIndex < FinalSequence.Count - 1)
                {
                    foreach (DialogueBranch branch in FinalSequence[CurrentDialogueIndex].DialogueBranches)
                    {
                        if (branch.ItemsRequired.Count == branch.ItemsDropped.Count)
                        {
                            NextDialogue();
                            return;
                        }
                        else
                        {
                            // If we reach past the dialogue, then we reset the
                            // current dialogue index counter and we hide the dialogue
                            // until it is once again triggered to show up.
                            CurrentDialogueIndex = -1;
                            DialogueManager.Instance.ToggleDialogue(false);
                            // We only update the player's inventory AFTER he has exited
                            // the dialogue, otherwise if he had used any items in it and
                            // quit or restarted the game, he would lose them forever.
                            //Character.Instance.RefreshItems();
                            _dialogueProgressionTrigger.raycastTarget = false;
                            if (_imageComponent)
                            {
                                _imageComponent.raycastTarget = true;
                            }

                            _isDialogueOngoing = false;
                            DialogueManager.Instance.CurrentNPCDialogue = null;
                            Character.Instance.InitiateInteraction();
                            _swipeController.enabled = true;
                            
                            return;
                        }
                    }
                }
                else
                {
                    if (SceneToLoadAfterDialogue != string.Empty)
                    {
                        StartCoroutine(LoadSceneCo(SceneToLoadAfterDialogue));
                    }
                    CurrentDialogueIndex = -1;
                    DialogueManager.Instance.ToggleDialogue(false);
                    //Character.Instance.RefreshItems();
                    _dialogueProgressionTrigger.raycastTarget = false;
                    if (_imageComponent)
                    {
                        _imageComponent.raycastTarget = true;
                    }

                    _isDialogueOngoing = false;
                    Character.Instance.InitiateInteraction();
                    if (_swipeController != null)
                    {
                        _swipeController.enabled = true;
                    }
                    return;
                }
            }
            else
            // This is meant to initialize the dialogue once the player 
            // has tapped on top of an npc icon.
            {

                if (_imageComponent)
                {
                    _imageComponent.raycastTarget = false;
                }
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
                //_dialogueProgressionTrigger.raycastTarget = true;

                _isDialogueOngoing = true;
            }
        }
    }

    private IEnumerator LoadSceneCo(string sceneToLoad)
    {
        Debug.Log(sceneToLoad);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneToLoad);
    }

    private void CheckForNotCompletedObjective()
    {
        List<Dialogue> dialogueToFilter = new List<Dialogue>();
        foreach (DialogueLanguages dialogueFormat in DialogueFormats)
        {
            switch (SettingsManager.Instance.Language)
            {
                case "English":
                    dialogueToFilter = dialogueFormat.Dialogue;
                    break;
                case "Dutch":
                    dialogueToFilter = dialogueFormat.Dialogue;
                    break;
            }
        }

        List<DialogueBranch> branchesForFiltering = new List<DialogueBranch>();
        foreach (Dialogue dialogue in dialogueToFilter)
        {
            for (int j = 0; j < dialogue.DialogueBranches.Count; j++)
            {
                // We check if every condition is met by the player before adding this dialogue branch as one for sorting by priority later on.
                if (
                    Character.Instance.Reputation >= dialogue.DialogueBranches[j].ReputationMinimum &&
                    Character.Instance.Stamina >= dialogue.DialogueBranches[j].StaminaMinimum &&
                    Character.Instance.Knowledge >= dialogue.DialogueBranches[j].KnowledgeMinimum &&
                    Character.Instance.Fitness >= dialogue.DialogueBranches[j].FitnessMinimum &&
                    Character.Instance.Charisma >= dialogue.DialogueBranches[j].CharismaMinimum &&
                    Character.Instance.Currency >= dialogue.DialogueBranches[j].CurrencyMinimum)
                {
                    // AND we check if we have matching previous responses for that
                    // dialogue sequence to be appropriate for use.
                    int elementsThatMatch = 0;
                    foreach (string requiredResponse in dialogue.DialogueBranches[j].PreviousResponses)
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

                    int objectivesThatMatch = 0;
                    foreach (Objective objective in dialogue.DialogueBranches[j].ObjectivesRequired)
                    {
                        foreach (Quest quest in Character.Instance.AllQuests)
                        {
                            foreach (Objective objOfCharacter in quest.Objectives)
                            {
                                if (objOfCharacter.Name == objective.Name &&
                                    objOfCharacter.CompletedStatus == true)
                                {
                                    objectivesThatMatch++;
                                }
                            }
                        }
                    }

                    if ((elementsThatMatch == dialogue.DialogueBranches[j].PreviousResponses.Count) && (objectivesThatMatch != dialogue.DialogueBranches[j].ObjectivesRequired.Count))
                    {
                        branchesForFiltering.Add(dialogue.DialogueBranches[j]);

                        //Debug.Log(FinalSequence[CurrentDialogueIndex].DialogueBranches[j].BranchTitle);
                        //Debug.Log("Number of elements match!");
                    }
                }
            }
        }

        if (branchesForFiltering.Count > 0)
        {
            SpeechBubble.SetActive(false);
        } else
        {
            SpeechBubble.SetActive(true);
        }
    }
}