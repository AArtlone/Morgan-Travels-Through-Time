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

    private CameraBehavior _cameraBehaviour;
    private FadeScreenController _fadeScreenController;
    public List<Sequence> SequencesOfTutorial = new List<Sequence>();
    private int _currentIndexOfSequence;
    private Vector3 _newCameraPosition;
    
    void Start()
    {
        _cameraBehaviour = FindObjectOfType<CameraBehavior>();
        _fadeScreenController = FindObjectOfType<FadeScreenController>();

        if (Character.Instance.HasMap == true)
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

    /// <summary>
    /// Once an objective/quest is completed, new sequences of entities
    /// will be activated with this function using the SequencesOfTutorial list
    /// in this script.
    /// </summary>
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

    /// <summary>
    /// Once the player taps on a gate way to a new area, the player will be taken
    /// to that area that is assigned to the function parameter.
    /// </summary>
    /// <param name="newArea"></param>
    public void EnterAreaPart(GameObject newArea)
    {
        Vector3 newAreaPosition = newArea.transform.position;

        // Calculates the new bounds our camera has to stay between in order to
        // not escape the area background boundaries.
        _cameraBehaviour.Background = newArea;
        _cameraBehaviour.BackgroundBounds = newArea.GetComponent<SpriteRenderer>().bounds;

        // + 6.4 because the area is located at x position and we want the camera
        // to position itself centered at the middle of the area instead of its
        // left corner anchor since that will reveal the previous area or empty
        // space next to it, so we move it half the resolution's width forward.
        _newCameraPosition = new Vector3(newAreaPosition.x + 6.4f, newAreaPosition.y, -10);

        // FADE EFFECT initiates
        _fadeScreenController.StartTransition();
        _fadeScreenController.FadeOutCamera();

        Invoke("MoveCameraToNewPosition", 1f);
    }

    /// <summary>
    /// Moves the camera to the new area part and fades it in after that.
    /// </summary>
    private void MoveCameraToNewPosition()
    {
        _cameraBehaviour.transform.position = _newCameraPosition;
        _fadeScreenController.FadeInCamera();
        Invoke("ResetCameraScreen", 1f);
    }

    /// <summary>
    /// Completes the fade effect by stopping further animations.
    /// </summary>
    private void ResetCameraScreen()
    {
        _fadeScreenController.EndTransition();
    }
}
