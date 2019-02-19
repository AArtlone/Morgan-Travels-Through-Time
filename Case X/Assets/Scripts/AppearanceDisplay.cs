using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceDisplay : MonoBehaviour
{
    private bool _showDisplayToggle;
    private Character _characterScript;
    private string _pathToWearables;
    public GameObject IconPrefab;
    public List<Clothing> Hairs;
    public GameObject HairsDisplay;
    public List<Clothing> Faces;
    public GameObject FacesDisplay;
    public List<Clothing> TopOutfits;
    public GameObject TopOutfitsDisplay;
    public List<Clothing> BottomOutfits;
    public GameObject BottomOutfitsDisplay;
    public List<Clothing> Shoes;
    public GameObject ShoesDisplay;
    public List<Clothing> Races;
    public GameObject RacesDisplay;
    public List<Clothing> Genders;
    public GameObject GendersDisplay;
    public List<Clothing> SkinColors;
    public GameObject SkinColorsDisplay;

    private void Start()
    {
        _characterScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();

        Invoke("SetupDisplays", 1f);
    }

    public void ToggleDisplay(Object display)
    {
        _showDisplayToggle = !_showDisplayToggle;
        GameObject displayObj = (GameObject)display;
        displayObj.SetActive(_showDisplayToggle);
    }

    private void SetupDisplays()
    {
        foreach (Clothing clothing in _characterScript.Wearables)
        {
            switch (clothing.BodyPart)
            {
                case "Hair":
                    {
                        Hairs.Add(clothing);
                        GameObject newIcon = Instantiate(IconPrefab, HairsDisplay.transform.GetChild(0).transform);

                        Sprite sprite = Resources.Load<Sprite>("Icons/" + clothing.Icon);
                        newIcon.GetComponent<Image>().sprite = sprite;
                        break;
                    }
                case "Face":
                    {
                        Faces.Add(clothing);
                        GameObject newIcon = Instantiate(IconPrefab, FacesDisplay.transform.GetChild(0).transform);

                        Sprite sprite = Resources.Load<Sprite>("Icons/" + clothing.Icon);
                        newIcon.GetComponent<Image>().sprite = sprite;
                        break;
                    }
                case "Top Outfit":
                    {
                        TopOutfits.Add(clothing);
                        GameObject newIcon = Instantiate(IconPrefab, TopOutfitsDisplay.transform.GetChild(0).transform);

                        Sprite sprite = Resources.Load<Sprite>("Icons/" + clothing.Icon);
                        newIcon.GetComponent<Image>().sprite = sprite;
                        break;
                    }
                case "Bottom Outfit":
                    {
                        BottomOutfits.Add(clothing);
                        GameObject newIcon = Instantiate(IconPrefab, BottomOutfitsDisplay.transform.GetChild(0).transform);

                        Sprite sprite = Resources.Load<Sprite>("Icons/" + clothing.Icon);
                        newIcon.GetComponent<Image>().sprite = sprite;
                        break;
                    }
                case "Shoes":
                    {
                        Shoes.Add(clothing);
                        GameObject newIcon = Instantiate(IconPrefab, ShoesDisplay.transform.GetChild(0).transform);

                        Sprite sprite = Resources.Load<Sprite>("Icons/" + clothing.Icon);
                        newIcon.GetComponent<Image>().sprite = sprite;
                        break;
                    }
                // Yes they are not considered clothing, but since its related to
                // the player appearance and we didn't initially thought about these
                // elements, we will leave it like that for now.
                case "Race":
                    {
                        Races.Add(clothing);
                        GameObject newIcon = Instantiate(IconPrefab, RacesDisplay.transform.GetChild(0).transform);

                        Sprite sprite = Resources.Load<Sprite>("Icons/" + clothing.Icon);
                        newIcon.GetComponent<Image>().sprite = sprite;
                        break;
                    }
                case "Gender":
                    {
                        Genders.Add(clothing);
                        GameObject newIcon = Instantiate(IconPrefab, GendersDisplay.transform.GetChild(0).transform);

                        Sprite sprite = Resources.Load<Sprite>("Icons/" + clothing.Icon);
                        newIcon.GetComponent<Image>().sprite = sprite;
                        break;
                    }
                case "Skin Color":
                    {
                        SkinColors.Add(clothing);
                        GameObject newIcon = Instantiate(IconPrefab, SkinColorsDisplay.transform.GetChild(0).transform);

                        Sprite sprite = Resources.Load<Sprite>("Icons/" + clothing.Icon);
                        newIcon.GetComponent<Image>().sprite = sprite;
                        break;
                    }
            }
        }
    }
}
