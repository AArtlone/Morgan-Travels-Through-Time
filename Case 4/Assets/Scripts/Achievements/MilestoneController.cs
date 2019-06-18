using System.Collections.Generic;
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
            foreach (Clothing cloth in ClothingToGive)
            {
                Character.Instance.AddWearable(cloth);
            }
        }
    }
}
