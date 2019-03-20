using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Checkpoint CheckpointLink;
    public LayerMask ObstacleLayer;
    private float _timeHoldingDown = 10;
    private float _currentTimeHoldingDown = 0;
    private bool _holdingDownObstacle;

    private void Update()
    {
        //Debug.Log(_currentTimeHoldingDown);
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            
            if (Physics2D.Raycast(origin, Vector3.forward, Mathf.Infinity, ObstacleLayer))
            {
                _holdingDownObstacle = true;
                if (_currentTimeHoldingDown < _timeHoldingDown)
                {
                    _currentTimeHoldingDown += Time.deltaTime;
                } 

                //Debug.Log("Holding...");
                if (_currentTimeHoldingDown >= _timeHoldingDown)
                {
                    ToggleObstacle();
                }
            }
        } else
        {
            _holdingDownObstacle = false;
            CheckpointLink.Passable = false;
        }

        if (_currentTimeHoldingDown > 0 && _holdingDownObstacle == false)
        {
            //Debug.Log("Falling...");
            _currentTimeHoldingDown -= Time.deltaTime;
        }
    }

    public void ToggleObstacle()
    {
        if (CheckpointLink.Passable)
        {
            return;
        }
        //Debug.Log("Completed");
        CheckpointLink.Passable = !CheckpointLink.Passable;
    }
}
