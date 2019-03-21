using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEnvironmentManager : MonoBehaviour
{
    private GameObject _hiddenObjectPuzzleNpc;
    private GameObject _clothingPuzzleNpc;
    private GameObject _dialogueNpc;
    private GameObject _closeButton;
    private GameObject _playerIcon;



    // Start is called before the first frame update
    void Start()
    {
        _hiddenObjectPuzzleNpc = GameObject.Find("2D Hidden Objects Puzzle NPC");
        _clothingPuzzleNpc = GameObject.Find("2D Guess Clothing Puzzle NPC");
        _dialogueNpc = GameObject.Find("2D Area NPC");
        _closeButton = GameObject.Find("Close Button");
        _playerIcon = GameObject.Find("Player Icon");
        if (Character.Instance.TutorialCompleted == false)
        {
            ChangeEnvironmentForTutorial();
        }
    }

    private void ChangeEnvironmentForTutorial()
    {
        _hiddenObjectPuzzleNpc.SetActive(false);
        _clothingPuzzleNpc.SetActive(false);
        _closeButton.SetActive(false);
    }

    private void EnablePuzzleNpc(GameObject gameObjectName)
    {
        gameObjectName.gameObject.SetActive(true);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
