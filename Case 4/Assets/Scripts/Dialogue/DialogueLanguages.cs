using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DialogueLanguages
{
    [Tooltip("The language here defines what dialogue will be filtered/displayed depending on the player's language configuration.")]
    public string Language;
    public List<Dialogue> Dialogue;
}


