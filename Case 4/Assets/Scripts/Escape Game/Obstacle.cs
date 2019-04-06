using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Checkpoint CheckpointLink;
    public LayerMask ObstacleLayer;
    private float _timeHoldingDown = 1;
    private float _currentTimeHoldingDown = 0;
    private bool _holdingDownObstacle;

    public GameObject Bridge;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //if(Input.GetKeyUp(KeyCode.A))
        //{
        //    CheckpointLink.Passable = true;
        //}
        //if (Input.GetKeyUp(KeyCode.S))
        //{
        //    DCheckpointLink.Passable = false;
        //}

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
                    _animator.enabled = true;

                    if (Bridge != null)
                    {
                        Vector3 newBridgePosition = Bridge.transform.position;
                        newBridgePosition.x -= Time.deltaTime * 5;
                        Bridge.transform.position = newBridgePosition;
                    }
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
            //CheckpointLink.Passable = false;
        }

        if (_currentTimeHoldingDown > 0 && _holdingDownObstacle == false)
        {
            //Debug.Log("Falling...");
            Vector3 newBridgePosition = Bridge.transform.position;
            newBridgePosition.x += Time.deltaTime * 5;
            Bridge.transform.position = newBridgePosition;

            _currentTimeHoldingDown -= Time.deltaTime;
            CheckpointLink.Passable = true;
            _animator.enabled = true;
        }

        if (_currentTimeHoldingDown <= 0.5f)
        {
            CheckpointLink.Passable = false;
            _animator.enabled = false;
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
