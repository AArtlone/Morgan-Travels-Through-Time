using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using TMPro;

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
    private List<GameObject> _itemsInFindList = new List<GameObject>();
    private GameObject _itemToHighlight;

    // Object references used for the puzzle canvas.
    public TextMeshProUGUI Timer;
    public GameObject FoundItemsDisplay;
    public GameObject ItemDisplayPrefab;
    public Button HintButton;
    public GameObject PuzzleEndPopup;

    private void Start()
    {
        // We get all the objects in the Objects To Find List in this hidden objects
        // puzzle from the editor, and we use those objects for the hints mechanic.
        for (int i = 0; i < transform.GetChild(0).transform.GetChild(1).transform.childCount; i++)
        {
            _itemsInFindList.Add(transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).gameObject);

            //Debug.Log(transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).gameObject.name);
        }

        Counter = TimeToComplete;
        Timer.text = Counter.ToString();

        ItemsFound.Clear();
    }

    public void StartTimer()
    {
        Avatar.Instance.gameObject.SetActive(false);
        InterfaceManager.Instance.BottomUIInventory.SetActive(false);

        // Here we reset the tapped and hinted at booleans because once the puzzle
        // is over and started again, we need to hint at the right objects instead of.
        // the old ones from the previous play session of that puzzle.
        foreach (GameObject item in _itemsInFindList)
        {
            item.GetComponent<Item>().IsItemHintedAt = false;
            item.GetComponent<Item>().ItemFound = false;
        }

        if (IsInvoking("CountDown"))
        {
            CancelInvoke();
        }

        Counter = TimeToComplete;
        Timer.text = Counter.ToString();

        InvokeRepeating("CountDown", 1f, 1f);

        for (int i = 0; i < FoundItemsDisplay.transform.childCount; i++)
        {
            FoundItemsDisplay.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = string.Empty;
        }
    }

    public void CompletePuzzle()
    {
        // TODO: Change the stars values of this puzzle using different formulas.
        CancelInvoke();

        PuzzleEndPopup.SetActive(true);
        PuzzleEndPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You have successfully completed this puzzle and have received a new item in your inventory!";

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
        PuzzleEndPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You have failed to complete this puzzle.";

        if (IsItemEarned == false)
        {
            Stars = 1;
        }

        transform.GetComponentInParent<Puzzle>().RefreshHiddenObjectsPuzzle();
    }

    public void ClosePuzzle()
    {
        InterfaceManager.Instance.BottomUIInventory.SetActive(true);
        transform.GetComponentInParent<Image>().raycastTarget = true;

        gameObject.SetActive(false);
    }

    public void ClosePopUp(Object obj)
    {
        GameObject objClicked = obj as GameObject;
        objClicked.SetActive(false);
    }

    public void PickItem(Object obj)
    {
        GameObject itemObjClicked = obj as GameObject;
        Item objAsItem = itemObjClicked.GetComponent<Item>();

        if (!ItemsFound.Contains(objAsItem.Name))
        {
            ItemsFound.Add(objAsItem.Name);
            objAsItem.ItemFound = true;
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
            FoundItemsDisplay.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = string.Empty;
        }

        for (int i = 0; i < FoundItemsDisplay.transform.childCount; i++)
        {
            if (ItemsFound[i] != null)
            {
                FoundItemsDisplay.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = ItemsFound[i];
            }
        }
    }

    public void UseHint()
    {
        Character.Instance.RemoveAvailableHints(1);

        Random rng = new Random();

        List<GameObject> objectsForHinting = new List<GameObject>();
        // We check if all the items for tapping are already hinted at, so
        // that we can start hinting at different ones from the start again rather
        // than hinting at objects already hinted at.
        int itemsHinted = 0;
        foreach (GameObject item in _itemsInFindList)
        {
            if (item.GetComponent<Item>().IsItemHintedAt) {
                itemsHinted++;
            }
        }
        //Debug.Log(itemsHinted);

        if (itemsHinted == _itemsInFindList.Count)
        {
            foreach (GameObject item in _itemsInFindList)
            {
                item.GetComponent<Item>().IsItemHintedAt = false;
            }
        }

        foreach (GameObject item in _itemsInFindList)
        {
            Item itemScript = item.GetComponent<Item>();
            if (itemScript.IsItemHintedAt == false && itemScript.ItemFound == false)
            {
                objectsForHinting.Add(item);
            }
        }
        _itemToHighlight = objectsForHinting[Random.Range(0, objectsForHinting.Count - 1)];

        _itemToHighlight.GetComponent<Image>().color = Color.red;
        _itemToHighlight.GetComponent<Item>().IsItemHintedAt = true;
        Invoke("ReturnItemColors", 1f);

        // The player can only use the button after the hint duration has passed.
        HintButton.interactable = false;
    }

    private void ReturnItemColors()
    {
        _itemToHighlight.GetComponent<Image>().color = Color.gray;

        // The player can only use the button after the hint duration has passed.
        HintButton.interactable = true;
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
