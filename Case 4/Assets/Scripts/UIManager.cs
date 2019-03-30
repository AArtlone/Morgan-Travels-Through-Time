using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private bool _SettingsUIDisplay = false;
    private bool _DiaryUIDisplay = false;
    private bool _InventoryDisplay = false;

    public GameObject SettingsDisplay;
    public GameObject DiaryDisplay;
    public GameObject InventoryDisplay;

    public Image _bodyIcon;
    public Image _faceIcon;
    public Image _hairIcon;
    public Image _topIcon;
    public Image _botIcon;
    public Image _shoesIcon;

    public GameObject[] MapAreas;

    private Sprite[] _spritesFromStorage;

    private void Start()
    {
        LoadCharacterAppearance();
        //if(Character.Instance.TutorialCompleted == false)
        //{
        //    foreach(GameObject mapArea in MapAreas)
        //    {
        //        if(mapArea.name != "Map Area 1")
        //        {
        //            mapArea.SetActive(false);
        //        }
        //    }
        //}
    }

    public void LoadCharacterAppearance()
    {
        _spritesFromStorage = Resources.LoadAll<Sprite>("Clothing/New Clothing");
        foreach (Clothing clothing in Character.Instance.Wearables)
        {
            if (clothing.BodyPart == "Body" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _bodyIcon.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Face" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _faceIcon.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Hair" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _hairIcon.sprite = sprite;
                    }
                }
            }
            if (clothing.BodyPart == "Top" && clothing.Selected == true)
            {
                foreach (Sprite sprite in _spritesFromStorage)
                {
                    if (sprite.name == clothing.Name)
                    {
                        _topIcon.sprite = sprite;
                    }
                }
            }
            if (_botIcon != null || _shoesIcon != null)
            {
                if (clothing.BodyPart == "Bot" && clothing.Selected == true)
                {
                    foreach (Sprite sprite in _spritesFromStorage)
                    {
                        if (sprite.name == clothing.Name)
                        {
                            _botIcon.sprite = sprite;
                        }
                    }
                }
                if (clothing.BodyPart == "Shoes" && clothing.Selected == true)
                {
                    foreach (Sprite sprite in _spritesFromStorage)
                    {
                        if (sprite.name == clothing.Name)
                        {
                            _shoesIcon.sprite = sprite;
                        }
                    }
                }
            }
        }
    }

    public void ToggleSettingsUI()
    {
        if (_DiaryUIDisplay)
        {
            DiaryDisplay.SetActive(false);
            _DiaryUIDisplay = !_DiaryUIDisplay;
        }
        _SettingsUIDisplay = !_SettingsUIDisplay;
        if (_SettingsUIDisplay)
        {
            SettingsDisplay.SetActive(true);
        } else if(!_SettingsUIDisplay)
        {
            SettingsDisplay.SetActive(false);
        }
    }
    public void ToggleDiaryUI()
    {
        if (_SettingsUIDisplay)
        {
            SettingsDisplay.SetActive(false);
            _SettingsUIDisplay = !_SettingsUIDisplay;
        }
        _DiaryUIDisplay = !_DiaryUIDisplay;
        if (_DiaryUIDisplay)
        {
            DiaryDisplay.SetActive(true);
        }
        else if (!_DiaryUIDisplay)
        {
            DiaryDisplay.SetActive(false);
        }
    }
    public void ToggleInventoryUI()
    {
        _InventoryDisplay = !_InventoryDisplay;
        if (_InventoryDisplay)
        {
            InventoryDisplay.SetActive(true);
        }
        else if (!_InventoryDisplay)
        {
            InventoryDisplay.SetActive(false);
        }
    }
}
