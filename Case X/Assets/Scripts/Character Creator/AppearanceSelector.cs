using UnityEngine;
using System;
using UnityEngine.UI;

public class AppearanceSelector : MonoBehaviour
{
    [NonSerialized]
    public string PortraitImage;
    [NonSerialized]
    public string BodyPart;

    private GameObject _hairBodyPart;
    private GameObject _faceBodyPart;
    private GameObject _shoesBodyPart;
    private GameObject _topOutfitsBodyPart;
    private GameObject _bottomOutfitsBodyPart;
    private GameObject _genderBodyPart;
    private GameObject _raceBodyPart;
    private GameObject _skinColorBodyPart;
    private Sprite[] _spritesFromStorage;

    private void Start()
    {
        _spritesFromStorage = Resources.LoadAll<Sprite>("Clothing");

        _hairBodyPart = GameObject.Find("Hair Body Part");
        _faceBodyPart = GameObject.Find("Face Body Part");
        _shoesBodyPart = GameObject.Find("Shoes Body Part");
        _topOutfitsBodyPart = GameObject.Find("Top Outfit Body Part");
        _bottomOutfitsBodyPart = GameObject.Find("Bottom Outfit Body Part");
    }

    public void SelectAppearance()
    {
        switch (gameObject.transform.parent.transform.parent.transform.name)
        {
            // More to add...
            case "Hair Display":
                    LoadNewBodyPart(_hairBodyPart);
                    break;
            case "Face Display":
                    LoadNewBodyPart(_faceBodyPart);
                    break;
            case "Top Outfit Display":
                    LoadNewBodyPart(_topOutfitsBodyPart);
                    break;
            case "Bottom Outfit Display":
                    LoadNewBodyPart(_bottomOutfitsBodyPart);
                    break;
            case "Shoes Display":
                    LoadNewBodyPart(_shoesBodyPart);
                    break;
            case "Race Display":
                    break;
            case "Gender Display":
                    break;
            case "Skin Color Display":
                    break;
        }
    }

    private void LoadNewBodyPart(GameObject bodyPart)
    {
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (PortraitImage == clothing.PortraitImage)
            {
                clothing.Selected = true;
            } else if (PortraitImage != clothing.PortraitImage && BodyPart == clothing.BodyPart)
            {
                clothing.Selected = false;
            }
        }

        foreach (Sprite sprite in _spritesFromStorage)
        {
            Character.Instance.RefreshWearables();

            if (sprite.name == PortraitImage)
            {
                bodyPart.GetComponent<Image>().sprite = sprite;
            }
        }
    }

    public void ResetCharacterAppearance()
    {
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            clothing.Selected = false;
        }

        _hairBodyPart.GetComponent<Image>().sprite = null;
        _faceBodyPart.GetComponent<Image>().sprite = null;
        _topOutfitsBodyPart.GetComponent<Image>().sprite = null;
        _bottomOutfitsBodyPart.GetComponent<Image>().sprite = null;
        _shoesBodyPart.GetComponent<Image>().sprite = null;

        Character.Instance.RefreshWearables();
        SelectAppearance();
    }
}
