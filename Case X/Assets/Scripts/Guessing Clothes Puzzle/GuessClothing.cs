using UnityEngine;
using System;

[Serializable]
public class GuessClothing
{
    [Tooltip("Not mandatory, but can be assigned for visibility reasons.")]
    public string Name;
    public string Sprite;
    public int Points;

    public GuessClothing(string Sprite, int Points)
    {
        this.Sprite = Sprite;
        this.Points = Points;
    }
}
