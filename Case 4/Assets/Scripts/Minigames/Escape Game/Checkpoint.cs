﻿using UnityEngine;
using System.Collections.Generic;

public class Checkpoint: MonoBehaviour
{
    public bool Passable;
    public bool Occupied;
    public List<QueueElement> QueueElements;
    public GameObject FirstQueueElement;
    public GameObject LastQueueElement;
}
