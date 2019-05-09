using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using LitJson;
using System.IO;

public class HiddenObjectsPuzzle : MonoBehaviour
{
    public Puzzle puzzleNPC;
    public string PuzzleName;
    public string PuzzleNameDutch;
    public int Stars;
    public int TimeToComplete;
    public bool Completed;
    public Image PuzzleBackground;
    // Those two lists are used for comparison mid-puzzle and are responsible
    // for deciding whether or not a puzzle is complete succcessfully.
    public List<string> ItemsRequired = new List<string>();
    public List<string> ItemsRequiredDutch = new List<string>();
    public List<string> ItemsFound = new List<string>();
    public List<GameObject> EntitiesToActivate = new List<GameObject>();
    public List<GameObject> EntitiesToDeactivate = new List<GameObject>();
    [Space(15)]
    [Tooltip("Item reward's name")]
    [Header("Item Earned once the puzzle is done is defined here!")]
    public string ItemRewardName;
    public string ItemRewardNameDutch;
    [Space(10)]
    [Tooltip("Item reward's description")]
    public string ItemRewardDescription;
    public string ItemRewardDescriptionDutch;
    [Space(10)]
    [Tooltip("Item reward's active/s description")]
    public string ItemRewardActive;
    public string ItemRewardActiveDutch;
    [Space(10)]
    [Tooltip("Item reward's assets image name (without file type)")]
    public string ItemRewardAssetsImageName;
    [Space(15)]
    public string QuestForObjective;
    public int ObjectiveToCompleteID;
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
    public List<GameObject> StarsList;

    private CameraBehavior _cameraBehaviour;
    private DHManager _DHManager;

    public HiddenObjectsPuzzle(string name, string nameDutch, int stars, bool completed)
    {
        PuzzleName = name;
        PuzzleNameDutch = nameDutch;
        Stars = stars;
        Completed = completed;
    }

    private void Start()
    {
        _cameraBehaviour = FindObjectOfType<CameraBehavior>();
        _DHManager = FindObjectOfType<DHManager>();
        // List does not end up being created at the top of the class, so i
        // make it again here instead.
        _itemsInFindList = new List<GameObject>();

        // We get all the objects in the Objects To Find List in this hidden objects
        // puzzle from the editor, and we use those objects for the hints mechanic.
        for (int i = 0; i < transform.GetChild(0).transform.GetChild(1).transform.childCount; i++)
        {
            //Debug.Log(transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).gameObject.name);

            if (transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).gameObject != null)
            {
                _itemsInFindList.Add(transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).gameObject);
            }
        }

        Counter = TimeToComplete;
        Timer.text = Counter.ToString();

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

        if (Stars == 0)
        {
            SetupPuzzleData();
        } else if (Stars > 0)
        {
            LoadPuzzleData();
        }
        puzzleNPC.LoadStars();
    }

    private void SetupPuzzleData()
    {
        TextAsset puzzlesData = Resources.Load<TextAsset>("Default World Data/Puzzles");
        JsonData puzzlesJsonData = JsonMapper.ToObject(puzzlesData.text);

        for (int i = 0; i < puzzlesJsonData["Puzzles"].Count; i++)
        {
            if (puzzlesJsonData["Puzzles"][i]["Name"].ToString() == PuzzleName ||
                puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString() == PuzzleNameDutch)
            {
                Stars = int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString());
                Completed = (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false);
            }
        }
    }

    private void LoadPuzzleData()
    {
        JsonData puzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(Application.persistentDataPath + "/Puzzles.json"));

        for (int i = 0; i < puzzlesJsonData["Puzzles"].Count; i++)
        {
            if (puzzlesJsonData["Puzzles"][i]["Name"].ToString() == PuzzleName ||
                puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString() == PuzzleNameDutch)
            {
                Stars = int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString());
                Completed = (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false);
            }
        }
    }

    private void RefreshPuzzleData()
    {
        JsonData puzzlesJsonData = JsonMapper.ToObject(File.ReadAllText(Application.persistentDataPath + "/Puzzles.json").ToString());

        List<HiddenObjectsPuzzle> HOPs = new List<HiddenObjectsPuzzle>();

        for (int i = 0; i < puzzlesJsonData["Puzzles"].Count; i++)
        {
            HOPs.Add(
                new HiddenObjectsPuzzle(
                    puzzlesJsonData["Puzzles"][i]["Name"].ToString(),
                    puzzlesJsonData["Puzzles"][i]["NameDutch"].ToString(),
                    int.Parse(puzzlesJsonData["Puzzles"][i]["Stars"].ToString()),
                    (puzzlesJsonData["Puzzles"][i]["Completed"].ToString() == "True" ? true : false)));
        }

        foreach (HiddenObjectsPuzzle HOP in HOPs)
        {
            if (HOP.PuzzleName == PuzzleName ||
                HOP.PuzzleNameDutch == PuzzleNameDutch)
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
            newJsonData += "\"Name\": \"" + HOP.PuzzleName + "\",";
            newJsonData += InsertNewLineTabs(3);
            newJsonData += "\"NameDutch\": \"" + HOP.PuzzleNameDutch + "\",";
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
        File.WriteAllText(Application.persistentDataPath + "/Puzzles.json", newJsonData);
    }

    public void StartTimer()
    {
        //InterfaceManager.Instance.BottomUIInventory.SetActive(false);

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

        InvokeRepeating("CountDown", 1f, 1f);

        //for (int i = 0; i < FoundItemsDisplay.transform.childCount; i++)
        //{
        //    FoundItemsDisplay.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
        //}
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.SoundPuzzleCompleted);

        // TODO: Change the stars values of this puzzle using different formulas.
        CancelInvoke();

        PuzzleEndPopup.SetActive(true);
        //_cameraBehaviour.IsInteracting = false;

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

        foreach (GameObject objToActivate in EntitiesToActivate)
        {
            objToActivate.SetActive(true);
        }

        foreach (GameObject objToDeactivate in EntitiesToDeactivate)
        {
            objToDeactivate.SetActive(false);
        }

        string dataToJson = File.ReadAllText(Application.persistentDataPath + "/Items.json");
        string dataToJsonDutch = File.ReadAllText(Application.persistentDataPath + "/ItemsDutch.json");
        JsonData data = JsonMapper.ToObject(dataToJson);
        JsonData dataDutch = JsonMapper.ToObject(dataToJsonDutch);

        for (int j = 0; j < data["Items"].Count; j++)
        {
            //Debug.Log("Have " + (data["Items"][j]["Name"].ToString()));
            if (data["Items"][j]["Name"].ToString() == ItemRewardName)
            {
                IsItemEarned = true;
            }
        }

        for (int j = 0; j < dataDutch["Items"].Count; j++)
        {
            //Debug.Log("Have " + (data["Items"][j]["Name"].ToString()));
            if (dataDutch["Items"][j]["Name"].ToString() == ItemRewardNameDutch)
            {
                IsItemEarned = true;
            }
        }

        if (IsItemEarned == false)
        {
            if (ItemRewardName != string.Empty)
            {
                // This does not work now since we have the scene
                // reload as band-aid fix for resetting the HOP.
                //if (ItemRewardName == "Diary")
                //{
                //    if (_DHManager != null)
                //    {
                //        //_DHManager.LoadSequence("Teach Main Interface");
                //    }
                //}

                Character.Instance.AddItem(
                new Item(
                    ItemRewardName,
                    ItemRewardNameDutch,
                    ItemRewardDescription,
                    ItemRewardDescriptionDutch,
                    ItemRewardActive,
                    ItemRewardActiveDutch,
                    ItemRewardAssetsImageName));
                IsItemEarned = true;
            }
        }

        if (Character.Instance.AreIconsExplained == false && Character.Instance.HasDiary == false)
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
        _cameraBehaviour.IsInteracting = false;

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

        if (IsItemEarned == false)
        {
            Stars = 1;
        }

        StartCoroutine(ShowStars());
        RefreshPuzzleData();
    }

    public void ClosePuzzle()
    {
        InterfaceManager.Instance.BottomUIInventory.SetActive(true);
        //transform.GetComponentInParent<Image>().raycastTarget = true;

        Character.Instance.InitiateInteraction();

        puzzleNPC.FinishPuzzleIconToggle();

        if (SceneManager.GetActiveScene().name != "Test Area")
        {
            SceneManager.LoadScene("Tutorial Map Area");
        }

        gameObject.transform.parent.gameObject.SetActive(false);
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

        string NameOfItem = string.Empty;
        switch (SettingsManager.Instance.Language)
        {
            case "English":
                NameOfItem = objAsItem.GetComponent<LanguageController>().English;
                break;
            case "Dutch":
                NameOfItem = objAsItem.GetComponent<LanguageController>().Dutch;
                break;
        }

        if (!ItemsFound.Contains(NameOfItem))
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.ItemFoundUsed);

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
                switch (SettingsManager.Instance.Language)
                {
                    case "English":
                        if (ItemsRequired.Contains(foundItem))
                        {
                            foundItemsCount++;
                            //Debug.Log(foundItemsCount);
                        }
                        break;
                    case "Dutch":
                        if (ItemsRequiredDutch.Contains(foundItem))
                        {
                            foundItemsCount++;
                            //Debug.Log(foundItemsCount);
                        }
                        break;
                }
            }

            StartCoroutine(FadeAwayItem(itemObjClicked));

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

    /// <summary>
    /// Reload the list of items in the botton UI and if any of the item names
    /// that are loaded match ones from the found items list, then turn those
    /// green and the ones that dont match to white.
    /// </summary>
    public void UpdateFoundItemsDisplay()
    {
        for (int i = 0; i < FoundItemsDisplay.transform.childCount; i++)
        {
            if (i < ItemsRequired.Count)
            {
                string nameOfItem = string.Empty;
                string language = string.Empty;
                switch (SettingsManager.Instance.Language)
                {
                    case "English":
                        nameOfItem = ItemsRequired[i];

                        break;
                    case "Dutch":
                        nameOfItem = ItemsRequiredDutch[i];

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
                            FoundItemsDisplay.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = ItemsRequired[i];
                            break;
                        case "Dutch":
                            FoundItemsDisplay.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = ItemsRequiredDutch[i];
                            break;
                    }
                }
            }
        }
    }

    public void UseHint()
    {
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
        _itemToHighlight.GetComponent<Image>().color = Color.white;

        // The player can only use the button after the hint duration has passed.
        HintButton.interactable = true;
    }

    private IEnumerator FadeAwayItem(GameObject obj)
    {
        Image imageComponent = obj.GetComponent<Image>();
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

    private IEnumerator ShowStars()
    {
        for (int i = 0; i < StarsList.Count; i++)
        {
            yield return new WaitForSeconds(0.5f);
            StarsList[i].SetActive(true);
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
