using UnityEngine;

public class SpellingPuzzleObject : MonoBehaviour
{
    public string Name;
    public bool Completed;
    public int Stars;
    public SpellingPuzzleObject(string name, bool completed, int stars)
    {
        Name = name;
        Completed = completed;
        Stars = stars;
    }
}
