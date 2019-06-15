using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HiddenObjectsPuzzle : MonoBehaviour
{
    public string Name;
    [NonSerialized]
    public string NameDutch;
    public int Stars;
    public int TimeToComplete;
    [NonSerialized]
    public bool Completed;
    public string SceneName;
    [Space(15)]
    public GameObject BottomUI;
    public GameObject InstructionManual;

    #region Items references
    // Those two lists are used for comparison mid-puzzle and are responsible
    // for deciding whether or not a puzzle is complete succcessfully.
    public List<Item> _itemsRequired = new List<Item>();
    [Space(15)]
    public List<string> ItemsFound = new List<string>();
    public List<Item> ItemsToBeRewarded = new List<Item>();

    // We use this to make it so that if the player completed the puzzle
    // more than once, he will not receive the same reward twice.
    private bool _isItemEarned;
    // This is used for the hint mechanic to later restore its original state after being hinted.
    private GameObject _itemToHighlight;
    #endregion

    #region Quest references
    [Space(15)]
    // The following objective in the quest set in the editor, will be completed after
    // this puzzle's successful pass.
    public string QuestForObjective;
    public int ObjectiveToCompleteID;
    #endregion

    #region Timer references
    [Space(15)]
    public TextMeshProUGUI Timer;
    [NonSerialized]
    public int Counter;
    #endregion

    #region Hint mechanic references
    // This list is used to know which items have been hinted at during the game.
    private List<GameObject> _itemsInFindList = new List<GameObject>();
    public Button HintButton;
    #endregion

    public GameObject ItemsToFindList;
    public GameObject FoundItemsDisplay;
    public GameObject ItemDisplayPrefab;
    public GameObject PuzzleEndPopup;
    public List<GameObject> StarsList;
    [NonSerialized]
    public bool _isInterfaceToggledOn;
    [NonSerialized]
    public bool _isEndPopupOn;
    private int _missclicks;

    public HiddenObjectsPuzzle(string name, string nameDutch, string sceneName, int stars, bool completed)
    {
        Name = name;
        NameDutch = nameDutch;
        SceneName = sceneName;
        Stars = stars;
        Completed = completed;
    }

    private void Start()
    {
        _isInterfaceToggledOn = true;

        // List does not end up being created at the top of the class,
        // so I make it here instead.
        _itemsInFindList = new List<GameObject>();
        _itemsRequired = new List<Item>();

        // We get all the objects in the Objects To Find List in this hidden objects
        // puzzle from the editor, and we use those objects for the hints mechanic.
        for (int i = 0; i < ItemsToFindList.transform.childCount; i++)
        {
            if (ItemsToFindList.transform.GetChild(i).gameObject != null)
            {
                _itemsInFindList.Add(ItemsToFindList.transform.GetChild(i).gameObject);
            }
        }

        // Fills in the list of items required to be found to finish the puzzle.
        foreach (Item item in GameObject.Find("Objects To Find List").GetComponentsInChildren<Item>())
        {
            _itemsRequired.Add(item);
        }

        /*// After that the timer of the puzzle and the hints parameter is initiated.
        Counter = TimeToComplete;
        Timer.text = Counter.ToString();*/

        TextMeshProUGUI hintButton = HintButton.GetComponentInChildren<TextMeshProUGUI>();
        hintButton.text = Character.Instance.AvailableHints + Environment.NewLine;
        
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                hintButton.text += (Character.Instance.AvailableHints > 1 ? "hints" : "hint");
                break;
            case "Dutch":
                hintButton.text += (Character.Instance.AvailableHints > 1 ? "hints" : "hint");
                break;
        }

        SetupPuzzleData();

        if (Character.Instance.TutorialCompleted == false)
        {
            InstructionManual.SetActive(true);
        }

        UpdateFoundItemsDisplay();
    }

    private void SetupPuzzleData()
    {
        JsonData puzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(Application.persistentDataPath + "/HiddenObjectPuzzles.json"));

        for (int i = 0; i < puzzlesJsonData["Puzzles"].Count; i++)
        {
            if (puzzlesJsonData["Puzzles"][i]["Name"].ToString() == Name ||
                puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString() == NameDutch)
            {
                Stars = int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString());
                SceneName = puzzlesJsonData["Puzzles"][i]["SceneName"].ToString();
                Completed = (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false);
            }
        }
    }

    private void RefreshPuzzleData()
    {
        JsonData puzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(Application.persistentDataPath + "/HiddenObjectPuzzles.json").ToString());

        List<HiddenObjectsPuzzle> HOPs = new List<HiddenObjectsPuzzle>();

        for (int i = 0; i < puzzlesJsonData["Puzzles"].Count; i++)
        {
            HOPs.Add(
                new HiddenObjectsPuzzle(
                    puzzlesJsonData["Puzzles"][i]["Name"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["SceneName"].ToString(),
                    int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString()),
                    (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false)));
        }

        foreach (HiddenObjectsPuzzle HOP in HOPs)
        {
            if (HOP.Name == Name ||
                HOP.NameDutch == NameDutch)
            {
                HOP.Stars = Stars;
                HOP.Completed = Completed;
            }
        }

        string newJsonData = "{";
        newJsonData += InsertNewLineTabs(1);
        newJsonData += "\"Puzzles\": [";

        foreach (HiddenObjectsPuzzle HOP in HOPs)
        {
            newJsonData += InsertNewLineTabs(2);
            newJsonData += "{";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Name\": \"" + HOP.Name + "\",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"NameDutch\": \"" + HOP.NameDutch + "\",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"SceneName\": \"" + HOP.SceneName + "\",";
            newJsonData += InsertNewLineTabs(3);
            if (HOP.Completed == true)
            {
                newJsonData += "\"Completed\": true" + ",";
            }
            else if (HOP.Completed == false)
            {
                newJsonData += "\"Completed\": false" + ",";
            }
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"Stars\": " + HOP.Stars;
            newJsonData += InsertNewLineTabs(2);
            newJsonData += "},";
        }
        newJsonData = newJsonData.Substring(0, newJsonData.Length - 1);
        newJsonData += InsertNewLineTabs(1) + "]" + Environment.NewLine + "}";
        File.WriteAllText(Application.persistentDataPath + "/HiddenObjectPuzzles.json", newJsonData);
    }

    public void StartTimer()
    {
        // Here we reset the tapped and hinted at booleans because once the puzzle
        // is over and started again, we need to hint at the right objects instead of.
        // the old ones from the previous play session of that puzzle.
        foreach (GameObject item in _itemsInFindList)
        {
            item.GetComponent<Item>().IsItemHintedAt = false;
            item.GetComponent<Item>().ItemFound = false;
        }

        /*if (IsInvoking("CountDown"))
        {
            CancelInvoke();
        }

        InvokeRepeating("CountDown", 1f, 1f);*/
    }
    public void ResetPuzzle()
    {
        for (int i = 0; i < FoundItemsDisplay.transform.childCount; i++)
        {
            FoundItemsDisplay.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }

        for (int i = 0; i < transform.GetChild(0).transform.GetChild(1).transform.childCount; i++)
        {
            transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponentInChildren<Image>().color = new Color(255, 255, 255, 255);
        }
    }

    public void CompletePuzzle()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.SoundPuzzleCompleted);
        }

        // TODO: Change the stars values of this puzzle using different formulas.
        CancelInvoke();

        PuzzleEndPopup.SetActive(true);
        _isEndPopupOn = true;

        switch (SettingsManager.Instance.Language)
        {
            case "English":
                PuzzleEndPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You have successfully completed this puzzle!";
                break;
            case "Dutch":
                PuzzleEndPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Je bent er in geslaagd de puzzel te voltooien!";
                break;
        }

        Character.Instance.CompleteObjectiveInQuest(ObjectiveToCompleteID, QuestForObjective);

        // The following code block checks if the items to be rewarded to the player for
        // completing this puzzle are already in his inventory, because if they are, we dont
        // want them to receive another copy of it.
        string dataToJson = File.ReadAllText(Application.persistentDataPath + "/Items.json");
        string dataToJsonDutch = File.ReadAllText(Application.persistentDataPath + "/ItemsDutch.json");
        JsonData data = JsonMapper.ToObject(dataToJson);
        JsonData dataDutch = JsonMapper.ToObject(dataToJsonDutch);

        foreach (Item item in ItemsToBeRewarded)
        {
            for (int i = 0; i < data["Items"].Count; i++)
            {
                if (data["Items"][i]["Name"].ToString() == item.Name ||
                    dataDutch["Items"][i]["Name"].ToString() == item.NameDutch)
                {
                    _isItemEarned = true;
                }
            }

            if (_isItemEarned == false &&
                item.Name != string.Empty)
            {
                    Character.Instance.AddItem(
                    new Item(
                        item.Name,
                        item.NameDutch,
                        item.Description,
                        item.DescriptionDutch,
                        item.Active,
                        item.ActiveDutch,
                        item.AssetsImageName));
                    _isItemEarned = true;
            }
        }

        if (Character.Instance.AreIconsExplained == false &&
            Character.Instance.HasDiary == false)
        {
            Character.Instance.AreIconsExplained = true;
        }
        Character.Instance.HasDiary = true;
        Character.Instance.RefreshJsonData();

        Completed = true;
        Stars = 3;

        StartCoroutine(ShowStars());
        RefreshPuzzleData();
    }

    public void LosePuzzle()
    {
        CancelInvoke();

        PuzzleEndPopup.SetActive(true);

        switch (SettingsManager.Instance.Language)
        {
            case "English":
                PuzzleEndPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "You have failed to complete this puzzle.";
                break;
            case "Dutch":
                PuzzleEndPopup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Het is je niet gelukt om de puzzel te klaren!";
                break;
        }

        if (_isItemEarned == false)
        {
            Stars = 1;
        }

        StartCoroutine(ShowStars());
        RefreshPuzzleData();
    }

    public void ClosePuzzle()
    {
        InterfaceManager.Instance.BottomUIInventory.SetActive(true);

        if (SceneManager.GetActiveScene().name != "Test Area")
        {
            InterfaceManager.Instance.LoadScene("Tutorial Map Area");
        }

        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void ToggleBottomInterface()
    {
        _isInterfaceToggledOn = !_isInterfaceToggledOn;
        RectTransform rectTransform = BottomUI.GetComponent<RectTransform>();

        if (_isInterfaceToggledOn == false)
        {
            rectTransform.anchoredPosition = new Vector2(0, -500);
        } else
        {
            rectTransform.anchoredPosition = new Vector2(0, -258);
        }
    }

    public void PickItem(GameObject obj)
    {
        Item objAsItem = obj.GetComponent<Item>();

        string NameOfItem = string.Empty;
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                NameOfItem = objAsItem.GetComponent<Item>().Name;
                break;
            case "Dutch":
                NameOfItem = objAsItem.GetComponent<Item>().NameDutch;
                break;
        }

        if (!ItemsFound.Contains(NameOfItem))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.ItemFoundUsed);
            }

            switch (SettingsManager.Instance.Language)
            {
                case "English":
                    ItemsFound.Add(objAsItem.GetComponent<LanguageController>().English);
                    break;
                case "Dutch":
                    ItemsFound.Add(objAsItem.GetComponent<LanguageController>().Dutch);
                    break;
            }

            objAsItem.ItemFound = true;
            int foundItemsCount = 0;
            foreach (string foundItem in ItemsFound)
            {
                foreach (Item item in _itemsRequired)
                {
                    if (item.Name == foundItem ||
                        item.NameDutch == foundItem)
                    {
                        foundItemsCount++;
                    }
                }
            }

            StartCoroutine(FadeAwayItem(obj));

            if (foundItemsCount == _itemsRequired.Count)
            {
                CompletePuzzle();
            }

            UpdateFoundItemsDisplay();
        }
    }

    /// <summary>
    /// Reload the list of items in the botton UI and if any of the item names
    /// that are loaded match ones from the found items list, then turn those
    /// green and the ones that dont match to white.
    /// </summary>
    public void UpdateFoundItemsDisplay()
    {
        for (int i = 0; i < FoundItemsDisplay.transform.childCount; i++)
        {
            if (i < _itemsRequired.Count)
            {
                string nameOfItem = string.Empty;
                string language = string.Empty;

                switch (SettingsManager.Instance.Language)
                {
                    case "English":
                        nameOfItem = _itemsRequired[i].Name;

                        break;
                    case "Dutch":
                        nameOfItem = _itemsRequired[i].NameDutch;

                        break;
                }
                // Checking if the item we have in the required items list will
                // have a match in the items found list and if so make it green
                // instead of white text.
                if (ItemsFound.Count > 0)
                {
                    foreach (string item in ItemsFound)
                    {
                        if (item == nameOfItem)
                        {
                            TextMeshProUGUI itemLabel = FoundItemsDisplay.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
                            itemLabel.text = nameOfItem;
                            StartCoroutine(FadeInGreenColor(itemLabel));
                        }
                        else
                        {
                            FoundItemsDisplay.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = nameOfItem;
                        }
                    }
                }
                else
                {
                    switch (SettingsManager.Instance.Language)
                    {
                        case "English":
                            FoundItemsDisplay.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = _itemsRequired[i].Name;
                            break;
                        case "Dutch":
                            FoundItemsDisplay.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = _itemsRequired[i].NameDutch;
                            break;
                    }
                }
            }
        }
    }

    public void UseHint()
    {
        ToggleBottomInterface();
        Invoke("ToggleBottomInterface", 2f);
        Character.Instance.RemoveAvailableHints(1);

        TextMeshProUGUI hintButton = HintButton.GetComponentInChildren<TextMeshProUGUI>();
        hintButton.text = Character.Instance.AvailableHints + Environment.NewLine;

        switch (SettingsManager.Instance.Language)
        {
            case "English":
                hintButton.text += (Character.Instance.AvailableHints > 1 ? "hints" : "hint");
                break;
            case "Dutch":
                hintButton.text += (Character.Instance.AvailableHints > 1 ? "hints" : "hint");
                break;
        }

        Random rng = new Random();

        List<GameObject> objectsForHinting = new List<GameObject>();
        // We check if all the items for tapping are already hinted at, so
        // that we can start hinting at different ones from the start again rather
        // than hinting at objects already hinted at.
        int itemsHinted = 0;
        foreach (GameObject item in _itemsInFindList)
        {
            if (item != null)
            {
                if (item.GetComponent<Item>().IsItemHintedAt)
                {
                    itemsHinted++;
                }
            }
        }

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

        if (_itemToHighlight != null)
        {
            _itemToHighlight.GetComponent<SpriteRenderer>().color = Color.red;
            _itemToHighlight.GetComponent<Item>().IsItemHintedAt = true;
        }
        Invoke("ReturnItemColors", 2f);

        // The player can only use the button after the hint duration has passed.
        HintButton.interactable = false;
    }

    private void ReturnItemColors()
    {
        _itemToHighlight.GetComponent<SpriteRenderer>().color = Color.white;

        // The player can only use the button after the hint duration has passed.
        HintButton.interactable = true;
    }

    private IEnumerator FadeAwayItem(GameObject obj)
    {
        SpriteRenderer imageComponent = obj.GetComponent<SpriteRenderer>();
        Color newColor = imageComponent.color;
        for (int i = 0; i < 100; i++)
        {
            newColor.a -= 0.1f;
            imageComponent.color = newColor;
            // The lower the parameter, the faster the animation.
            yield return new WaitForSeconds(0.02f);
        }

        Destroy(obj);
    }

    private IEnumerator FadeInGreenColor(TextMeshProUGUI text)
    {
        Color newColor = text.color;
        for (int i = 0; i < 255; i++)
        {
            newColor.r -= 0.05f;
            newColor.g += 0.05f;
            newColor.b -= 0.05f;
            text.color = newColor;
            // The lower the parameter, the faster the animation.
            yield return new WaitForSeconds(0.02f);
        }
    }

    /*public void CountDown()
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
    }*/

    public void MissedTap()
    {
        _missclicks += 1;
    }

    private IEnumerator ShowStars()
    {
        for (int i = 0; i < StarsList.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            StarsList[i].SetActive(true);
            Debug.Log("aaaa");
        }
    }
    private string InsertNewLineTabs(int numberOfTabs)
    {
        string whiteSpace = Environment.NewLine;
        for (int i = 0; i < numberOfTabs; i++)
        {
            whiteSpace += "\t";
        }

        return whiteSpace;
    }
}
