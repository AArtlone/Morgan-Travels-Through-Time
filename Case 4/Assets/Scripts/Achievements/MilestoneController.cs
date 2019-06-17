using System.Collections.Generic;
using UnityEngine;

public class MilestoneController : MonoBehaviour
{
    public string AchievementToComplete;
    public List<Clothing> ClothingToGive;

    void Start()
    {
        AchievementManager.Instance.GetAchievement("Tutorial Complete");
        foreach (Clothing cloth in ClothingToGive)
        {
            Character.Instance.AddWearable(cloth);
        }
    }
}
