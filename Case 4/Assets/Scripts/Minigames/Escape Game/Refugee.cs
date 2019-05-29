using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class Refugee : MonoBehaviour
{
    private Escape _gameInterface;

    private int _currentCheckpointIndex = 0;
    private Checkpoint _targetCheckpoint;
    public enum RefugeeStatus { Walking, Wondering, Idle };
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

    void Start()
    {
        _animator = GetComponent<Animator>();
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
            ChangeRefugeeStatus(RefugeeStatus.Walking);
        }

        if (Status == RefugeeStatus.Wondering)
        {
            Wondering();
        } else if (Status == RefugeeStatus.Walking)
        {
            MoveTowardsCheckpointQueueElement();
        }
    }

    private void ChangeRefugeeStatus(RefugeeStatus NewRefugeeStatus)
    {
        if (NewRefugeeStatus == RefugeeStatus.Idle)
        {
            _animator.SetBool("IsWalking", false);
            Status = NewRefugeeStatus;
            StartCoroutine(ChangeRefugeeStatusCO(RefugeeStatus.Wondering));
        } else if (NewRefugeeStatus == RefugeeStatus.Wondering)
        {
            _animator.SetBool("IsWalking", true);
            Status = NewRefugeeStatus;
            StartCoroutine(ChangeRefugeeStatusCO(RefugeeStatus.Idle));
        } else if (NewRefugeeStatus == RefugeeStatus.Walking)
        {
            FlipNPC("Right");
            _animator.SetBool("IsWalking", true);
            StopAllCoroutines();
            Status = NewRefugeeStatus;
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
            transform.position = Vector2.MoveTowards(transform.position, _targetCheckpoint.FirstQueueElement.transform.position, Speed * .5f * Time.deltaTime);
        } else
        {
            if (_targetCheckpoint.tag == "Final Checkpoint")
            {
                _gameInterface.RefugeesSaved++;
                _gameInterface.RefugeesSavedInThisSession++;
                _gameInterface.TotalPoints += RewardInPoints;
                _gameInterface.CurrentRefugees[WaveIndex].Remove(this);

                //for (int i = 0; i < _gameInterface.CurrentRefugees.Count; i++)
                //{
                //    _gameInterface.CurrentRefugees[WaveIndex][i].RefugeeIndex = i;
                //}

                //if (_gameInterface.CurrentRefugees.Count <= 0 && _gameInterface.CurrentWave <= _gameInterface.RefugeeWaves.Count - 1)
                //{
                //    _gameInterface.TotalPoints += _gameInterface.RefugeeWaves[_gameInterface.CurrentWave].RewardInPoints;

                //    _gameInterface.CurrentWave++;
                //    _gameInterface.StartNextWave();
                //}

                if (_gameInterface.CurrentWave == _gameInterface.RefugeeWaves.Count)
                {
                    _gameInterface.Completed = true;
                    _gameInterface.EndGame();
                }

                _gameInterface.SaveEscapeGamesData();
                Destroy(gameObject);
            } else if (_targetCheckpoint.tag == "Inbetween Checkpoint")
            {
                if (RefugeeIndex == 0)
                {
                    if (_gameInterface.CurrentWave < _gameInterface.RefugeeWaves.Count - 1)
                    {
                        _gameInterface.CurrentWave++;
                        _gameInterface.StartNextWave();
                    }
                }
            }
            ChangeRefugeeStatus(RefugeeStatus.Wondering);
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
    private void FlipNPC(string Side)
    {
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
    }
}
