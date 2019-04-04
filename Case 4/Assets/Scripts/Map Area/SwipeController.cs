using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    #region Swiping mechanic variables
    private enum DraggedDirection { Up, Down, Left, Right };
    private Vector3 _startTapPosition;
    private Vector3 _endTapPosition;
    private Vector3 _directionOfSwipeNormalized;
    private string _swipeDirection;
    #endregion

    #region Area-swiping mechanic
    private MapEnvironmentManager _mapEnvironmentManager;
    public List<GameObject> AreaPartsList = new List<GameObject>();
    private int _currentAreaPartIndex = 0;
    #endregion

    private CameraBehavior _cameraBehaviour;

    private void Start()
    {
        _mapEnvironmentManager = FindObjectOfType<MapEnvironmentManager>();
        _cameraBehaviour = FindObjectOfType<CameraBehavior>();
    }

    public void ChangeArea()
    {
        if (_currentAreaPartIndex >= 0)
        {
            if (_swipeDirection == "Left")
            {
                if (_currentAreaPartIndex == 0)
                {
                    _currentAreaPartIndex = 0;   
                } else if (_currentAreaPartIndex > 0)
                {
                    if (_mapEnvironmentManager.EnterAreaPart(AreaPartsList[_currentAreaPartIndex - 1]))
                    {
                        _currentAreaPartIndex--;
                    }
                    
                }
            } else if (_swipeDirection == "Right")
            {
                if (_currentAreaPartIndex == AreaPartsList.Count - 1)
                {
                    _currentAreaPartIndex = AreaPartsList.Count - 1;
                } else
                {
                    if (_mapEnvironmentManager.EnterAreaPart(AreaPartsList[_currentAreaPartIndex + 1]))
                    {
                        _currentAreaPartIndex++;
                    }
                }
            }
        }
    }

    private void Update()
    {
        //Debug.Log(DialogueManager.Instance.DialogueTemplate.activeSelf + " " + _cameraBehaviour.IsUIOpen + " " + _cameraBehaviour.IsInteracting);
        if (Input.touchCount > 0 && DialogueManager.Instance.DialogueTemplate.activeSelf == false && _cameraBehaviour.IsUIOpen == false && _cameraBehaviour.IsInteracting == false)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    _startTapPosition = touch.position;
                    _endTapPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    _endTapPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    _endTapPosition = touch.position;
                    _directionOfSwipeNormalized = (_endTapPosition - _startTapPosition).normalized;

                    CheckDirection();
                }
            }
        }
    }

    private void CheckDirection()
    {
        if (_directionOfSwipeNormalized.x > 0)
        {
            _swipeDirection = "Right";
        } else if (_directionOfSwipeNormalized.x < 0)
        {
            _swipeDirection = "Left";
        }

        ChangeArea();
    }
}
