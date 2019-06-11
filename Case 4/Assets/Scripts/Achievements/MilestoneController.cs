using UnityEngine;

public class MilestoneController : MonoBehaviour
{
    public string AchievementToComplete;
    public Clothing ClothingToGive;

    void Start()
    {
        AchievementManager.Instance.GetAchievement("Tutorial Complete");
        Character.Instance.AddWearable(ClothingToGive);
    }
}
