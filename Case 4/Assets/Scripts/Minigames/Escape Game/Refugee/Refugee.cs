using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using Anima2D;
using System.Collections.Generic;

public class Refugee : MonoBehaviour
{
    private Escape _gameInterface;

    private int _currentCheckpointIndex = 0;
    public Checkpoint _targetCheckpoint;
    public RefugeeStatus PreviousStatus;
    public enum RefugeeStatus { Walking, Wondering, Idle, Injured, WalkingToObsIntElement, AtObsIntElement, CarryingInjured, WalkingWithStretcher };
    public RefugeeStatus Status;
    private Animator _animator;
    public int RewardInPoints;
    public int RefugeeIndex;
    [NonSerialized]
    public int WaveIndex;
    public bool WonderingLeft;
    public bool WonderingRight;
    public bool FacingLeft;
    public bool FacingRight;

    public int Speed;
    public int WonderingSpeed;

    private GameObject _obstIntElement;
    private Vector3 _posToMoveTo; //used by injured refugee
    private List<GameObject> _carryingRefugees = new List<GameObject>();
    public static GameObject FirstCarryingRefugee;
    public static GameObject SecondCarryingRefugee;
    public bool HasBeenCarrying = false;
    public bool HasBeenInjured = false;
    //public static GameObject Stretcher;
    private float _initialYPos;
    //private KeyValuePair<float, GameObject> _carryingRefugees = new KeyValuePair<float, GameObject>;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _gameInterface = FindObjectOfType<Escape>();

        _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
        WonderingRight = true;
        FacingRight = true;
        _animator.SetBool("IsWalking", true);
    }
    
    void Update()
    {
        if(_currentCheckpointIndex < _gameInterface.Checkpoints.Count - 1 &&
            _gameInterface.Checkpoints[_currentCheckpointIndex + 1].Passable == true)
        {
            _currentCheckpointIndex++;
            _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
            _posToMoveTo = new Vector3(_targetCheckpoint.FirstQueueElement.transform.position.x, transform.position.y, _targetCheckpoint.FirstQueueElement.transform.position.z);
            /*if (Status != RefugeeStatus.CarryingInjured)
            {
                ChangeRefugeeStatus(RefugeeStatus.Walking);
            }*/
            ChangeRefugeeStatus(RefugeeStatus.Walking);
        }

        if (Status == RefugeeStatus.Wondering)
        {
            Wondering();
        } else if (Status == RefugeeStatus.Walking)
        {
            if (_gameInterface.RefugeesCanProceed)
                MoveTowardsCheckpointQueueElement();
        } else if (Status == RefugeeStatus.WalkingToObsIntElement)
        {
            MoveTowardsObsIntElement();
        }
    }

    private void ChangeRefugeeStatus(RefugeeStatus NewRefugeeStatus)
    {
        PreviousStatus = Status;

        switch (NewRefugeeStatus)
        {
            case RefugeeStatus.Idle:
                _animator.SetBool("IsWalking", false);
                StartCoroutine(ChangeRefugeeStatusCO(RefugeeStatus.Wondering));
                break;
            case RefugeeStatus.Wondering:
                if (Status != RefugeeStatus.Injured || HasBeenInjured == false)
                    _animator.SetBool("IsWalking", true);
                StartCoroutine(ChangeRefugeeStatusCO(RefugeeStatus.Idle));
                break;
            case RefugeeStatus.Walking:
                if (Status != RefugeeStatus.Injured || Status != RefugeeStatus.CarryingInjured)
                {
                    FlipNPC("Right");
                }
                if (Status != RefugeeStatus.Injured || HasBeenInjured == false)
                    _animator.SetBool("IsWalking", true);
                StopAllCoroutines();
                break;
            case RefugeeStatus.Injured:
                //_animator.SetBool("IsInjured", true);
                _animator.SetBool("IsWalking", false);
                StopAllCoroutines();
                break;
            case RefugeeStatus.WalkingToObsIntElement:
                if (Status != RefugeeStatus.Injured || HasBeenInjured == false)
                    FlipNPC("Right");
                _animator.SetBool("IsWalking", true);
                StopAllCoroutines();
                break;
            case RefugeeStatus.AtObsIntElement:
                _animator.SetBool("IsWalking", false);
                StopAllCoroutines();
                break;
            case RefugeeStatus.CarryingInjured:
                _animator.SetBool("IsWalking", false);
                //FlipNPC("Right");
                StopAllCoroutines();
                break;
        }
        Status = NewRefugeeStatus; 
    }

    private IEnumerator ChangeRefugeeStatusCO (RefugeeStatus NewRefugeeStatus)
    {
        int secondsToWait = Random.Range(1, 4);
        yield return new WaitForSeconds(secondsToWait);
        ChangeRefugeeStatus(NewRefugeeStatus);
    }

    /// <summary>
    /// Moves the NPC towards it's Queue Element of the targeted checkpoint
    /// </summary>
    private void MoveTowardsCheckpointQueueElement()
    {
        if (Vector2.Distance(transform.position, _targetCheckpoint.FirstQueueElement.transform.position) > 2f)
        {
            /*if (FirstCarryingRefugee != null)
                FirstCarryingRefugee.GetComponentInChildren<Animator>().SetBool("IsWalking", true);
            if (SecondCarryingRefugee != null)
                SecondCarryingRefugee.GetComponentInChildren<Animator>().SetBool("IsWalking", true);*/
            if (HasBeenCarrying == false)
            {
                transform.position = Vector2.MoveTowards(transform.position, _posToMoveTo, Speed * .5f * Time.deltaTime);
            }
        } else
        {
            if (_targetCheckpoint.tag == "Final Checkpoint")
            {   
                _gameInterface.RefugeesSaved++;
                _gameInterface.RefugeesSavedInThisSession++;
                _gameInterface.TotalPoints += RewardInPoints;
                _gameInterface.CurrentRefugees[WaveIndex].Remove(this);

                if (_gameInterface.CurrentRefugees[WaveIndex].Count <= 0 && _gameInterface.CurrentWave <= _gameInterface.RefugeeWaves.Count - 1)
                {
                    _gameInterface.TotalPoints += _gameInterface.RefugeeWaves[_gameInterface.CurrentWave].RewardInPoints;

                    _gameInterface.CurrentWave++;
                    if (_gameInterface.CurrentWave != _gameInterface.RefugeeWaves.Count)
                    {
                        _gameInterface.StartNextWave();
                    }
                }

                if (_gameInterface.CurrentWave == _gameInterface.RefugeeWaves.Count)
                {
                    _gameInterface.Completed = true;
                    _gameInterface.EndGame();
                }

                _gameInterface.SaveEscapeGamesData();
                Destroy(gameObject);
            }
            if (HasBeenInjured)
            {
                if (FirstCarryingRefugee != null)
                    FirstCarryingRefugee.GetComponent<Refugee>().ChangeRefugeeStatus(RefugeeStatus.CarryingInjured);
                if (SecondCarryingRefugee != null)
                    SecondCarryingRefugee.GetComponent<Refugee>().ChangeRefugeeStatus(RefugeeStatus.CarryingInjured);
                ChangeRefugeeStatus(RefugeeStatus.Injured);
            }
            else
            {
                if (!HasBeenCarrying)
                {
                    ChangeRefugeeStatus(RefugeeStatus.Wondering);
                }
            }
        }
    }

    /*private void MoveTowardsCheckpointQueueElementWithStretcher()
    {
        if (Vector2.Distance(transform.position, _targetCheckpoint.FirstQueueElement.transform.position) > 2f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _posToMoveTo, Speed * .5f * Time.deltaTime);
        } else
        {
            ChangeRefugeeStatus(RefugeeStatus.CarryingInjured);
        }
    }*/

    private void MoveTowardsObsIntElement()
    {
        if (Vector2.Distance(transform.position, _obstIntElement.transform.position) > .1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _obstIntElement.transform.position, Speed * .5f * Time.deltaTime);
        } else
        {
            ChangeRefugeeStatus(RefugeeStatus.AtObsIntElement);
            _obstIntElement.GetComponent<ObstacleIntercationElement>().RefugeeReachedObsIntElement();
        }
    }

    /// <summary>
    /// NPC's movement while it is standing at the checkpoint in fron of the obstacle
    /// </summary>
    private void Wondering()
    {
        GameObject firstQueueElement = _targetCheckpoint.FirstQueueElement;
        GameObject lastQueueElement = _targetCheckpoint.LastQueueElement;
        if (WonderingRight)
        {
            if (FacingLeft)
            {
                FlipNPC("Right");
            }
            transform.position = Vector2.MoveTowards(transform.position, firstQueueElement.transform.position, WonderingSpeed * .5f * Time.deltaTime);
        } else if (WonderingLeft)
        {
            if (FacingRight)
            {
                FlipNPC("Left");
            }
            transform.position = Vector2.MoveTowards(transform.position, lastQueueElement.transform.position, WonderingSpeed * .5f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Flips the NPC by changing it's scale on the X axis
    /// </summary>
    /// <param name="Side"></param>
    public void FlipNPC(string Side)
    {
        /*if (Status == RefugeeStatus.Injured)
        {
            return;
        }*/
        if (Side == "Left")
        {
            FacingLeft = true;
            FacingRight = false;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        } else if (Side == "Right")
        {
            FacingRight = true;
            FacingLeft = false;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void InjureRefugee()
    {
        _initialYPos = transform.position.y;
        ChangeRefugeeStatus(RefugeeStatus.Injured);
        HasBeenInjured = true;
        _animator.SetBool("IsInjured", true);
        FlipNPC("Right");
        /*if (FacingLeft)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 90f);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 90f);
        }*/
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 90f);
        foreach (SpriteMeshInstance component in transform.GetChild(0).GetComponentsInChildren<SpriteMeshInstance>())
        {
            component.sortingOrder = component.sortingOrder + _gameInterface.GetLayerMultiplier();
        }
    }

    public void CureRefugee()
    {
        transform.GetChild(0).transform.position = new Vector3(transform.position.x, transform.position.y + .6f, transform.position.z);
        Vector3 posForStretcher = new Vector3(transform.position.x, _initialYPos - .2f, transform.position.z);
        GameObject stretcher = Instantiate(_gameInterface.StretcherPrefab, posForStretcher, Quaternion.identity);
        _gameInterface.Stretcher = stretcher;
        stretcher.transform.parent = transform;
        stretcher.transform.position = posForStretcher;
        _gameInterface.FronPosAtStrether = _gameInterface.Stretcher.transform.GetChild(0).gameObject;
        _gameInterface.BackPosAtStrether = _gameInterface.Stretcher.transform.GetChild(1).gameObject;
        for (int i = 0; i < _gameInterface.CurrentRefugees[WaveIndex].Count; i++)
        {
            if (_gameInterface.CurrentRefugees[WaveIndex][i] != this)
            {
                if (_carryingRefugees.Count < 2)
                {
                    if (_carryingRefugees.Count == 0)
                    {
                        GameObject carryingRefugee = _gameInterface.CurrentRefugees[WaveIndex][i].gameObject;
                        _carryingRefugees.Add(carryingRefugee);
                        FirstCarryingRefugee = carryingRefugee;
                        FirstCarryingRefugee.transform.parent = transform;
                        if (FirstCarryingRefugee.GetComponent<Refugee>().FacingLeft)
                        {
                            FirstCarryingRefugee.GetComponent<Refugee>().FlipNPC("Right");
                        }
                        
                    } else if (_carryingRefugees.Count == 1)
                    {
                        GameObject carryingRefugee = _gameInterface.CurrentRefugees[WaveIndex][i].gameObject;
                        _carryingRefugees.Add(carryingRefugee);
                        SecondCarryingRefugee = carryingRefugee;
                        SecondCarryingRefugee.transform.parent = transform;
                        if (SecondCarryingRefugee.GetComponent<Refugee>().FacingLeft)
                        {
                            SecondCarryingRefugee.GetComponent<Refugee>().FlipNPC("Right");
                        }
                    }
                } else
                {
                    break;
                }
            }
        }
        FirstCarryingRefugee.transform.position = _gameInterface.BackPosAtStrether.transform.position;
        SecondCarryingRefugee.transform.position = _gameInterface.FronPosAtStrether.transform.position;
        FirstCarryingRefugee.GetComponent<Refugee>().ChangeRefugeeStatus(RefugeeStatus.CarryingInjured);
        SecondCarryingRefugee.GetComponent<Refugee>().ChangeRefugeeStatus(RefugeeStatus.CarryingInjured);
        FirstCarryingRefugee.GetComponent<Refugee>().HasBeenCarrying = true;
        SecondCarryingRefugee.GetComponent<Refugee>().HasBeenCarrying = true;

        foreach (Refugee refugee in _gameInterface.CurrentRefugees[WaveIndex])
        {
            if (refugee != this)
            {
                Physics2D.IgnoreCollision(refugee.GetComponent<CapsuleCollider2D>(), _gameInterface.Stretcher.GetComponent<BoxCollider2D>());
            }
        }
        /*if (FacingLeft)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 180f);
        }*/
        _gameInterface.CurrentRefugees[WaveIndex].Remove(FirstCarryingRefugee.GetComponent<Refugee>());
        _gameInterface.CurrentRefugees[WaveIndex].Remove(SecondCarryingRefugee.GetComponent<Refugee>());
        _gameInterface._cannonInterface.GetComponentInParent<Obstacle>().PlayCanon();
    }

    public void AssignRefugeeToObsIntElement(GameObject obj)
    {
        ChangeRefugeeStatus(RefugeeStatus.WalkingToObsIntElement);
        _obstIntElement = obj;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Water")
        {
            _gameInterface.CurrentRefugees[WaveIndex].Remove(this);
            _gameInterface.EndGameLose();
            Destroy(gameObject);
        }
        if(collision.transform.tag == "First Queue Element")
        {
            WonderingRight = false;
            WonderingLeft= true;
        }
        if (collision.transform.tag == "Last Queue Element")
        {
            WonderingLeft = false;
            WonderingRight = true;
        }
        if (collision.transform.tag == "Initial Checkpoint")
        {
            if (this ==  _gameInterface.CurrentRefugees[_gameInterface.CurrentWave][_gameInterface.CurrentRefugees[_gameInterface.CurrentWave].Count - 1])
            {
                FindObjectOfType<CannonBallShooting>().Shoot();
            }
        }
    }
}
