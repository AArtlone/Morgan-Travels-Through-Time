using System.Collections.Generic;
using System;

[Serializable]
public class NPCLog
{
    public string Name;
    public List<DialogueLogObject> Log = new List<DialogueLogObject>();
}
