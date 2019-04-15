using System.Collections.Generic;
using UnityEngine;

public class DHManager : MonoBehaviour
{
    public List<DHSequence> Sequences = new List<DHSequence>();

    private void Start()
    {
        if (Character.Instance.TutorialCompleted == false)
        {
            LoadSequence("Teach Main Interface");
        }
    }

    public void LoadSequence(string name)
    {
        foreach (DHSequence seq in Sequences)
        {
            if (seq.Name == name)
            {
                foreach (GameObject obj in seq.ObjectsToActivate)
                {
                    obj.SetActive(true);
                    seq.gameObject.SetActive(true);
                }
            }
        }
    }
}
