using UnityEngine;

public class SpellingPuzzleObject : MonoBehaviour
{
    public string Name;
    public string NameDutch;
    public bool Completed;
    public int Stars;
    public SpellingPuzzleObject(string name, string nameDutch, bool completed, int stars)
    {
        Name = name;
        NameDutch = nameDutch;
        Completed = completed;
        Stars = stars;
    }
}
