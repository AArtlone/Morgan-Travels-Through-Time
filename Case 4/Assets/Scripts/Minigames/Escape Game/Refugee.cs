using UnityEngine;
using System;

public class Refugee : MonoBehaviour
{
    private Escape _gameInterface;

    private int _currentCheckpointIndex = 0;
    private Checkpoint _targetCheckpoint;
    public enum RefugeeStatus { Walking, Wondering };
    public RefugeeStatus Status;
    private Animator _animator;
    private Rigidbody2D _rb;
    public int RewardInPoints;
    public int RefugeeIndex;
    public bool WonderingLeft;
    public bool WonderingRight;
    public bool FacingLeft;
    public bool FacingRight;

    public GameObject IconPrefab;
    [NonSerialized]
    public RefugeeIcon IconOfRefugee;

    public int Speed;
    public int WonderingSpeed;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _gameInterface = FindObjectOfType<Escape>();

        IconOfRefugee = Instantiate(IconPrefab, GameObject.FindGameObjectWithTag("Icons Container").transform).GetComponent<RefugeeIcon>();
        IconOfRefugee.RefugeeForIcon = this;
        IconOfRefugee.gameObject.SetActive(false);

        _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
        _animator.SetBool("IsWalking", true);
        WonderingRight = true;
        FacingRight = true;
    }
    
    void Update()
    {
        while (_targetCheckpoint.Passable == false)
        {
            for(int i = _currentCheckpointIndex; i >= 0; i--)
            {
                if (_gameInterface.Checkpoints[i].Passable == true)
                {
                    _targetCheckpoint = _gameInterface.Checkpoints[i];
                    break;
                } 
            }
        }
        
        if(_currentCheckpointIndex < _gameInterface.Checkpoints.Count - 1&&
            _gameInterface.Checkpoints[_currentCheckpointIndex + 1].Passable == true)
        {
            _currentCheckpointIndex++;
            _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
            ChangeRefugeeStatus(RefugeeStatus.Walking);
        }

        if (Status == RefugeeStatus.Walking)
        {
            MoveTowardsCheckpointQueueElement();
        } else if (Status == RefugeeStatus.Wondering)
        {
            Wondering();
        }
    }

    private void ChangeRefugeeStatus(RefugeeStatus NewRefugeeStatus)
    {
        Status = NewRefugeeStatus; 
    }

    /// <summary>
    /// Moves the NPC towards it's Queue Element of the targeted checkpoint
    /// </summary>
    private void MoveTowardsCheckpointQueueElement()
    {
        IconOfRefugee.Icon = IconOfRefugee.ActiveImage;
        if (Vector2.Distance(transform.position, _targetCheckpoint.QueueElements[RefugeeIndex].transform.position) > 2f)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetCheckpoint.QueueElements[RefugeeIndex].transform.position, Speed * .5f * Time.deltaTime);
        } else
        {
            if (_targetCheckpoint.tag == "Final Checkpoint")
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


                foreach (Checkpoint checkpoint in _gameInterface.Checkpoints)
                {
                    checkpoint.RemoveQueueElement(RefugeeIndex);
                }
                if (_gameInterface.CurrentWave == _gameInterface.RefugeeWaves.Count)
                {
                    _gameInterface.Completed = true;
                    _gameInterface.EndGame();
                }

                _gameInterface.SaveEscapeGamesData();
                Destroy(gameObject);
            }
            //_currentCheckpointIndex++;
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
            _gameInterface.CurrentRefugees.Remove(this);
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
