using LitJson;
using Newtonsoft.Json;
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
    public Transform _TransformToMoveTo;
    public bool NeedsToMove;
    private bool _canWalkToNextPosition;
    private bool _canInteractWithPlayer = true;
    [SerializeField]
    public List<DialogueLanguages> DialogueFormats = new List<DialogueLanguages>();

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
    List<Dialogue> dialoguesToPickFrom = new List<Dialogue>();
    List<Dialogue> FinalSequence = new List<Dialogue>();
    private string SceneToLoadAfterDialogue;

    private string _languagedPicked;
    private int _dialoguePickedIndex;
    private int _dialogueBranchPickedIndex;

    // ANIMATION-related references
    private Animator _animator;

    private DHManager _DHManager;
    private MapEnvironmentManager _mapEnvironmentManager;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _DHManager = FindObjectOfType<DHManager>();
        _mapEnvironmentManager = FindObjectOfType<MapEnvironmentManager>();

        DialogueManager.Instance.ToggleDialogue(false);
        if (DialogueProgressionTrigger2D == null)
        {
            _dialogueProgressionTrigger = transform.GetChild(0).GetComponent<Image>();
            //_dialogueProgressionTrigger.raycastTarget = false;
        }
        else
        {
            _dialogueProgressionTrigger = DialogueProgressionTrigger2D;
        }

        _imageComponent = GetComponent<Image>();

        CheckForNotCompletedObjective();
    }

    private void Update()
    {
        foreach (Quest quest in Character.Instance.AllQuests)
        {
            if (quest.Name == "1672???")
            {
                foreach (Objective objective in quest.Objectives)
                {
                    if (objective.ID == 0 && objective.CompletedStatus == true)
                    {
                        _canWalkToNextPosition = true;
                    }
                }
            }
        }

        if (_canWalkToNextPosition && !_isDialogueOngoing && NeedsToMove)
        {
            // Once the npc starts walking, we toggle his animations
            _animator.SetBool("IsWalking", true);
            _canInteractWithPlayer = false;
            if (SpeechBubble != null)
            {
                SpeechBubble.GetComponent<SpriteRenderer>().enabled = false;
            }

            if (_TransformToMoveTo != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(_TransformToMoveTo.position.x, transform.position.y), 5f * .5f * Time.deltaTime);
                transform.localScale = new Vector3(-0.5f, 0.5f, 1);

                if (Vector2.Distance(transform.position, new Vector2(_TransformToMoveTo.position.x, transform.position.y)) < 0.1f)
                {
                    _canInteractWithPlayer = true;
                    if (SpeechBubble != null)
                    {
                        SpeechBubble.GetComponent<SpriteRenderer>().enabled = true;
                    }

                    _animator.SetBool("IsWalking", false);
                    transform.localScale = new Vector3(0.5f, 0.5f, 1);
                }
            }
        }
    }
    /// <summary>
    /// This function displays the new dialogue branch that has been decided to be
    /// displayed relative to the player's progress in the game.
    /// </summary>
    public void NextDialogue()
    {
        if (DialogueManager.Instance.IsDialogueTextLoaded)
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
            if (FinalSequence[CurrentDialogueIndex].MoveRightPortraitDown)
            {
                DialogueManager.Instance.OffSetPortrait("right");
            }

            DialogueManager.Instance.ChangeFaceExpression(FinalSequence[CurrentDialogueIndex].LeftCharacterExpression.ToString());

            DialogueManager.Instance.ChangeNPCFaceExpression(Name, FinalSequence[CurrentDialogueIndex].RightCharacterExpression.ToString());

            //Offsetting portraits based on who is talking
            if (FinalSequence[CurrentDialogueIndex].LeftPortraitTalking)
            {
                //DialogueManager.Instance.OffSetPortrait("left");
                DialogueManager.Instance.HightlightAndDarkenSpeakers("left");
            }
            else if (FinalSequence[CurrentDialogueIndex].RightPortraitTalking)
            {
                //DialogueManager.Instance.OffSetPortrait("right");
                DialogueManager.Instance.HightlightAndDarkenSpeakers("right");
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
                        string itemsEngData = File.ReadAllText(Application.persistentDataPath + "/Items.json");
                        JsonData data = JsonMapper.ToObject(itemsEngData);

                        for (int l = 0; l < data["Items"].Count; l++)
                        {
                            if (data["Items"][l]["Name"].ToString() == itemRequired)
                            {
                                itemsMatching++;
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

                    int milestonesThatMatch = 0;
                    foreach (string milestoneRequired in FinalSequence[CurrentDialogueIndex].DialogueBranches[j].MilestonesRequired)
                    {
                        foreach (ProgressEntry log in ProgressLog.Instance.Log)
                        {
                            if (milestoneRequired == log.Milestone && log.Completed == true)
                            {
                                milestonesThatMatch++;
                            }
                        }
                    }

                    if ((elementsThatMatch == FinalSequence[CurrentDialogueIndex].DialogueBranches[j].PreviousResponses.Count) && (objectivesThatMatch == FinalSequence[CurrentDialogueIndex].DialogueBranches[j].ObjectivesRequired.Count) &&
                    (itemsMatching == FinalSequence[CurrentDialogueIndex].DialogueBranches[j].ItemsRequired.Count) &&
                    (milestonesThatMatch == FinalSequence[CurrentDialogueIndex].DialogueBranches[j].MilestonesRequired.Count))
                    {
                        _availableBranches.Add(FinalSequence[CurrentDialogueIndex].DialogueBranches[j]);
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
                    _dialogueBranchPickedIndex = i;
                }
            }

            // And here we display the text of the final selected branch. 
            DialogueManager.Instance.ChangeDialogueText(FinalDialogueBranch.DialogueText);

            _dialogueBranchPickedIndex = FinalDialogueBranch.ID;

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

                    if (dialogueItem.Name == "Map")
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
            }
            else
            {
                DialogueProgressionTrigger2D.raycastTarget = true;
                _dialogueProgressionTrigger.raycastTarget = true;
            }

            foreach (string milestoneToComplete in FinalDialogueBranch.MilestonesToComplete)
            {
                foreach (ProgressEntry milestone in ProgressLog.Instance.Log)
                {
                    if (milestoneToComplete == milestone.Milestone)
                    {
                        ProgressLog.Instance.SetEntry(milestone.Milestone, true);
                    }
                }
            }

            Character.Instance.CompleteObjectiveInQuest(FinalDialogueBranch.ObjectiveToComplete, FinalDialogueBranch.QuestOfDialogue);

            if (FinalDialogueBranch.SceneToLoad != string.Empty)
            {
                SceneToLoadAfterDialogue = FinalDialogueBranch.SceneToLoad;
            }

            DialogueLogManager.Instance.AddNewDialogue(Name, SettingsManager.Instance.Language, FinalDialogueBranch.DialogueText);

            foreach (DialogueLanguages format in DialogueFormats)
            {
                if (format.Language != SettingsManager.Instance.Language)
                {
                    foreach (Dialogue dialogue in format.Dialogue)
                    {
                        if (dialogue.ID == FinalSequence[CurrentDialogueIndex].ID)
                        {
                            foreach (DialogueBranch dialogueBranch in dialogue.DialogueBranches)
                            {
                                if (dialogueBranch.ID == FinalDialogueBranch.ID)
                                {
                                    string messageConvertedForJson = JsonConvert.ToString(dialogueBranch.DialogueText);
                                    messageConvertedForJson = messageConvertedForJson.Substring(1, messageConvertedForJson.Length - 2);

                                    DialogueLogManager.Instance.AddNewDialogue(Name, format.Language, messageConvertedForJson);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            DialogueManager.Instance.ChangeDialogueTextRapidly(FinalDialogueBranch.DialogueText);
        }
    }

    /// <summary>
    /// This function runs functions necessary to display the next dialogue and 
    /// dialogue branch for the current dialogue based on the player's progress in
    /// the game.
    /// </summary>
    public void ContinueDialogue()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.NewPageInDiary);

        if (_mapEnvironmentManager != null)
        {
            _mapEnvironmentManager.BackpackButton.GetComponent<Canvas>().sortingOrder = 100;

            if (InterfaceManager.Instance.InventoryUICanvas != null)
            {
                InterfaceManager.Instance.InventoryUICanvas.sortingOrder = 99;
            }
        }

        if (_canInteractWithPlayer)
        {
            int index = 0;
            for (int i = 0; i < DialogueFormats.Count; i++)
            {
                if (DialogueFormats[i].Language == SettingsManager.Instance.Language)
                {
                    index = i;
                    _languagedPicked = SettingsManager.Instance.Language;
                    break;
                }
            }

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
                            }
                        }
                    }
                }

                int itemsMatching = 0;
                foreach (string itemRequired in DialogueFormats[index].Dialogue[i].RequiredItems)
                {
                    string dataToJson = File.ReadAllText(Application.persistentDataPath + "/Items.json");
                    JsonData data = JsonMapper.ToObject(dataToJson);

                    for (int j = 0; j < data["Items"].Count; j++)
                    {
                        if (data["Items"][j]["Name"].ToString() == itemRequired)
                        {
                            itemsMatching++;
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

                int milestonesThatMatch = 0;
                foreach (ProgressEntry milestoneRequired in DialogueFormats[index].Dialogue[i].RequiredMilestones)
                {
                    foreach (ProgressEntry log in ProgressLog.Instance.Log)
                    {
                        if (milestoneRequired.Milestone == log.Milestone && log.Completed == milestoneRequired.Completed)
                        {
                            milestonesThatMatch++;
                        }
                    }
                }

                if (objectivesCompleted == DialogueFormats[index].Dialogue[i].ObjectivesToMeet.Count &&
                    itemsMatching == DialogueFormats[index].Dialogue[i].RequiredItems.Count &&
                    milestonesThatMatch == DialogueFormats[index].Dialogue[i].RequiredMilestones.Count)
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

            // When the player taps on the npc or anywhere on the dialogue box, it
            // will progress the dialogue further.
            if (_isDialogueOngoing && DialogueManager.Instance.IsDialogueTextLoaded)
            {
                if (CurrentDialogueIndex < FinalSequence.Count - 1)
                {
                    foreach (DialogueBranch branch in FinalSequence[CurrentDialogueIndex].DialogueBranches)
                    {
                        if (branch.ItemsRequired.Count == branch.ItemsDropped.Count)
                        {
                            _dialoguePickedIndex = FinalSequence[CurrentDialogueIndex].ID;
                            _dialogueBranchPickedIndex = branch.ID;

                            NextDialogue();

                            return;
                        }
                        else
                        {
                            // TODO: Try to make functions for most of those code blocks.
                            if (_mapEnvironmentManager != null)
                            {
                                _mapEnvironmentManager.BackpackButton.GetComponent<Canvas>().sortingOrder = -1;

                                if (InterfaceManager.Instance.InventoryUICanvas != null)
                                {
                                    InterfaceManager.Instance.InventoryUICanvas.sortingOrder = -2;
                                }
                            }

                            // This does not work now since we have the scene
                            // reload as band-aid fix for resetting the HOP.
                            foreach (Item dialogueItem in FinalDialogueBranch.ItemsEarned)
                            {
                                if (dialogueItem.Name == "Map")
                                {
                                    _DHManager.LoadSequence("Teach Map Icon");
                                }
                            }

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

                            return;
                        }
                    }
                }
                else
                {
                    if (_mapEnvironmentManager != null)
                    {
                        _mapEnvironmentManager.BackpackButton.GetComponent<Canvas>().sortingOrder = -1;

                        if (InterfaceManager.Instance.InventoryUICanvas != null)
                        {
                            InterfaceManager.Instance.InventoryUICanvas.sortingOrder = -2;
                        }
                    }

                    foreach (Item dialogueItem in FinalDialogueBranch.ItemsEarned)
                    {
                        if (dialogueItem.Name == "Map")
                        {
                            _DHManager.LoadSequence("Teach Map Icon");
                        }
                    }

                    if (SceneToLoadAfterDialogue != null)
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
                    return;
                }
            }
            else
            // This is meant to initialize the dialogue once the player 
            // has tapped on top of an npc icon.
            {
                //if (_isDialogueOngoing)
                //{
                //    DialogueManager.Instance.ChangeDialogueTextRapidly(FinalDialogueBranch.DialogueText);
                //}

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
        if (SceneToLoadAfterDialogue == "Tutorial Complete")
        {
            InterfaceManager.Instance.FadeEndTutorial();
        }
        if (SceneManager.GetActiveScene().name == "Castle Area")
        {
            Character.Instance.LastMapArea = sceneToLoad;
        }
        InterfaceManager.Instance.LoadingScreen.SetActive(true);

        InterfaceManager.Instance.LoadScene(sceneToLoad);
        yield return new WaitForEndOfFrame();
        /*AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoad);
        yield return new WaitForSeconds(1.2f);*/
    }

    /// <summary>
    /// This function runs in order to filter out dialogue that the player is not
    /// yet qualified to view depending on his progress.
    /// </summary>
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

                    int milestonesThatMatch = 0;
                    foreach (string milestoneRequired in dialogue.DialogueBranches[j].MilestonesRequired)
                    {
                        foreach (ProgressEntry log in ProgressLog.Instance.Log)
                        {
                            if (milestoneRequired == log.Milestone && log.Completed == true)
                            {
                                milestonesThatMatch++;
                            }
                        }
                    }

                    if ((elementsThatMatch == dialogue.DialogueBranches[j].PreviousResponses.Count) &&
                        (objectivesThatMatch != dialogue.DialogueBranches[j].ObjectivesRequired.Count) &&
                        (milestonesThatMatch != dialogue.DialogueBranches[j].MilestonesRequired.Count))
                    {
                        branchesForFiltering.Add(dialogue.DialogueBranches[j]);
                    }
                }
            }
        }

        if (SpeechBubble != null)
        {
            if (branchesForFiltering.Count > 0)
            {
                SpeechBubble.SetActive(false);
            }
            else
            {
                SpeechBubble.SetActive(true);
            }
        }
    }

    //private void AddCorrespondingDialogueForLanguage(string language)
    //{

    //}
}