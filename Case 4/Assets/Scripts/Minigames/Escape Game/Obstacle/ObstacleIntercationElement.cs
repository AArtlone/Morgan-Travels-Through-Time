using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleIntercationElement : MonoBehaviour
{
    public Obstacle ObstacleLink;
    public bool IsOccupied = false;
    private bool _isAssigned = false;
    private static int _refugeesAtObsIntElement;
    private Escape _gameInterface;
    private Refugee _refuggeToAssign;
    private List<KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement>> _obsIntElementsOfThisType = new List<KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement>>();

    private void Start()
    {
        _gameInterface = FindObjectOfType<Escape>();
        _gameInterface.AllObsIntElements.Add(new KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement>(ObstacleLink.Type, this));

        foreach (KeyValuePair<Obstacle.ObstacleType, ObstacleIntercationElement> pair in _gameInterface.AllObsIntElements)
        {
            if (pair.Key == ObstacleLink.Type)
            {
                _obsIntElementsOfThisType.Add(pair);
            }
        }
    }

    public static void ResetValuesForNextWave()
    {
        _refugeesAtObsIntElement = 0;
    }

    public void AssignRefugee()
    {
        if (!_isAssigned)
        {
            if (ChooseRefugee() != null)
            {
                ChooseRefugee().AssignRefugeeToObsIntElement(gameObject);
                _isAssigned = true;
            }
        }
    }

    public void RefugeeReachedObsIntElement()
    {
        if (_refuggeToAssign.Status == Refugee.RefugeeStatus.AtObsIntElement)
        {
            _refugeesAtObsIntElement++;
        }

        if (_refugeesAtObsIntElement == 3)
        {
            ObstacleLink.PlayTree();
        }
    }

    private Refugee ChooseRefugee()
    {
        foreach (Refugee refugee in _gameInterface.CurrentRefugees[_gameInterface.CurrentWave])
        {
            if (refugee.Status != Refugee.RefugeeStatus.Injured && refugee.Status != Refugee.RefugeeStatus.AtObsIntElement && refugee.Status != Refugee.RefugeeStatus.WalkingToObsIntElement && refugee.Status != Refugee.RefugeeStatus.CarryingInjured && refugee.PreviousStatus != Refugee.RefugeeStatus.CarryingInjured && refugee.PreviousStatus != Refugee.RefugeeStatus.Injured &&
                refugee._targetCheckpoint.Index + 1 == ObstacleLink.CheckpointLink.Index)
            {
                _refuggeToAssign = refugee;
                break;
            }
        }
        return _refuggeToAssign;
    }
}
