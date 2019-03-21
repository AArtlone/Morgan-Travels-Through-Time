using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEnvironmentManager : MonoBehaviour
{
    private GameObject _hiddenObjectPuzzleNpc;
    public GameObject ClothingPuzzleNpc;
    private GameObject _dialogueNpc;
    public GameObject CloseButton;
    private GameObject _playerIcon;



    // Start is called before the first frame update
    void Start()
    {
        _hiddenObjectPuzzleNpc = GameObject.Find("2D Hidden Objects Puzzle NPC");
        _dialogueNpc = GameObject.Find("2D Area NPC");
        _playerIcon = GameObject.Find("Player Icon");
        if(Character.Instance.HasMap == true)
        {
            CloseButton.SetActive(true);
            ClothingPuzzleNpc.SetActive(true);
        }
    }

    private void EnablePuzzleNpc(GameObject gameObjectName)
    {
        gameObjectName.gameObject.SetActive(true);
    }

    public void ReceivedMapEvents()
    {
        CloseButton.SetActive(true);
        ClothingPuzzleNpc.SetActive(true);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
