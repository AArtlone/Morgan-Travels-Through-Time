using UnityEngine;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
    public int MovementSpeed;
    private Animator _animator;
    private Rigidbody2D _rb;
    private bool _isAvatarMoving = false;
    private string _direction;
    private bool _isInteracting = false;

    public Sprite ButtonIdle;
    public Sprite ButtonTouched;

    public Sprite InteractButtonIdle;
    public Sprite InteractButtonActive;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_isAvatarMoving)
        {
            if (_direction == "Left")
            {
                _rb.AddForce(Vector2.left * MovementSpeed * Time.deltaTime * 20);
            } else if (_direction == "Right")
            {
                _rb.AddForce(Vector2.right * MovementSpeed * Time.deltaTime * 20);
            }
        }
    }

    private void MovePlayerAnimator(string direction)
    {
        _animator.SetBool("Idle", false);
        if (direction == "Right")
        {
            _animator.SetBool("Left", false);
            _animator.SetBool("Right", true);
        }
        else if (direction == "Left")
        {
            _animator.SetBool("Right", false);
            _animator.SetBool("Left", true);
        }
    }

    public void MoveLeft(Object button)
    {
        GameObject buttonObj = (GameObject)button;
        buttonObj.GetComponent<Image>().sprite = ButtonTouched;

        if (_isAvatarMoving == false)
        {
            _isAvatarMoving = true;
            MovePlayerAnimator("Left");
            _direction = "Left";
        }
    }

    public void MoveRight(Object button)
    {
        GameObject buttonObj = (GameObject)button;
        buttonObj.GetComponent<Image>().sprite = ButtonTouched;

        if (_isAvatarMoving == false)
        {
            _isAvatarMoving = true;
            MovePlayerAnimator("Right");
            _direction = "Right";
        }
    }

    public void StopMovement(Object button)
    {
        GameObject buttonObj = (GameObject)button;
        buttonObj.GetComponent<Image>().sprite = ButtonIdle;

        _isAvatarMoving = false;
        _animator.SetBool("Idle", true);
    }

    public void IsInteracting(Object button)
    {
        GameObject buttonObj = (GameObject)button;
        buttonObj.GetComponent<Image>().sprite = InteractButtonActive;
        _isInteracting = true;
    }

    public void IsNotInteracting(Object button)
    {
        GameObject buttonObj = (GameObject)button;
        buttonObj.GetComponent<Image>().sprite = InteractButtonIdle;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isInteracting)
        {
            if (collision.transform.tag == "NPC")
            {
                // Replace with interaction mechanic in the future
                collision.GetComponent<NPC>().ContinueDialogue();
            }
            else if (collision.transform.tag == "Hidden Objects Puzzle")
            {
                collision.GetComponent<Puzzle>().InitiateHiddenObjectsPuzzle();
            }
            else if (collision.transform.tag == "Guess Clothes Puzzle")
            {
                collision.GetComponent<Puzzle>().InitiateGuessClothesPuzzle();
            }
            else if (collision.transform.tag == "Escape Game")
            {
                collision.GetComponent<SceneManagement>().LoadEscapeGame();
            }
            _isInteracting = false;
        }
    }
}
