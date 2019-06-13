using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontToggle : MonoBehaviour
{
    private List<string> _fontsList = new List<string>();
    public TextMeshProUGUI CurrentFontInSettings;
    public TextMeshProUGUI FontLabel;
    private int _currentFontIndex;

    private void Start()
    {
        InitializeFontsList();

        if (CurrentFontInSettings != null)
        {
            if (SettingsManager.Instance.Font.Length > 8)
            {
                CurrentFontInSettings.text = SettingsManager.Instance.Font.Substring(0, 8) + "..";
            } else
            {
                CurrentFontInSettings.text = SettingsManager.Instance.Font;
            }
        }
    }

    private void InitializeFontsList()
    {
        var arrayOfFonts = Enum.GetValues(typeof(LanguageController.Font));
        foreach (var item in arrayOfFonts) {
            _fontsList.Add(item.ToString());
        }
    }

    public void ChangeFont(string direction)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.ButtonPress);

        if (direction == "Forward" || direction == "Forwards")
        {
            _currentFontIndex++;
        } else if (direction == "Backward" || direction == "Backwards")
        {
            _currentFontIndex--;
        }

        if (_currentFontIndex < 0)
        {
            _currentFontIndex = _fontsList.Count - 1;
        } else if (_currentFontIndex > _fontsList.Count - 1)
        {
            _currentFontIndex = 0;
        }

        SettingsManager.Instance.Font = _fontsList[_currentFontIndex];
        CurrentFontInSettings.text = SettingsManager.Instance.Font.Substring(0, 8) + "..";

        foreach (LanguageController font in SettingsManager.Instance.LanguageControllers)
        {
            if (font != null)
            {
                font.UpdateCurrentFont();
            }
        }

        SettingsManager.Instance.RefreshSettings();
    }
}
