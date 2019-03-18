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
    
    public GameObject IconPrefab;
    [NonSerialized]
    public RefugeeIcon IconOfRefugee;

    public int Speed;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _gameInterface = GetComponentInParent<Escape>();

        IconOfRefugee = Instantiate(IconPrefab).GetComponent<RefugeeIcon>();
        IconOfRefugee.RefugeeForIcon = this;
        IconOfRefugee.gameObject.SetActive(false);

        _gameInterface.Refugees.Add(this);

        _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
    }
    
    void Update()
    {
        if (_targetCheckpoint.Passable == true)
        {
            _animator.SetBool("Active", true);
            IconOfRefugee.Icon = IconOfRefugee.ActiveImage;

            if (Vector2.Distance(transform.position, _targetCheckpoint.gameObject.transform.position) > 1f)
            {
                transform.position = Vector2.Lerp(transform.position, _targetCheckpoint.gameObject.transform.position, .7f * Time.deltaTime);
            }
            //TODO: make him towards the target checkpoint
        } else
        {
            _animator.SetBool("Active", false);
            IconOfRefugee.Icon = IconOfRefugee.IdleImage;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Final Checkpoint")
        {
            _gameInterface.RefugeesSaved++;
            _gameInterface.Refugees.Remove(this);
            Destroy(gameObject);
        }
        if(collision.gameObject.tag == "Checkpoint")
        {
            _currentCheckpointIndex++;
            _targetCheckpoint = _gameInterface.Checkpoints[_currentCheckpointIndex];
        }
    }

    private void OnBecameInvisible()
    {
        IconOfRefugee.gameObject.SetActive(true);
        Debug.Log("AAAAAAAA");
    }

    private void OnBecameVisible()
    {
        IconOfRefugee.gameObject.SetActive(false);
        Debug.Log("EEEEEEEE");
    }
}
