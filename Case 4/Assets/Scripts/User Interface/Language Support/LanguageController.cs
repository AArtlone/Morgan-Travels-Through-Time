using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageController : MonoBehaviour
{
    [Header("Tick this when the text must not change, but the font should!")]
    public bool DisableLanguageToggling;
    public enum Language { English, Dutch };
    [Header("Current language on this text field.")]
    public Language SelectedLanguage;

    public enum Font { HandleeRegular, JosefinSansRegular };

    [Header("Fill in the different language variations of this text field here!")]
    [TextArea(0, 100)]
    public string English;
    [TextArea(0, 100)]
    public string Dutch;

    // Component for the text field that will be overwritten depending on the
    // currently selected language by the player.
    private TextMeshProUGUI _textField;
    private Text _textInputField;

    private SettingsManager _settingsManager;

    private void Start()
    {
        _textField = GetComponent<TextMeshProUGUI>();
        
        if (GetComponent<Text>())
        {
            _textInputField = GetComponent<Text>();
        }

        _settingsManager = FindObjectOfType<SettingsManager>();
        
        if (_settingsManager != null)
        {
            if (!_settingsManager.LanguageControllers.Contains(this))
            {
                _settingsManager.LanguageControllers.Add(this);
            }
        }

        if (DisableLanguageToggling == false)
        {
            UpdateCurrentLanguage();
        }
        UpdateCurrentFont();
    }

    /// <summary>
    /// This function runs whenever the player selects a language's flag in the
    /// settings window.
    /// </summary>
    public void UpdateCurrentLanguage()
    {
        if (DisableLanguageToggling == false && SettingsManager.Instance != null)
        {
            switch (SettingsManager.Instance.Language)
            {
                case "English":
                    SelectedLanguage = Language.English;
                    break;
                case "Dutch":
                    SelectedLanguage = Language.Dutch;
                    break;
            }

            LoadLanguage(SelectedLanguage);
        }
    }

    public void UpdateCurrentFont()
    {
        foreach (Font font in Enum.GetValues(typeof(Font)))
        {
            if (font.ToString() == SettingsManager.Instance.Font)
            {
                if (_textField != null)
                    _textField.font = Resources.Load<TMP_FontAsset>("Fonts/" + font.ToString() + " SDF");
            }
        }
    }
    
    /// <summary>
    /// This function updates this class's game object text field to reflect the
    /// currently selected language in the player's configuration.
    /// </summary>
    /// <param name="language"></param>
    public void LoadLanguage(Language language)
    {
        if (DisableLanguageToggling == false)
        {
            switch (language)
            {
                case Language.English:
                    LoadTextIntoField(English);
                    if (_textField != null)
                    {
                        _textField.text = English;
                    }
                    break;
                case Language.Dutch:
                    LoadTextIntoField(Dutch);
                    if (_textField != null)
                    {
                        _textField.text = Dutch;
                    }
                    break;
                default:
                    LoadTextIntoField(Dutch);
                    if (_textField != null)
                    {
                        _textField.text = Dutch;
                    }
                    break;
            }
        }
    }

    private void LoadTextIntoField(string text) {
        if (_textInputField != null)
        {
            _textInputField.text = text;
        }

        if (_textField != null)
        {
            _textField.text = text;
        }
    }
}
