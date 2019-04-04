using TMPro;
using UnityEngine;

public class LanguageController : MonoBehaviour
{
    public enum Language { English, Dutch };
    [Header("Current language on this text field.")]
    public Language SelectedLanguage;

    [Header("Fill in the different language variations of this text field here!")]
    [TextArea(0, 100)]
    public string English;
    [TextArea(0, 100)]
    public string Dutch;

    // Component for the text field that will be overwritten depending on the
    // currently selected language by the player.
    private TextMeshProUGUI _textField;

    private SettingsManager _settingsManager;

    private void Start()
    {
        _textField = GetComponent<TextMeshProUGUI>();

        _settingsManager = FindObjectOfType<SettingsManager>();
        
        if (!_settingsManager.LanguageControllers.Contains(this))
        {
            _settingsManager.LanguageControllers.Add(this);
        }

        UpdateCurrentLanguage();
        LoadLanguage(SelectedLanguage);
    }

    public void UpdateCurrentLanguage()
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
        
    public void LoadLanguage(Language language)
    {
        switch (language)
        {
            case Language.English:
                _textField.text = English;
                break;
            case Language.Dutch:
                _textField.text = Dutch;
                break;
            default:
                _textField.text = Dutch;
                break;
        }
    }
}
