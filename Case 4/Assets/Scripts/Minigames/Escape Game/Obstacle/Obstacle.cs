using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType { Fire, EvilPlant, Flag, Bridge, Sheeps, Tree, Cannon };
    public ObstacleType Type;
    public Checkpoint CheckpointLink;
    public LayerMask ObstacleLayer;
    private float _timeHoldingDown = 1;
    private float _currentTimeHoldingDown = 0;
    private bool _holdingDownObstacle;
    private Escape _gameInterface;

    public GameObject Bridge;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _gameInterface = FindObjectOfType<Escape>();
    }

    private void Update()
    {
        if (gameObject.name == "Bridge Lever")
        {
            PlayBridge();
        }
        /*else if (gameObject.name == "Tree Obstacle")
        {
            PlayTree();
        }*/
    }

    public void ToggleObstacle()
    {
        CheckpointLink.Passable = !CheckpointLink.Passable;
    }

    public void PlayBridge()
    {
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
                        Vector3 newBridgePosition = Bridge.transform.localScale;
                        newBridgePosition.x += Time.deltaTime * 0.3f;
                        Bridge.transform.localScale = newBridgePosition;
                    }
                }

                if (_currentTimeHoldingDown >= _timeHoldingDown)
                {
                    ToggleObstacle();
                }
            }
        }
        else
        {
            _holdingDownObstacle = false;
        }

        if (_currentTimeHoldingDown > 0 && _holdingDownObstacle == false)
        {
            Vector3 newBridgePosition = Bridge.transform.localScale;
            newBridgePosition.x -= Time.deltaTime * 0.3f;
            Bridge.transform.localScale = newBridgePosition;

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

    public void PlayTree()
    {
        ToggleObstacle();
        gameObject.SetActive(false);
        _gameInterface.RespawnObstacle(gameObject);
        /*if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 origin = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D hitObj = Physics2D.Raycast(origin, Vector3.forward, Mathf.Infinity, ObstacleLayer);
            
            if (hitObj.transform != null)
            {
                if (hitObj.transform.name == gameObject.name)
                {
                    ToggleObstacle();
                    gameObject.SetActive(false);
                    _gameInterface.RespawnObstacle(gameObject);
                }
            }
        }*/
    }

    public void PLayFire()
    {
        ToggleObstacle();
        gameObject.SetActive(false);
        _gameInterface.RespawnObstacle(gameObject);
    }

    public void PlayFlag()
    {
        ToggleObstacle();
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Items/Inventory/Groningen Flag");
        _gameInterface.RespawnFlagObstacle(gameObject);
        Invoke("ToggleObstacle", 5f);
    }

    public void PlayCanon()
    {
        ToggleObstacle();
    }

    public void CutPlant()
    {
        // TODO: Play cutting sound
        ToggleObstacle();
        gameObject.SetActive(false);
        _gameInterface.RespawnObstacle(gameObject);
        Invoke("ToggleObstacle", 5f);
    }

    public void SetPlantOnFire()
    {
        // TODO: Play burning sound
        _gameInterface.Fire.SetActive(true);
    }

    public void ExtinguishPlant()
    {
        // TODO: Play extinguish sound
        ToggleObstacle();
        gameObject.SetActive(false);
        _gameInterface.Fire.SetActive(false);
        _gameInterface.RespawnObstacle(gameObject);
        Invoke("ToggleObstacle", 5f);
    }

    public void PlaySheeps(Item.ItemType type)
    {
        if (type == Item.ItemType.BommenBerendFlag)
        {
            // TODO: Do some anymation, i guess
            Debug.Log("Tried to distract sheeps");
        } else if (type == Item.ItemType.BunchOfHay)
        {
            // TODO: Do something wiht aninmations
            ToggleObstacle();
            Invoke("ToggleObstacle", 5f);
        }
    }
}
