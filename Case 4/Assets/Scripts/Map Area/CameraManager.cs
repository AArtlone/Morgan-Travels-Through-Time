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
    private string _swipeDirection;
    private float _swipeLength;
    private float _swipeLengthWorld;
    #endregion

    private Camera _camera;
    private CameraBehavior _cameraBehaviour;
    private MapEnvironmentManager _mapEnvironmentManager;
    //private int _currentAreaPart;
    //private float _maxBound;
    //private float _minBound;

    private void Start()
    {
        _cameraBehaviour = FindObjectOfType<CameraBehavior>();
        _mapEnvironmentManager = FindObjectOfType<MapEnvironmentManager>();
        _camera = _cameraBehaviour.GetComponent<Camera>();
        //_currentAreaPart = 0;
        //_minBound = _mapEnvironmentManager.MapParts[0].GetComponent<SpriteRenderer>().bounds.min.x;
    }

    private void Update()
    {
        //Debug.Log(DialogueManager.Instance.DialogueTemplate.activeSelf + " " + _cameraBehaviour.IsUIOpen + " " + _cameraBehaviour.IsInteracting);
        if (SceneManager.GetActiveScene().name != "Escape Game")
        {
            if (Input.touchCount > 0 && DialogueManager.Instance.DialogueTemplate.activeSelf == false && _cameraBehaviour.IsUIOpen == false && _cameraBehaviour.IsInteracting == false)
            {
                GetSwipe();
            }
        } else
        {
            if (_cameraBehaviour.IsInterfaceElementSelected == false  && Input.touchCount > 0)
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
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
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
                if (_directionOfSwipeNormalized.x > 0)
                {
                    newCameraPosition.x -= _swipeLengthWorld * 0.15f;
                    _mapEnvironmentManager.CurrentCamera.transform.position = new Vector3(
                        Mathf.Clamp(newCameraPosition.x, _cameraBehaviour.BackgroundBounds.min.x + _camera.orthographicSize + 4, _cameraBehaviour.BackgroundBounds.max.x - _camera.orthographicSize - 4),
                        newCameraPosition.y,
                        newCameraPosition.z);

                    //if (newCameraPosition.x <= _cameraBehaviour.BackgroundBounds.min.x + 5 &&
                    //    _mapEnvironmentManager.MapParts[_currentAreaPart - 1].gameObject != null)
                    //{
                    //    if (_mapEnvironmentManager.EnterAreaPart(_mapEnvironmentManager.MapParts[_currentAreaPart - 1].gameObject))
                    //    {
                    //        _currentAreaPart--;
                    //        _cameraBehaviour.BackgroundBounds = _mapEnvironmentManager.MapParts[_currentAreaPart].gameObject.GetComponent<SpriteRenderer>().bounds;
                    //        _maxBound = _cameraBehaviour.BackgroundBounds.max.x;
                    //        Debug.Log("Entered!");
                    //    }
                    //}
                }
                else if (_directionOfSwipeNormalized.x < 0)
                {
                    newCameraPosition.x += _swipeLengthWorld * 0.15f;
                    _mapEnvironmentManager.CurrentCamera.transform.position = new Vector3(
                        Mathf.Clamp(newCameraPosition.x, _cameraBehaviour.BackgroundBounds.min.x + _camera.orthographicSize + 4, _cameraBehaviour.BackgroundBounds.max.x - _camera.orthographicSize - 4),
                        newCameraPosition.y,
                        newCameraPosition.z);

                    //if (newCameraPosition.x >= _cameraBehaviour.BackgroundBounds.min.x + 5 &&
                    //    _mapEnvironmentManager.MapParts[_currentAreaPart + 1].gameObject != null)
                    //{
                    //    if (_mapEnvironmentManager.EnterAreaPart(_mapEnvironmentManager.MapParts[_currentAreaPart + 1].gameObject))
                    //    {
                    //        _currentAreaPart++;
                    //        _cameraBehaviour.BackgroundBounds = _mapEnvironmentManager.MapParts[_currentAreaPart].gameObject.GetComponent<SpriteRenderer>().bounds;
                    //        _maxBound = _cameraBehaviour.BackgroundBounds.max.x;
                    //        Debug.Log("Entered!");
                    //    }
                    //}
                }

                //Debug.Log(_tapPositionStartWorld + " | " + tapPositionEndWorld);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _endTapPosition = touch.position;
                //CheckDirection();
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
            _swipeDirection = "Right";

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
                    } else
                    {
                        Cameras[i].SetActive(false);
                    }
                }
            }
        }
        else if (_directionOfSwipeNormalized.x < 0)
        {
            _swipeDirection = "Left";

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
        //Debug.Log(_currentCameraIndex);
        //Debug.Log(_swipeDirection);
    }
}
