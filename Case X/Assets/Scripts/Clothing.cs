using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Clothing : MonoBehaviour
{
    public string BodyPart;
    public string Name;
    #region Stats
    public int Stamina;
    public int Knowledge;
    public int Fitness;
    public int Charisma;
    #endregion

    public Clothing(string BodyPart, string Name, int Stamina, int Knowledge, int Fitness, int Charisma)
    {
        this.BodyPart = BodyPart;
        this.Name = Name;
        this.Stamina = Stamina;
        this.Knowledge = Knowledge;
        this.Fitness = Fitness;
        this.Charisma = Charisma;
    }
}
