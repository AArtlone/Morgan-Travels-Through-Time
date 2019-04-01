using System.Collections.Generic;
using UnityEngine;

public class MapPart : MonoBehaviour
{
    [Header("Place requirements necessary to enter this area part in these list!")]
    public List<Objective> ObjectivesRequired = new List<Objective>();
    public List<Clothing> ClothingRequired = new List<Clothing>();
}
