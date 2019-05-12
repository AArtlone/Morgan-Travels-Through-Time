using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceDisplay : MonoBehaviour
{
    public GameObject IconPrefab;
    public GameObject DummieIconPrefab;
    public GameObject DeselectIconPrefab;
    [Space(10)]
    public List<Clothing> Bodies;
    public GameObject BodiesDisplay;
    [Space(10)]
    public List<Clothing> Faces;
    public GameObject FacesDisplay;
    [Space(10)]
    public List<Clothing> Hairs;
    public GameObject HairsDisplay;
    [Space(10)]
    public List<Clothing> Tops;
    public GameObject TopsDisplay;
    [Space(10)]
    public List<Clothing> Bots;
    public GameObject BotsDisplay;
    [Space(10)]
    public List<Clothing> Shoes;
    public GameObject ShoesDisplay;

    public List<GameObject> Buttons;
    public List<GameObject> Displays;


    public List<GameObject> BodyIcons;
    public List<GameObject> FaceIcons;
    public List<GameObject> HairIcons;
    public List<GameObject> TopIcons;
    public List<GameObject> BotIcons;
    public List<GameObject> ShoesIcons;

    private GameObject _bodyBodyPart;
    private GameObject _hairBodyPart;
    private GameObject _faceBodyPart;
    private GameObject _topBodyPart;
    private GameObject _botBodyPart;
    private GameObject _shoesBodyPart;
    private Sprite[] _spritesFromStorage;

    public Sprite UnSelectedButton;
    public Sprite SelectedButton;

    public Animator PanelAnimator;
    private const int clothingLength = 24;

    private void Start()
    {
        _spritesFromStorage = Resources.LoadAll<Sprite>("Clothing");

        _bodyBodyPart = GameObject.Find("Body Body Part");
        _hairBodyPart = GameObject.Find("Hair Body Part");
        _faceBodyPart = GameObject.Find("Face Body Part");
        _topBodyPart = GameObject.Find("Top Body Part");
        _botBodyPart = GameObject.Find("Bot Body Part");
        _shoesBodyPart = GameObject.Find("Shoes Body Part");

        SetupDisplays();
        LoadCharacterAppearance();
    }

    // Whenever the player clicks on a button to view a body type's elements
    // this will make it so that its list of buttons will be hidden and viewed
    // whenever he clicks on the body type button.
    public void ToggleDisplay(Object display)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.NewPageInDiary);

        PanelAnimator.SetBool("IsOpen", true);
        GameObject displayObj = (GameObject)display;
        foreach(GameObject _display in Displays)
        {
            if(_display != displayObj)
            {
                _display.SetActive(false);
            }
        }
        if(displayObj.activeSelf)
        {
            displayObj.SetActive(false);
            PanelAnimator.SetBool("IsOpen", false);
        } else
        {
            displayObj.SetActive(true);
        }
    }

    public void ToggleIconFrame(GameObject button)
    {
        switch (button.transform.parent.transform.parent.transform.name)
        {
            case "Bodies Display":
                for (int i = 0; i < BodyIcons.Count; i++)
                {
                    BodyIcons[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                button.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "Faces Display":
                for (int i = 0; i < FaceIcons.Count; i++)
                {
                    FaceIcons[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                button.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "Hairs Display":
                for (int i = 0; i < HairIcons.Count; i++)
                {
                    HairIcons[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                button.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "Tops Display":
                for (int i = 0; i < TopIcons.Count; i++)
                {
                    TopIcons[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                button.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "Bots Display":
                for (int i = 0; i < BotIcons.Count; i++)
                {
                    BotIcons[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                button.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "Shoes Display":
                for (int i = 0; i < ShoesIcons.Count; i++)
                {
                    ShoesIcons[i].transform.GetChild(0).gameObject.SetActive(false);
                }
                button.transform.GetChild(0).gameObject.SetActive(true);
                break;
        }
        
        AudioManager.Instance.PlaySound(AudioManager.Instance.CloseWindow);
    }

    /// <summary>
    ///  This function makes every button for every body type that can be clicked to
    ///  display its items to become highlighted and every other body type button to
    ///  be de-selected for clarity.
    /// </summary>
    /// <param name="button"></param>
    public void ToggleButton(Object button)
    {
        GameObject buttonObj = (GameObject)button;
        foreach(GameObject _button in Buttons)
        {
            if(_button != buttonObj)
            {
                _button.GetComponent<Image>().sprite = UnSelectedButton;
            }
        }
        if(buttonObj.GetComponent<Image>().sprite == SelectedButton)
        {
            buttonObj.GetComponent<Image>().sprite = UnSelectedButton;
        } else
        {
            buttonObj.GetComponent<Image>().sprite = SelectedButton;
        }
        
    }

    /// <summary>
    /// This function creates the icons in the body tab's lists (displays) using the
    /// predefined wearables from the wearables json processed by the player.
    /// </summary>
    private void SetupDisplays()
    {
        // The length starts from the maximum amount the displays for body part
        // clothings can display at max without overflowing it.
        int bodyPartLength = clothingLength;
        int facePartLength = clothingLength;
        int hairPartLength = clothingLength;
        int topPartLength = clothingLength;
        int botPartLength = clothingLength;
        int shoesPartLength = clothingLength;

        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            switch (clothing.BodyPart)
            {
                case "Body":
                    {
                        bodyPartLength--;
                        LoadIcon(clothing, BodiesDisplay, Bodies);
                        break;
                    }
                case "Face":
                    {
                        facePartLength--;
                        LoadIcon(clothing, FacesDisplay, Faces);
                        break;
                    }
                case "Hair":
                    {
                        hairPartLength--;
                        LoadIcon(clothing, HairsDisplay, Hairs);
                        break;
                    }
                case "Top":
                    {
                        topPartLength--;
                        LoadIcon(clothing, TopsDisplay, Tops);
                        break;
                    }
                case "Bot":
                    {
                        botPartLength--;
                        LoadIcon(clothing, BotsDisplay, Bots);
                        break;
                    }
                case "Shoes":
                    {
                        shoesPartLength--;
                        LoadIcon(clothing, ShoesDisplay, Shoes);
                        break;
                    }
            }
        }

        shoesPartLength--;
        hairPartLength--;
        GenerateDeselectIcon(ShoesDisplay);
        GenerateDeselectIcon(HairsDisplay);

        // After all the items from the player's inventory have been displayed
        // for selection, we create dummy icons that show more items can be
        // acquired throughout the game.
        GenerateDummieIcons(bodyPartLength, BodiesDisplay, "bodyIcon001");
        GenerateDummieIcons(facePartLength, FacesDisplay, "faceIcon001");
        GenerateDummieIcons(hairPartLength, HairsDisplay, "hairIcon001");
        GenerateDummieIcons(topPartLength, TopsDisplay, "topIcon001");
        GenerateDummieIcons(botPartLength, BotsDisplay, "botIcon001");
        GenerateDummieIcons(shoesPartLength, ShoesDisplay, "shoesIcon001");
    }

    /// <summary>
    /// Generates a number of dummies based on a specific body part inside the
    /// display it must fill after all the real clothes were generated.
    /// </summary>
    /// <param name="numberOfDummies"></param>
    /// <param name="display"></param>
    /// <param name="bodyPart"></param>
    private void GenerateDummieIcons(int numberOfDummies, GameObject display, string bodyPart)
    {
        for (int i = 0; i < numberOfDummies; i++)
        {
            GameObject newIcon = Instantiate(DummieIconPrefab, display.transform.GetChild(0).transform);

            Sprite sprite = Resources.Load<Sprite>("Icons/" + bodyPart);
            Image imageComponent = newIcon.GetComponent<Image>();
            imageComponent.sprite = sprite;
            imageComponent.color = new Color(0, 0, 0, 0.5f);
        }
    }

    // Generates the Deselect body part icon
    private void GenerateDeselectIcon(GameObject display)
    {
        GameObject deselectIcon = Instantiate(DeselectIconPrefab, display.transform.GetChild(0).transform);
        if(display.name == "Shoes Display")
        {
            ShoesIcons.Add(deselectIcon);
        } else if (display.name == "Hairs Display")
        {
            HairIcons.Add(deselectIcon);
        }
    }

    /// <summary>
    /// This retrieves the icons from the storage and assigns them to buttons in the
    /// appearance selection tabs on the right.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="display"></param>
    /// <param name="listOfItems"></param>
    private void LoadIcon(Clothing item, GameObject display, List<Clothing> listOfItems)
    {
        if (display != null)
        {
            listOfItems.Add(item);
            GameObject newIcon = Instantiate(IconPrefab, display.transform.GetChild(0).transform);
            newIcon.GetComponent<AppearanceSelector>().PortraitImage = item.PortraitImage;
            newIcon.GetComponent<AppearanceSelector>().BodyPart = item.BodyPart;
            if(item.BodyPart == "Body")
            {
                BodyIcons.Add(newIcon);
            } else if(item.BodyPart == "Face")
            {
                FaceIcons.Add(newIcon);
            } else if(item.BodyPart == "Hair")
            {
                HairIcons.Add(newIcon);
            } else if(item.BodyPart == "Top")
            {
                TopIcons.Add(newIcon);
            } else if(item.BodyPart == "Bot")
            {
                BotIcons.Add(newIcon);
            } else if(item.BodyPart == "Shoes")
            {
                ShoesIcons.Add(newIcon);
            }

            //Sprite selectedFrameSprite = Resources.Load<Sprite>("Icons/" + "selectedFrame");
            GameObject selectedFrame = newIcon.transform.GetChild(0).gameObject;
            //selectedFrame.GetComponent<Image>().sprite = selectedFrameSprite;
            selectedFrame.SetActive(false);
            Sprite iconSprite = Resources.Load<Sprite>("Icons/" + item.Icon);
            GameObject clothingIcon = newIcon.transform.GetChild(1).gameObject;
            clothingIcon.GetComponent<Image>().sprite = iconSprite;

            if (item.Selected == true)
            {
                selectedFrame.SetActive(true);
            }
        }
    }

    /// <summary>
    /// This imports the appearance elements according to the body parts and selected
    /// appearance elements from the player and it will load them into the player's
    /// character view.
    /// </summary>
    public void LoadCharacterAppearance()
    {
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.Selected)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.PortraitImage)
                    {
                        // More to add...
                        switch (clothing.BodyPart)
                        {
                            case "Body":
                                 _bodyBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Hair":
                                  _hairBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Face":
                                _faceBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Top":
                                _topBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Bot":
                                _botBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Shoes":
                                _shoesBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                        }
                    }
                }
            }
        }
    }
}
