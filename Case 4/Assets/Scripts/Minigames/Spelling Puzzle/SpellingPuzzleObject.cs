using UnityEngine;

public class SpellingPuzzleObject : MonoBehaviour
{
    public string Name;
    public string NameDutch;
    public string SceneName;
    public bool Completed;
    public int Stars;
    public SpellingPuzzleObject(string name, string nameDutch, string sceneName, bool completed, int stars)
    {
        Name = name;
        NameDutch = nameDutch;
        Completed = completed;
        SceneName = sceneName;
        Stars = stars;
    }
}
