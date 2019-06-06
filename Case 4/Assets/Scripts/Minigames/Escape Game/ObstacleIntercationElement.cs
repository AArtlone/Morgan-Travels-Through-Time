using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleIntercationElement : MonoBehaviour
{
    public Obstacle ObstacleLink;
    public bool IsOccupied = false;
    private bool _isAssigned = false;
    private Escape _gameInterface;
    private Refugee _refuggeToAssign;
    private List<KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement>> _obsIntElementsOfThisType = new List<KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement>>();

    private void Start()
    {
        _gameInterface = FindObjectOfType<Escape>();
        _gameInterface.AllObsIntElements.Add(new KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement>(ObstacleLink.Type, this));

        foreach (KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement> pair in _gameInterface.AllObsIntElements)
        {
            if (pair.Value == this)
            {
                _obsIntElementsOfThisType.Add(pair);
            }
        }
    }

    public void AssignRefugee()
    {
        if (!_isAssigned)
        {
            ChooseRefugee().AssignRefugeeToObsIntElement(gameObject);
            _isAssigned = true;
        }
    }

    public void RefugeeReachedObsIntElement()
    {
        if (IsOccupied == false)
        {
            IsOccupied = true;
        }

        bool areAllOccupied = false;
        foreach (KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement> pair in _obsIntElementsOfThisType)
        {
            if (pair.Value.IsOccupied == true)
            {
                areAllOccupied = true;
            }
        }

        if (areAllOccupied == true)
        {
            ObstacleLink.PlayTree();
        }
    }

    private Refugee ChooseRefugee()
    {
        foreach (Refugee refugee in _gameInterface.CurrentRefugees[_gameInterface.CurrentWave])
        {
            if (refugee.Status != Refugee.RefugeeStatus.Injured && refugee.Status != Refugee.RefugeeStatus.AtObsIntElement && refugee.Status != Refugee.RefugeeStatus.WalkingToObsIntElement)
            {
                _refuggeToAssign = refugee;
                break;
            }
        }
        return _refuggeToAssign;
    }
}
