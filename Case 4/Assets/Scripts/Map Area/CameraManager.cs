using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public List<GameObject> Cameras;
    private int _currentCameraIndex;

    #region Swiping mechanic variables
    private enum DraggedDirection { Up, Down, Left, Right };
    private Vector3 _startTapPosition;
    private Vector3 _tapPositionStartWorld;
    private Vector3 _endTapPosition;
    private Vector3 _directionOfSwipeNormalized;
    private float _swipeLength;
    private float _swipeLengthWorld;
    #endregion

    private Camera _camera;
    private CameraBehavior _cameraBehaviour;
    private MapEnvironmentManager _mapEnvironmentManager;
    private string _direction;

    private void Start()
    {
        _cameraBehaviour = FindObjectOfType<CameraBehavior>();
        _mapEnvironmentManager = FindObjectOfType<MapEnvironmentManager>();
        _camera = _cameraBehaviour.GetComponent<Camera>();
    }

    private void Update()
    {
        // The camera should only disable swiping in between dialogue in scenes that
        // *should* contain a dialogue and a dialogue manager!
        string currentScene = SceneManager.GetActiveScene().name;
        if ((currentScene != "Escape Game" || currentScene != "Hidden Objects Puzzle") &&
            DialogueManager.Instance != null)
        {
            if (Input.touchCount > 0 &&
                DialogueManager.Instance.DialogueTemplate.activeSelf == false &&
                _cameraBehaviour.IsUIOpen == false &&
                _cameraBehaviour.IsInteracting == false)
            {
                GetSwipe();
            }
        }
        else
        {
            // A scene which allows for camera movement that is not a map area, such as the
            // escape game will require the mechanic of swiping from the camera behaviour.
            // Other puzzles such as the HOP do not require swiping, therefore we first
            // confirm if that behaviour exists in the HOP or not before using it.
            if (_cameraBehaviour != null &&
                _cameraBehaviour.IsInterfaceElementSelected == false &&
                Input.touchCount > 0)
            {
                GetSwipe();
            }
        }
    }

    /// <summary>
    /// This function returns the properties of the swipe the player made.
    /// </summary>
    private void GetSwipe()
    {
        //Debug.Log(_direction);
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _direction = string.Empty;
                _startTapPosition = touch.position;
                _endTapPosition = touch.position;
                _tapPositionStartWorld = Camera.main.ScreenToWorldPoint(_startTapPosition);
            }

            if (touch.phase == TouchPhase.Moved)
            {
                _endTapPosition = touch.position;
                _directionOfSwipeNormalized = (_endTapPosition - _startTapPosition).normalized;

                Vector3 tapPositionEndWorld = Camera.main.ScreenToWorldPoint(_endTapPosition);

                _swipeLength = _endTapPosition.x - _startTapPosition.x;
                _swipeLengthWorld = Mathf.Abs(tapPositionEndWorld.x - _tapPositionStartWorld.x);

                Vector3 newCameraPosition = _mapEnvironmentManager.CurrentCamera.transform.position;
                if (SceneManager.GetActiveScene().name == "Escape Game")
                {
                    if (_directionOfSwipeNormalized.x > 0)
                    {
                        newCameraPosition.x -= _swipeLengthWorld * 0.15f;
                        _mapEnvironmentManager.CurrentCamera.transform.position = new Vector3(
                            Mathf.Clamp(newCameraPosition.x, _cameraBehaviour.BackgroundBoundsLeft.min.x + _camera.orthographicSize + 4, _cameraBehaviour.BackgroundBoundsRight.max.x - _camera.orthographicSize - 4),
                            newCameraPosition.y,
                            newCameraPosition.z);
                    }
                    else if (_directionOfSwipeNormalized.x < 0)
                    {
                        newCameraPosition.x += _swipeLengthWorld * 0.15f;
                        _mapEnvironmentManager.CurrentCamera.transform.position = new Vector3(
                            Mathf.Clamp(newCameraPosition.x, _cameraBehaviour.BackgroundBoundsLeft.min.x + _camera.orthographicSize + 4, _cameraBehaviour.BackgroundBoundsRight.max.x - _camera.orthographicSize - 4),
                            newCameraPosition.y,
                            newCameraPosition.z);
                    }
                } else
                {
                    if (_directionOfSwipeNormalized.x > 0 && _direction == string.Empty)
                    {
                        _direction = "Right";
                        newCameraPosition.x -= _swipeLengthWorld * 0.15f;
                        _mapEnvironmentManager.CurrentCamera.transform.position = new Vector3(
                            Mathf.Clamp(newCameraPosition.x, _cameraBehaviour.BackgroundBounds.min.x + _camera.orthographicSize + 4, _cameraBehaviour.BackgroundBounds.max.x - _camera.orthographicSize - 4),
                            newCameraPosition.y,
                            newCameraPosition.z);
                    }
                    else if (_directionOfSwipeNormalized.x < 0 && _direction == string.Empty)
                    {
                        _direction = "Left";
                        newCameraPosition.x += _swipeLengthWorld * 0.15f;
                        _mapEnvironmentManager.CurrentCamera.transform.position = new Vector3(
                            Mathf.Clamp(newCameraPosition.x, _cameraBehaviour.BackgroundBounds.min.x + _camera.orthographicSize + 4, _cameraBehaviour.BackgroundBounds.max.x - _camera.orthographicSize - 4),
                            newCameraPosition.y,
                            newCameraPosition.z);
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _direction = string.Empty;
                _endTapPosition = touch.position;
            }
        }
    }

    /// <summary>
    /// This function returns the direction of the player's swiping on the screen
    /// and uses it to direct the player to a new map area part.
    /// </summary>
    private void CheckDirection()
    {
        if (_directionOfSwipeNormalized.x > 0)
        {
            if (_swipeLength >= 200)
            {
                _currentCameraIndex--;
                if (_currentCameraIndex < 0)
                {
                    _currentCameraIndex = 0;
                }

                for (int i = 0; i < Cameras.Count; i++)
                {
                    if (i == _currentCameraIndex)
                    {
                        Cameras[i].SetActive(true);
                        if (_mapEnvironmentManager != null)
                        {
                            _mapEnvironmentManager.CurrentCamera = Cameras[i].gameObject;
                        }
                    }
                    else
                    {
                        Cameras[i].SetActive(false);
                    }
                }
            }
        }
        else if (_directionOfSwipeNormalized.x < 0)
        {
            if (_swipeLength <= -200)
            {
                _currentCameraIndex++;
                if (_currentCameraIndex > Cameras.Count - 1)
                {
                    _currentCameraIndex = Cameras.Count - 1;
                }

                for (int i = 0; i < Cameras.Count; i++)
                {
                    if (i == _currentCameraIndex)
                    {
                        Cameras[i].SetActive(true);
                        if (_mapEnvironmentManager != null)
                        {
                            _mapEnvironmentManager.CurrentCamera = Cameras[i].gameObject;
                        }
                    }
                    else
                    {
                        Cameras[i].SetActive(false);
                    }
                }
            }
        }
    }
}
