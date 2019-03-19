using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class AppearanceSelector : MonoBehaviour
{
    [NonSerialized]
    public string PortraitImage;
    [NonSerialized]
    public string BodyPart;

    private GameObject _bodyBodyPart;
    private GameObject _faceBodyPart;
    private GameObject _hairBodyPart;
    private GameObject _topBodyPart;
    private GameObject _botBodyPart;
    private GameObject _shoesBodyPart;
    private Sprite[] _spritesFromStorage;

    private void Start()
    {
        _spritesFromStorage = Resources.LoadAll<Sprite>("Clothing");

        _bodyBodyPart = GameObject.Find("Body Body Part");
        _faceBodyPart = GameObject.Find("Face Body Part");
        _hairBodyPart = GameObject.Find("Hair Body Part");
        _topBodyPart = GameObject.Find("Top Body Part");
        _botBodyPart = GameObject.Find("Bot Body Part");
        _shoesBodyPart = GameObject.Find("Shoes Body Part");
    }

    public void SelectAppearance()
    {
        switch (gameObject.transform.parent.transform.parent.transform.name)
        {
            case "Bodies Display":
                LoadNewBodyPart(_bodyBodyPart);
                break;
            case "Faces Display":
                LoadNewBodyPart(_faceBodyPart);
                break;
            case "Hairs Display":
                LoadNewBodyPart(_hairBodyPart);
                break;
            case "Tops Display":
                LoadNewBodyPart(_topBodyPart);
                break;
            case "Bots Display":
                LoadNewBodyPart(_botBodyPart);
                break;
            case "Shoes Display":
                LoadNewBodyPart(_shoesBodyPart);
                break;
        }
    }

    private void LoadNewBodyPart(GameObject bodyPart)
    {
        if(bodyPart.GetComponent<Image>().color.a == 0)
        {
            Color color = bodyPart.GetComponent<Image>().color;
            bodyPart.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);
        }
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
            if(SceneManager.GetActiveScene().name == "Begining Character Creation")
            {
                Character.Instance.RefreshWearables();
            }

            if (sprite.name == PortraitImage)
            {
                bodyPart.GetComponent<Image>().sprite = sprite;
            }
        }
    }

    public void ResetCharacterAppearance()
    {
        TextAsset wearablesData = Resources.Load<TextAsset>("Default World Data/Wearables");
        JsonData wearablesJsonData = JsonMapper.ToObject(wearablesData.text);
        
        for (int i = 0; i < wearablesJsonData["Wearables"].Count; i++)
        {
            if (wearablesJsonData["Wearables"][i]["Selected"].ToString() == "True")
            {
                Sprite sprite = Resources.Load<Sprite>("Clothing/New Clothing/" + wearablesJsonData["Wearables"][i]["PortraitImage"].ToString());
                if (wearablesJsonData["Wearables"][i]["BodyPart"].ToString() == "Body") {
                    _bodyBodyPart.GetComponent<Image>().sprite = sprite;
                }
                else if (wearablesJsonData["Wearables"][i]["BodyPart"].ToString() == "Face")
                {
                    _faceBodyPart.GetComponent<Image>().sprite = sprite;
                }
            }
        }
        _hairBodyPart.GetComponent<Image>().sprite = null;
        _topBodyPart.GetComponent<Image>().sprite = null;
        _botBodyPart.GetComponent<Image>().sprite = null;
        _shoesBodyPart.GetComponent<Image>().sprite = null;

        Color hairColor = _hairBodyPart.GetComponent<Image>().color;
        _hairBodyPart.GetComponent<Image>().color = new Color(hairColor.r, hairColor.g, hairColor.b, 0);
        Color topColor = _topBodyPart.GetComponent<Image>().color;
        _topBodyPart.GetComponent<Image>().color = new Color(topColor.r, topColor.g, topColor.b, 0);
        Color botColor = _botBodyPart.GetComponent<Image>().color;
        _botBodyPart.GetComponent<Image>().color = new Color(botColor.r, botColor.g, botColor.b, 0);
        Color shoesColor = _shoesBodyPart.GetComponent<Image>().color;
        _shoesBodyPart.GetComponent<Image>().color = new Color(shoesColor.r, shoesColor.g, shoesColor.b, 0);

        Character.Instance.RefreshWearables();
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == "Body" && clothing.Selected == true)
            {
                continue;
            }
            if (clothing.BodyPart == "Face" && clothing.Selected == true)
            {
                continue;
            }
            clothing.Selected = false;
        }

        SelectAppearance();
    }
}
