using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

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

    private string _pathToSettingsJson;

    // Settings that will be loaded into the interface window
    private int _currentResolutionIndex;
    private string _currentResolutionName;
    List<string> Resolutions = new List<string>();
    public TextMeshProUGUI _currentResolutionText;

    private void Awake()
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
    }

    private void Start()
    {
        _currentResolutionText = GameObject.FindGameObjectWithTag("ResolutionSetting").GetComponent<TextMeshProUGUI>();
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
    /// Updated the existing
    /// </summary>
    public void RefreshSettings()
    {
        string newSettingsData = JsonUtility.ToJson(this);
        File.WriteAllText(_pathToSettingsJson, ExtractCurrentSettings());
    }

    public void NextResolution()
    {
        GetCurrentResolution();

        GetCurrentResolution();

        _currentResolutionIndex++;
        if (_currentResolutionIndex > Resolutions.Count - 1)
        {
            _currentResolutionIndex = Resolutions.Count - 1;
        }

        _currentResolutionText.text = Resolutions[_currentResolutionIndex];
    }

    public void PreviousResolution()
    {
        GetCurrentResolution();

        _currentResolutionIndex--;
        if (_currentResolutionIndex <= 0)
        {
            _currentResolutionIndex = 0;
        }

        _currentResolutionText.text = Resolutions[_currentResolutionIndex];
    }

    public void GetCurrentResolution()
    {
        _currentResolutionText = GameObject.FindGameObjectWithTag("ResolutionSetting").GetComponent<TextMeshProUGUI>();

        _currentResolutionName = string.Empty;
        _currentResolutionName += Screen.currentResolution.width + "x" + Screen.currentResolution.height;

        for (int i = 0; i < Resolutions.Count; i++)
        {
            if (_currentResolutionName == Resolutions[i])
            {
                _currentResolutionIndex = i;
                break;
            }
        }
    }
}
