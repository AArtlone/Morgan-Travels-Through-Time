using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceDisplay : MonoBehaviour
{
    private bool _showDisplayToggle;
    public GameObject IconPrefab;
    [Space(10)]
    public List<Clothing> Hairs;
    public GameObject HairsDisplay;
    [Space(10)]
    public List<Clothing> Faces;
    public GameObject FacesDisplay;
    [Space(10)]
    public List<Clothing> Outfits;
    public GameObject OutfitsDisplay;
    [Space(10)]
    public List<Clothing> Genders;
    public GameObject GendersDisplay;
    [Space(10)]
    public List<Clothing> SkinColors;
    public GameObject SkinColorsDisplay;

    private GameObject _hairBodyPart;
    private GameObject _faceBodyPart;
    private GameObject _outfitsBodyPart;
    private GameObject _genderBodyPart;
    private GameObject _skinColorBodyPart;
    private Sprite[] _spritesFromStorage;

    private void Start()
    {
        _spritesFromStorage = Resources.LoadAll<Sprite>("Clothing");

        _hairBodyPart = GameObject.Find("Hair Body Part");
        _faceBodyPart = GameObject.Find("Face Body Part");
        _outfitsBodyPart = GameObject.Find("Outfit Body Part");
        _skinColorBodyPart = GameObject.Find("Skin Color Body Part");
        _genderBodyPart = GameObject.Find("Gender Body Part");

        Invoke("SetupDisplays", 1f);
        Invoke("LoadCharacterAppearance", 1f);
    }

    // Whenever the player clicks on a button to view a body type's elements
    // this will make it so that its list of buttons will be hidden and viewed
    // whenever he clicks on the body type button.
    public void ToggleDisplay(Object display)
    {
        _showDisplayToggle = !_showDisplayToggle;
        GameObject displayObj = (GameObject)display;
        displayObj.SetActive(_showDisplayToggle);
    }

    // This function creates the icons in the body tab's lists (displays) using the
    // predefined wearables from the wearables json processed by the player.
    private void SetupDisplays()
    {
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            switch (clothing.BodyPart)
            {
                case "Hair":
                    {
                        LoadIcon(clothing, HairsDisplay, Hairs);
                        break;
                    }
                case "Face":
                    {
                        LoadIcon(clothing, FacesDisplay, Faces);
                        break;
                    }
                case "Outfit":
                    {
                        LoadIcon(clothing, OutfitsDisplay, Outfits);
                        break;
                    }
                // Yes they are not considered clothing, but since its related to
                // the player appearance and we didn't initially thought about these
                // elements, we will leave it like that for now.
                case "Gender":
                    {
                        LoadIcon(clothing, GendersDisplay, Genders);
                        break;
                    }
                case "Skin Color":
                    {
                        LoadIcon(clothing, SkinColorsDisplay, SkinColors);
                        break;
                    }
            }
        }
    }

    // This retrieves the icons from the storage and assigns them to buttons in the
    // appearance selection tabs on the right.
    private void LoadIcon(Clothing item, GameObject display, List<Clothing> listOfItems)
    {
        if (display != null)
        {
            listOfItems.Add(item);
            GameObject newIcon = Instantiate(IconPrefab, display.transform.GetChild(0).transform);
            newIcon.GetComponent<AppearanceSelector>().PortraitImage = item.PortraitImage;
            newIcon.GetComponent<AppearanceSelector>().BodyPart = item.BodyPart;

            Sprite sprite = Resources.Load<Sprite>("Icons/" + item.Icon);
            newIcon.GetComponent<Image>().sprite = sprite;
        }
    }

    // This imports the appearance elements according to the body parts and selected
    // appearance elements from the player and it will load them into the player's
    // character view.
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
                            case "Hair":
                                 _hairBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Face":
                                  _faceBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Outfit":
                                _outfitsBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Gender":
                                _genderBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                            case "Skin Color":
                                _skinColorBodyPart.GetComponent<Image>().sprite = sprite;
                                break;
                        }
                    }
                }
            }
        }
    }
}
