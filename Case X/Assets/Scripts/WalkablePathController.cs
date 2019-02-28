using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WalkablePathController : MonoBehaviour
{
    private Animator _characterAnimator;
    private Vector3 DestinationPosition;
    [NonSerialized]
    public bool HasPlayerReachedDestination = true;

    private void Start()
    {
        _characterAnimator = Avatar.Instance.GetComponent<Animator>();
    }

    public void MovePlayer()
    {
        DestinationPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);

        _characterAnimator.SetBool("Idle", false);
        if (Camera.main.ScreenToViewportPoint(DestinationPosition).x >= 0.5f)
        {
            _characterAnimator.SetBool("Left", false);
            _characterAnimator.SetBool("Right", true);
        } else if (Camera.main.ScreenToViewportPoint(DestinationPosition).x < 0.5f)
        {
            _characterAnimator.SetBool("Right", false);
            _characterAnimator.SetBool("Left", true);
        }

        HasPlayerReachedDestination = false;
    }

    private void Update()
    {
        if (HasPlayerReachedDestination == false)
        {
            Avatar.Instance.transform.position = Vector3.MoveTowards(Avatar.Instance.transform.position, DestinationPosition, Avatar.Instance.MovementSpeed * 0.5f);

            if (Vector3.Distance(
                Avatar.Instance.transform.position,
                DestinationPosition) < 10f)
            {
                _characterAnimator.SetBool("Idle", true);
                HasPlayerReachedDestination = true;
            }
        }
    }
}
