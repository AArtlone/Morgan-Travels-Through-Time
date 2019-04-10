using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SettingsManager : MonoBehaviour
{
    [NonSerialized]
    public static SettingsManager Instance;

    public string Language;
    public int ResolutionWidth;
    public int ResolutionHeight;
    public int MasterVolume;
    public int SoundEffectsVolume;
    public int BackgroundMusicVolume;

    public List<AudioSource> BackgroundSources;
    public List<AudioSource> SoundSources;

    private string _pathToSettingsJson;
    public LanguageToggle[] LanguageIconObjects;
    public List<Image> LanguageIcons;
    public List<LanguageController> LanguageControllers = new List<LanguageController>();

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _pathToSettingsJson = Application.persistentDataPath + "/Settings.json";

        if (File.Exists(_pathToSettingsJson))
        {
            LoadSettingsFromFile(_pathToSettingsJson);
        }
        else
        {
            ResetSettings();

            string settingsText = "{\"Settings\":{";
            settingsText += "\"Language\":" + "\"" + Language + "\",";
            settingsText += "\"ResolutionWidth\":" + ResolutionWidth + ",";
            settingsText += "\"ResolutionHeight\":" + ResolutionHeight + ","; ;
            settingsText += "\"MasterVolume\":" + MasterVolume + ","; ;
            settingsText += "\"SoundEffectsVolume\":" + SoundEffectsVolume + ",";
            settingsText += "\"BackgroundMusicVolume\":" + BackgroundMusicVolume;
            settingsText += "}}";

            File.WriteAllText(_pathToSettingsJson, settingsText);
        }
        UpdateHiglightedLanguageIcons();
    }

    public void DeleteJsonFile()
    {
        File.Delete(_pathToSettingsJson);
        Destroy(Instance);
        Destroy(gameObject);
    }

    /// <summary>
    /// Loads the file from the data storage of the device instead of the game
    /// resources.
    /// </summary>
    /// <param name="filePath"></param>
    private void LoadSettingsFromFile(string filePath)
    {
        string settingsData = File.ReadAllText(_pathToSettingsJson);
        JsonData settingsJson = JsonMapper.ToObject(settingsData);

        Language = settingsJson["Settings"]["Language"].ToString();
        ResolutionWidth = int.Parse(settingsJson["Settings"]["ResolutionWidth"].ToString());
        ResolutionHeight = int.Parse(settingsJson["Settings"]["ResolutionHeight"].ToString());
        MasterVolume = int.Parse(settingsJson["Settings"]["MasterVolume"].ToString());
        SoundEffectsVolume = int.Parse(settingsJson["Settings"]["SoundEffectsVolume"].ToString());
        BackgroundMusicVolume = int.Parse(settingsJson["Settings"]["BackgroundMusicVolume"].ToString());
    }

    /// <summary>
    /// Gets the existing data from the settings singleton(this) and returns it
    /// as a json string.
    /// </summary>
    /// <returns></returns>
    private string ExtractCurrentSettings()
    {
        string settingsText = "{\"Settings\":{";
        settingsText += "\"Language\":" + "\"" + Language + "\",";
        settingsText += "\"ResolutionWidth\":" + ResolutionWidth + ",";
        settingsText += "\"ResolutionHeight\":" + ResolutionHeight + ","; ;
        settingsText += "\"MasterVolume\":" + MasterVolume + ","; ;
        settingsText += "\"SoundEffectsVolume\":" + SoundEffectsVolume + ",";
        settingsText += "\"BackgroundMusicVolume\":" + BackgroundMusicVolume;
        settingsText += "}}";

        return settingsText;
    }

    /// <summary>
    /// Resets the settings of the game using the default configuration in resources.
    /// </summary>
    public void ResetSettings()
    {
        TextAsset settingsText = Resources.Load<TextAsset>("Default World Data/Settings");
        JsonData settingsJson = JsonMapper.ToObject(settingsText.text);

        // Reading and assigning data from the settings file to this manager class.
        Language = settingsJson["Settings"]["Language"].ToString();
        ResolutionWidth = int.Parse(settingsJson["Settings"]["ResolutionWidth"].ToString());
        ResolutionHeight = int.Parse(settingsJson["Settings"]["ResolutionHeight"].ToString());
        MasterVolume = int.Parse(settingsJson["Settings"]["MasterVolume"].ToString());
        SoundEffectsVolume = int.Parse(settingsJson["Settings"]["SoundEffectsVolume"].ToString());
        BackgroundMusicVolume = int.Parse(settingsJson["Settings"]["BackgroundMusicVolume"].ToString());
    }

    /// <summary>
    /// Updates the existing settings configuration in the json file based on this
    /// class's parameters whenever it is configured by the player.
    /// </summary>
    public void RefreshSettings()
    {
        string newSettingsData = JsonUtility.ToJson(this);
        File.WriteAllText(_pathToSettingsJson, ExtractCurrentSettings());
    }

    /// <summary>
    /// Highlights and De-Highlights language flags in the settings window depending
    /// on the player's currently selected language.
    /// </summary>
    public void UpdateHiglightedLanguageIcons()
    {
        switch (Language)
        {
            case "English":
                foreach (Image languageIcon in LanguageIcons)
                {
                    if (languageIcon != null)
                    {
                        if (languageIcon.name == "English")
                        {
                            languageIcon.color = new Color(255, 255, 255, 1f);
                        }
                        else
                        {
                            languageIcon.color = new Color(255, 255, 255, 0.5f);
                        }
                    }
                }
                break;
            case "Dutch":
                foreach (Image languageIcon in LanguageIcons)
                {
                    if (languageIcon != null)
                    {
                        if (languageIcon.name == "Dutch")
                        {
                            languageIcon.color = new Color(255, 255, 255, 1f);
                        }
                        else
                        {
                            languageIcon.color = new Color(255, 255, 255, 0.5f);
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// This function runs whenever the audio settings' volume parameter of
    /// audio sources in the scenes is updated.
    /// </summary>
    public void UpdateAudioVolume()
    {
        foreach (AudioSource audioSource in BackgroundSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = SoundEffectsVolume / 100f;
            }
        }

        foreach (AudioSource audioSource in BackgroundSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = BackgroundMusicVolume / 100f;
            }
        }
    }
}
