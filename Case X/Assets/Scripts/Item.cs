using UnityEngine;
using System;
using UnityEngine.UI;

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

    public void DragItem()
    {
        Touch touch = Input.GetTouch(0);
        transform.parent = GameObject.Find("Canvas").transform;
        transform.position = new Vector2(touch.position.x, touch.position.y);
    }

    public void DropItem()
    {
        Ray ray = new Ray(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // We use the canvas tag for map interactions instead of the dialogue.
            if (hit.transform.tag == "Canvas")
            {
                Debug.Log("Dropped on top of canvas");
            }

            // Here we check if the item was dragged and dropped on top of the
            // dialogue box and if so, then we populate the dropped items list in
            // that dialogue branch's array of dropped items and check whether or
            // not all the required items have been dropped in order to progress
            // further in the dialogue
            if (hit.transform.tag == "Dialogue Box")
            {
                NPC currentNpc = DialogueManager.Instance.CurrentNPCDialogue;
                foreach (DialogueBranch branch in currentNpc.Dialogue[currentNpc.CurrentDialogueIndex].DialogueBranches)
                {
                    foreach (string requiredItem in branch.ItemsRequired)
                    {
                        // If the item's name matches one of the required ones, then we
                        // give feedback to the player the item has been dropped.
                        if (Name == requiredItem)
                        {
                            branch.ItemsDropped.Add(Name);

                            Debug.Log(string.Format("Item {0} is successfully dropped!", Name));

                            if (branch.ItemsDropped.Count == branch.ItemsRequired.Count)
                            {
                                currentNpc.ContinueDialogue();
                            }
                        }
                        else
                        {
                            Debug.Log(string.Format("Item {0} was not dropped!", Name));
                        }
                    }
                }
            }
        }

        transform.parent = GameObject.Find("Items").transform;
        Character.Instance.ReloadInventory();
    }
}
