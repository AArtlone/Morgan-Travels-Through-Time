using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothing : MonoBehaviour
{
    public string BodyPart;
    public string Name;

    public Clothing(string BodyPart, string Name)
    {
        this.BodyPart = BodyPart;
        this.Name = Name;
    }
}
