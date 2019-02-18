using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class Item : MonoBehaviour
{
    public string Name;
    public string Description;
    public string Active;
    public string AssetsImageName;

    public Item(string Name, string Description, string Active, string AssetsImageName)
    {
        this.Name = Name;
        this.Description = Description;
        this.Active = Active;
        this.AssetsImageName = AssetsImageName;
    }
}
