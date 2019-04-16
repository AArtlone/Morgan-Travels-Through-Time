using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DHManager : MonoBehaviour
{
    public List<DHSequence> Sequences = new List<DHSequence>();

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial Map Area" && Character.Instance.HasMap == false)
        {
            LoadSequence("Teach Character Portrait");
        }

        if (SceneManager.GetActiveScene().name == "Tutorial Map Area" && Character.Instance.HasDiary == true && Character.Instance.AreIconsExplained == true)
        {
            Debug.Log("AA");
            Character.Instance.AreIconsExplained = false;
            Character.Instance.RefreshJsonData();
            LoadSequence("Teach Diary and Backpack");
        }
    }

    public void LoadSequence(string name)
    {
        foreach (DHSequence seq in Sequences)
        {
            if (seq.Name == name)
            {
                foreach (GameObject obj in seq.ObjectsToActivateAtTheStart)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}
