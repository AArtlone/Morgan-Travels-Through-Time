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
        _genderBodyPart = GameObject.Find("Gender Body Part");
        _skinColorBodyPart = GameObject.Find("Skin Color Body Part");
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
            case "Outfit Display":
                LoadNewBodyPart(_outfitsBodyPart);
                break;
            case "Gender Display":
                LoadNewBodyPart(_genderBodyPart);
                break;
            case "Skin Color Display":
                LoadNewBodyPart(_skinColorBodyPart);
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
        _outfitsBodyPart.GetComponent<Image>().sprite = null;
        _genderBodyPart.GetComponent<Image>().sprite = null;
        _skinColorBodyPart.GetComponent<Image>().sprite = null;

        Character.Instance.RefreshWearables();
        SelectAppearance();
    }
}
