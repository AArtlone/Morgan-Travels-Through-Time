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

    /// <summary>
    /// This function updates the language setting in the player's file and updates
    /// the icons in the settings window and every text field currently in the scene.
    /// </summary>
    /// <param name="LanguageToToggle"></param>
    public void ChangeLanguage(string languageToToggle)
    {
        switch (languageToToggle)
        {
            case "English":
                _settingsManager.Language = "English";
                foreach (LanguageController textLabel in _settingsManager.LanguageControllers)
                {
                    textLabel?.LoadLanguage(LanguageController.Language.English);
                }
                break;
            case "Dutch":
                _settingsManager.Language = "Dutch";
                foreach (LanguageController textLabel in _settingsManager.LanguageControllers)
                {
                    textLabel?.LoadLanguage(LanguageController.Language.Dutch);
                }
                break;
        }

        RefreshIconSprites();
        _settingsManager.RefreshSettings();
    }
    public void ChangeFont()
    {
        foreach (LanguageController textLabel in _settingsManager.LanguageControllers)
        {
            textLabel?.UpdateCurrentFont();
        }
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
