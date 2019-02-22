using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class HiddenObjectsPuzzle : MonoBehaviour
{
    public string PuzzleName;
    public int Stars;
    public int TimeToComplete;
    public Image PuzzleBackground;
    // Those two lists are used for comparison mid-puzzle and are responsible
    // for deciding whether or not a puzzle is complete succcessfully.
    public List<string> ItemsRequired = new List<string>();
    public List<string> ItemsFound = new List<string>();
    [Space(15)]
    [Tooltip("Item reward's name")]
    public string ItemRewardName;
    [Tooltip("Item reward's description")]
    public string ItemRewardDescription;
    [Tooltip("Item reward's active/s description")]
    public string ItemRewardActive;
    [Tooltip("Item reward's assets image name (without file type)")]
    public string ItemRewardAssetsImageName;
    [NonSerialized]
    public int Counter;
    private int _missclicks;
    // We use this to make it so that if the player completed the puzzle
    // more than once, he will not receive the same reward twice.
    public bool IsItemEarned;

    // Object references used for the puzzle canvas.
    public Text Timer;
    public GameObject FoundItemsDisplay;
    public GameObject ItemDisplayPrefab;
    public Button HintButton;
    public GameObject PuzzleEndPopup;

    private void Start()
    {
        Counter = TimeToComplete;
        Timer.text = Counter.ToString();

        ItemsFound.Clear();
    }

    public void StartTimer()
    {
        if (IsInvoking("CountDown"))
        {
            CancelInvoke();
        }

        Counter = TimeToComplete;
        Timer.text = Counter.ToString();

        InvokeRepeating("CountDown", 1f, 1f);
    }

    public void CompletePuzzle()
    {
        // TODO: Change the stars values of this puzzle using different formulas.
        CancelInvoke();

        PuzzleEndPopup.SetActive(true);
        PuzzleEndPopup.transform.GetChild(2).GetComponent<Text>().text = "You have successfully completed this puzzle and have received a new item in your inventory!";

        if (IsItemEarned == false)
        {
            Character.Instance.AddItem(
                new Item(
                    ItemRewardName,
                    ItemRewardDescription,
                    ItemRewardActive,
                    ItemRewardAssetsImageName));
            IsItemEarned = true;
        }

        Stars = 3;

        transform.GetComponentInParent<Puzzle>().RefreshHiddenObjectsPuzzle();
    }

    public void LosePuzzle()
    {
        CancelInvoke();

        PuzzleEndPopup.SetActive(true);
        PuzzleEndPopup.transform.GetChild(2).GetComponent<Text>().text = "You have failed to complete this puzzle.";

        if (IsItemEarned == false)
        {
            Stars = 1;
        }

        transform.GetComponentInParent<Puzzle>().RefreshHiddenObjectsPuzzle();
    }

    public void ClosePuzzle()
    {
        transform.GetComponentInParent<Image>().raycastTarget = true;
        gameObject.SetActive(false);
    }

    public void PickItem(UnityEngine.Object obj)
    {
        GameObject itemObjClicked = obj as GameObject;
        Item objAsItem = itemObjClicked.GetComponent<Item>();

        if (!ItemsFound.Contains(objAsItem.Name))
        {
            ItemsFound.Add(objAsItem.Name);
            int foundItemsCount = 0;
            foreach (string foundItem in ItemsFound)
            {
                if (ItemsRequired.Contains(foundItem))
                {
                    foundItemsCount++;
                    //Debug.Log(foundItemsCount);
                }
            }

            if (foundItemsCount == ItemsRequired.Count)
            {
                CompletePuzzle();
            }

            UpdateFoundItemsDisplay();
        }

        /*
        Debug.Log("");
        foreach(string item in _itemsFound)
        {
            Debug.Log(item);
        }
        */
    }

    public void UpdateFoundItemsDisplay()
    {
        for (int i = 0; i < FoundItemsDisplay.transform.childCount; i++)
        {
            FoundItemsDisplay.transform.GetChild(i).GetComponent<Text>().text = string.Empty;
        }

        for (int i = 0; i < FoundItemsDisplay.transform.childCount; i++)
        {
            if (ItemsFound[i] != null)
            {
                FoundItemsDisplay.transform.GetChild(i).GetComponent<Text>().text = ItemsFound[i];
            }
        }
    }

    public void UseHint()
    {
        Character.Instance.RemoveAvailableHints(1);
    }

    public void CountDown()
    {
        if (Counter <= 0)
        {
            LosePuzzle();
        }
        else
        {
            Counter--;
            Timer.text = Counter.ToString();
        }
    }

    public void MissedTap()
    {
        _missclicks += 1;

        //Debug.Log(_missclicks);
    }
}
