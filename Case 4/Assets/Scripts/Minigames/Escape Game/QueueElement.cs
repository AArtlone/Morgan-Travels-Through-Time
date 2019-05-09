using System;
using UnityEngine;

public class QueueElement : MonoBehaviour
{
    public bool IsAvaialbe;
    [NonSerialized]
    public float maximumDistance = -3f; //the maximum distance between the first and the last refugee
}
