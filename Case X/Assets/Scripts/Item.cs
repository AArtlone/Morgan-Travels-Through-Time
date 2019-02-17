using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class Item : MonoBehaviour
{
    public string Name;
    public string Description;
    public string Passives;
    public string Actives;
    
    public Item(string Name, string Description, string Passives, string Actives)
    {
        this.Name = Name;
        this.Description = Description;
        this.Passives = Passives;
        this.Actives = Actives;
    }
}
