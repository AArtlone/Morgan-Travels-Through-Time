using UnityEngine;
using System;

[Serializable]
public class Item : MonoBehaviour
{
    public string Name;
    public string Description;
    public string Active;
    public string AssetsImageName;

    #region Hidden objects puzzle bools
    // Specifically for determining which items to hint at for the player.
    [NonSerialized]
    public bool IsItemHintedAt;
    [NonSerialized]
    public bool ItemFound;
    #endregion

    private int _timer;

    public Item(string Name, string Description, string Active, string AssetsImageName)
    {
        this.Name = Name;
        this.Description = Description;
        this.Active = Active;
        this.AssetsImageName = AssetsImageName;
    }

    public void HoldingDown()
    {
        InvokeRepeating("CountTimerUp", 0f, 1f);
    }

    public void Release()
    {
        CancelInvoke();
        _timer = 0;
    }

    private void CountTimerUp()
    {
        _timer++;

        if (_timer == 2)
        {
            InterfaceManager.Instance.OpenPopup(InterfaceManager.Instance.ItemDetailsWindow);

            // It is mandatory you save the image from resources into a sprite variable
            // otherwise it still will not convert it properly for the instance manager.
            Sprite sprite = Resources.Load<Sprite>("Items/Inventory/" + AssetsImageName);
            InterfaceManager.Instance.ItemDetailsPortrait.sprite = sprite;
            InterfaceManager.Instance.ItemDetailsName.text = Name;
            InterfaceManager.Instance.ItemDetailsDescription.text = Description;
            InterfaceManager.Instance.ItemDetailsActives.text = Active;
        }
    }

    public void DisplayItemDetails()
    {
        InterfaceManager.Instance.OpenPopup(InterfaceManager.Instance.ItemDetailsWindow);

        // It is mandatory you save the image from resources into a sprite variable
        // otherwise it still will not convert it properly for the instance manager.
        Sprite sprite = Resources.Load<Sprite>("Items/Inventory/" + AssetsImageName);
        InterfaceManager.Instance.ItemDetailsPortrait.sprite = sprite;
        InterfaceManager.Instance.ItemDetailsName.text = Name;
        InterfaceManager.Instance.ItemDetailsDescription.text = Description;
        InterfaceManager.Instance.ItemDetailsActives.text = Active;
    }

    public void DragItem()
    {
        Touch touch = Input.GetTouch(0);
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.position = new Vector2(touch.position.x, touch.position.y);
        // This makes it so that the timer is not increasing if youre holding the
        // item AND dragging it at the same time.
        _timer = 0;
    }
    
    public void DropItem()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // We use the canvas tag for map interactions instead of the dialogue.
            if (hit.transform.tag == "Canvas")
            {
                //Debug.Log("Dropped on top of canvas");
            }

            if (hit.transform.tag == "NPC")
            {
                // TODO: Based on item throw, show a wrong item drop dialogue or the correct one for the main npc dialogue starting from the point after you drop the item.
                if (DialogueManager.Instance.CurrentNPCDialogue == null)
                {
                    bool isCorrectItemDropped = false;
                    NPC npc = hit.transform.gameObject.GetComponent<NPC>();

                    int index = 0;
                    for (int i = 0; i < npc.DialogueFormats.Count; i++)
                    {
                        if (npc.DialogueFormats[i].Language == SettingsManager.Instance.Language)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (npc.DialogueFormats[index].Dialogue[0].DialogueBranches[0].ItemsDropped.Count == 0 &&
                        npc.DialogueFormats[index].Dialogue[0].DialogueBranches[0].ItemsRequired[0] ==
                        Name)
                    {
                        npc.DialogueFormats[index].Dialogue[0].DialogueBranches[0].ItemsDropped.Add(Name);
                        isCorrectItemDropped = true;
                    } else
                    {
                        //Debug.Log("Wrong item!");
                    }
                    if (isCorrectItemDropped)
                    {
                        npc.ContinueDialogue();
                    }
                }
            }

            // Here we check if the item was dragged and dropped on top of the
            // dialogue box and if so, then we populate the dropped items list in
            // that dialogue branch's array of dropped items and check whether or
            // not all the required items have been dropped in order to progress
            // further in the dialogue
            if (hit.transform.tag == "Dialogue Box")
            {
                NPC currentNpc = DialogueManager.Instance.CurrentNPCDialogue;

                int index = 0;
                for (int i = 0; i < currentNpc.DialogueFormats.Count; i++)
                {
                    if (currentNpc.DialogueFormats[i].Language == SettingsManager.Instance.Language)
                    {
                        index = i;
                        break;
                    }
                }

                foreach (DialogueBranch branch in currentNpc.DialogueFormats[index].Dialogue[currentNpc.CurrentDialogueIndex].DialogueBranches)
                {
                    foreach (string requiredItem in branch.ItemsRequired)
                    {
                        // If the item's name matches one of the required ones, then we
                        // give feedback to the player the item has been dropped.
                        if (Name == requiredItem)
                        {
                            branch.ItemsDropped.Add(Name);

                            //Debug.Log(string.Format("Item {0} is successfully dropped!", Name));

                            if (branch.ItemsDropped.Count == branch.ItemsRequired.Count)
                            {
                                currentNpc.ContinueDialogue();
                            }
                        }
                        else
                        {
                            //Debug.Log(string.Format("Item {0} was not dropped!", Name));
                        }
                    }
                }
            }
        }

        transform.SetParent(GameObject.Find("Items").transform);
        Character.Instance.ReloadInventory();
    }
}
