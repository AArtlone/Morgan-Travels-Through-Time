using System.Collections.Generic;
using UnityEngine;

public class MapEnvironmentManager : MonoBehaviour
{
    public GameObject HiddenObjectPuzzleNpc;
    public GameObject ClothingPuzzleNpc;
    public GameObject EscapeGameNpc;
    private GameObject _dialogueNpc;
    public GameObject CloseButton;
    private GameObject _playerIcon;
    
    public List<Sequence> SequencesOfTutorial = new List<Sequence>();

    private int _currentIndexOfSequence = 0;
    
    void Start()
    {
        if(Character.Instance.HasMap == true)
        {
            CloseButton.SetActive(true);
            HiddenObjectPuzzleNpc.SetActive(true);
            ClothingPuzzleNpc.SetActive(true);
        }
        if(Character.Instance.TutorialCompleted == true)
        {
            EscapeGameNpc.SetActive(true);
        }
    }

    public void LoadObjectsFromSequence()
    {
        for (int i = 0; i < SequencesOfTutorial.Count; i++)
        {
            if (_currentIndexOfSequence == i)
            {
                foreach (GameObject obj in SequencesOfTutorial[_currentIndexOfSequence].GameObjectsInSequence)
                {
                    obj.SetActive(true);
                }
            }
        }

        _currentIndexOfSequence++;
    }
}
