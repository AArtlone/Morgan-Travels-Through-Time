using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MilestoneController : MonoBehaviour
{
    public string AchievementToComplete;
    public List<Clothing> ClothingToGive;

    void Start()
    {
        bool exists = false;
        foreach (Achievement achievement in AchievementManager.Instance.AllAchievements)
        {
            if (achievement.Name == AchievementToComplete && achievement.Status == true)
            {
                exists = true;
                break;
            }
        }
        if (exists == false)
        {
            AchievementManager.Instance.GetAchievement(AchievementToComplete);
            /*foreach (Clothing cloth in ClothingToGive)
            {
                Character.Instance.AddWearable(cloth);
            }*/

            string defaultTextEnglish = Resources.Load<TextAsset>("Default World Data/Wearables1").text;
            string defaultTextDutch = Resources.Load<TextAsset>("Default World Data/WearablesDutch1").text;

            File.WriteAllText(Application.persistentDataPath + "/Wearables.json", defaultTextEnglish);
            File.WriteAllText(Application.persistentDataPath + "/WearablesDutch.json", defaultTextDutch);
            Character.Instance.SetupWorldData();
            Character.Instance.RefreshWearables();
        }
    }
}
