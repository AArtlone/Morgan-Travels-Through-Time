using UnityEngine;
using System;

public class Refugee : MonoBehaviour
{
    private Escape _gameInterface;

    private int _currentCheckpointIndex = 0;
    private Checkpoint _targetCheckpoint;
    public enum RefugeeStatus { Active, Idle };
    public RefugeeStatus Status;
    private Animator _animator;
    private Rigidbody2D _rb;
    public int RewardInPoints;
    public int RefugeeIndex;
    
    public GameObject IconPrefab;
    [NonSerialized]
    public RefugeeIcon IconOfRefugee;

    public int Speed;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _gameInterface = FindObjectOfType<Escape>();

        IconOfRefugee = Instantiate(IconPrefab, GameObject.FindGameObjectWithTag("Icons Container").transform).GetComponent<RefugeeIcon>();
        IconOfRefugee.RefugeeForIcon = this;
        IconOfRefugee.gameObject.SetActive(false);

        _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
    }
    
    void Update()
    {
        if (_targetCheckpoint.Passable == true)
        {
            _animator.SetBool("IsWalking", true);
            IconOfRefugee.Icon = IconOfRefugee.ActiveImage;

            if (Vector2.Distance(transform.position, _targetCheckpoint.QueueElements[RefugeeIndex].transform.position) > 2f)
            {
                transform.position = Vector2.MoveTowards(transform.position, _targetCheckpoint.QueueElements[RefugeeIndex].transform.position, Speed * .5f * Time.deltaTime);
            } else
            {
                if(_targetCheckpoint.tag == "Final Checkpoint")
                {
                    Destroy(IconOfRefugee);
                    _gameInterface.RefugeesSaved++;
                    _gameInterface.RefugeesSavedInThisSession++;
                    _gameInterface.TotalPoints += RewardInPoints;
                    _gameInterface.CurrentRefugees.Remove(this);

                    for (int i = 0; i < _gameInterface.CurrentRefugees.Count; i++)
                    {
                        _gameInterface.CurrentRefugees[i].RefugeeIndex = i;
                    }

                    if (_gameInterface.CurrentRefugees.Count <= 0 && _gameInterface.CurrentWave <= _gameInterface.RefugeeWaves.Count - 1)
                    {
                        _gameInterface.TotalPoints += _gameInterface.RefugeeWaves[_gameInterface.CurrentWave].RewardInPoints;

                        _gameInterface.CurrentWave++;
                        _gameInterface.StartNextWave();
                    }

                    _gameInterface.SaveEscapeGamesData();

                    foreach (Checkpoint checkpoint in _gameInterface.Checkpoints)
                    {
                        checkpoint.RemoveQueueElement(RefugeeIndex);
                    }
                    if (_gameInterface.CurrentWave == _gameInterface.RefugeeWaves.Count)
                    {
                        _gameInterface.EndGame();
                    }
                    Destroy(gameObject);
                }
                _animator.SetBool("IsWalking", false);
                _currentCheckpointIndex++;
                _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
            }
            //TODO: make him towards the target checkpoint
        } else
        {
            _animator.SetBool("IsWalking", false);
            IconOfRefugee.Icon = IconOfRefugee.IdleImage;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Water")
        {
            _gameInterface.CurrentRefugees.Remove(this);
            _gameInterface.EndGameLose();
            Destroy(gameObject);
        }
        //if (collision.gameObject.tag == "Checkpoint")
        //{
        //    _currentCheckpointIndex++;
        //    _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
        //}
    }

    private void OnBecameInvisible()
    {
        if (IconOfRefugee != null)
        {
            IconOfRefugee.gameObject.SetActive(true);
            //Debug.Log("Refugee is now invisible");
        }
    }

    private void OnBecameVisible()
    {
        if (IconOfRefugee != null)
        {
            IconOfRefugee.gameObject.SetActive(false);
            //Debug.Log("Refugee is now visible");
        }
    }
}
