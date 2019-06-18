using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    private string _achievementsJsonFilePath;
    public List<Achievement> AllAchievements = new List<Achievement>();
    public GameObject AchievementPrefab;
    private Animator _achievementPrefabAnimator;

    private void Awake()
    {
        _achievementPrefabAnimator = AchievementPrefab.GetComponent<Animator>();
        _achievementsJsonFilePath = Application.persistentDataPath + "/Achievements.json";
        if (File.Exists(_achievementsJsonFilePath))
            Setup(true);
        else
            Setup(false);

        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
        }
    }

    private void Setup(bool exists)
    {
        JsonData achievementsData;
        if (exists)
            achievementsData = JsonMapper.ToObject(
                    File.ReadAllText(_achievementsJsonFilePath));
        else
            achievementsData = JsonMapper.ToObject(
                Resources.Load<TextAsset>("Default World Data/Achievements").text);

        AllAchievements.Clear();
        for (int i = 0; i < achievementsData["Achievements"].Count; i++)
        {
            AllAchievements.Add(new Achievement(achievementsData["Achievements"][i]["Name"].ToString(),
                achievementsData["Achievements"][i]["NameDutch"].ToString(),
                achievementsData["Achievements"][i]["Icon"].ToString(),
                achievementsData["Achievements"][i]["Status"].ToString() == "True" ? true : false));
        }

        if (exists == false)
        {
            TextAsset achievementData = Resources.Load<TextAsset>("Default World Data/Achievements");

            File.WriteAllText(Application.persistentDataPath + "/Achievements.json", achievementData.text);
        }
    }

    private void Refresh()
    {
        string newAchievements = "{";
        newAchievements += InsertNewLineTabs(1);
        newAchievements += "\"Achievements\": [";
        foreach (Achievement achievement in AllAchievements)
        {
            newAchievements += InsertNewLineTabs(2);
            newAchievements += "{";
            newAchievements += InsertNewLineTabs(3);
            newAchievements += "\"Name\": " + "\"" + achievement.Name + "\" ,";
            newAchievements += InsertNewLineTabs(3);
            newAchievements += "\"NameDutch\": " + "\"" + achievement.NameDutch + "\" ,";
            newAchievements += InsertNewLineTabs(3);
            newAchievements += "\"Icon\": " + "\"" + achievement.Icon + "\" ,";
            newAchievements += InsertNewLineTabs(3);
            newAchievements += "\"Status\": " + (achievement.Status.ToString() == "True" ? "true" : "false");
            newAchievements += InsertNewLineTabs(2);
            newAchievements += "},";
        }
        newAchievements = newAchievements.Substring(0, newAchievements.Length - 1);
        newAchievements += InsertNewLineTabs(1);
        newAchievements += "]";
        newAchievements += Environment.NewLine + "}";

        File.WriteAllText(_achievementsJsonFilePath, newAchievements);
    }

    private string InsertNewLineTabs(int numberOfTabs)
    {
        string whiteSpace = Environment.NewLine;
        for (int i = 0; i < numberOfTabs; i++)
        {
            whiteSpace += "\t";
        }

        return whiteSpace;
    }

    public void GetAchievement(string name)
    {
        foreach (Achievement achievement in AllAchievements)
        {
            if (achievement.Name == name)
            {
                achievement.Status = true;
                AchievementPrefab.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Achievements/" + achievement.Icon);
                if (SettingsManager.Instance.Language == "English")
                {
                    AchievementPrefab.GetComponentInChildren<TextMeshProUGUI>().text = achievement.Name;
                } else if (SettingsManager.Instance.Language == "Dutch")
                {
                    AchievementPrefab.GetComponentInChildren<TextMeshProUGUI>().text = achievement.NameDutch;
                }

                _achievementPrefabAnimator.SetBool("Toggle", true);
                StartCoroutine(ReturnAchievement());
            }
        }
        Refresh();
    }

    private IEnumerator ReturnAchievement()
    {
        yield return new WaitForSeconds(7f);
        _achievementPrefabAnimator.SetBool("Toggle", false);
    }
}
