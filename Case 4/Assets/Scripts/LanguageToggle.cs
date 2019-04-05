using UnityEngine;
using UnityEngine.UI;

public class LanguageToggle : MonoBehaviour
{
    private SettingsManager _settingsManager;

    private void Start()
    {
        _settingsManager = FindObjectOfType<SettingsManager>();

        if (!_settingsManager.LanguageIcons.Contains(GetComponent<Image>()))
        {
            _settingsManager.LanguageIcons.Add(GetComponent<Image>());
        }

        SettingsManager.Instance.UpdateHiglightedLanguageIcons();
    }

    public void ChangeLanguage(string LanguageToToggle)
    {
        switch (LanguageToToggle)
        {
            case "English":
                _settingsManager.Language = "English";
                foreach (LanguageController textLabel in _settingsManager.LanguageControllers)
                {
                    if (textLabel != null)
                    {
                        textLabel.LoadLanguage(LanguageController.Language.English);
                    }
                }
                break;
            case "Dutch":
                _settingsManager.Language = "Dutch";
                foreach (LanguageController textLabel in _settingsManager.LanguageControllers)
                {
                    if (textLabel != null)
                    {
                        textLabel.LoadLanguage(LanguageController.Language.Dutch);
                    }
                }
                break;
        }

        RefreshIconSprites();
        _settingsManager.RefreshSettings();
    }

    private void RefreshIconSprites()
    {
        foreach (Image image in _settingsManager.LanguageIcons)
        {
            if (image != null)
            {
                if (image.gameObject != gameObject)
                {
                    image.color = new Color(255, 255, 255, 0.5f);
                }
                else
                {
                    image.color = new Color(255, 255, 255, 1f);
                }
            }
        }
    }
}
