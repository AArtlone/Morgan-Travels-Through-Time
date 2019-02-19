﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Clothing : MonoBehaviour
{
    public string BodyPart;
    public string Name;
    public string Icon;
    public string PortraitImage;
    #region Stats
    public int Stamina;
    public int Knowledge;
    public int Fitness;
    public int Charisma;
    #endregion

    public Clothing(string BodyPart, string Name, string Icon, string PortraitImage, int Stamina, int Knowledge, int Fitness, int Charisma)
    {
        this.BodyPart = BodyPart;
        this.Name = Name;
        this.Icon = Icon;
        this.PortraitImage = PortraitImage;
        this.Stamina = Stamina;
        this.Knowledge = Knowledge;
        this.Fitness = Fitness;
        this.Charisma = Charisma;
    }
}
