using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public GameObject Background;
    private Bounds _backgroundBounds;
    public Transform CharacterAvatar;
    [Range(0, 100)]
    public float CameraSpeed;
    private bool _isCameraTravelling = false;
    private Vector3 _tapPosition;
    public bool IsInteracting = false;

    private void Start()
    {
        _backgroundBounds = Background.GetComponent<SpriteRenderer>().bounds;
    }

    private void Update()
    {
        Debug.Log(IsInteracting);
        if (IsInteracting == false)
        {
            if (Input.touchCount > 0 || _isCameraTravelling == false)
            {
                _tapPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                GameObject prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                prim.transform.position = _tapPosition;

                _isCameraTravelling = true;
            }
            else
            {
                if (Vector3.Distance(transform.position, _tapPosition) > 1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(_tapPosition.x, transform.position.y, transform.position.z), CameraSpeed * Time.deltaTime * 2);

                    Vector3 newPosition = transform.position;
                    newPosition.x = Mathf.Clamp(transform.position.x, _backgroundBounds.min.x + 5, _backgroundBounds.max.x - 5);
                    transform.position = newPosition;
                }
                else
                {
                    _isCameraTravelling = false;
                }
            }
        }

        if (Input.touchCount > 0 && IsInteracting == false)
        {
            RaycastHit2D hitObj = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector3.forward, 1000);

            if (hitObj.transform.tag == "NPC")
            {
                // Replace with interaction mechanic in the future
                hitObj.transform.GetComponent<NPC>().ContinueDialogue();
                IsInteracting = true;
            }
            else if (hitObj.transform.tag == "Hidden Objects Puzzle")
            {
                hitObj.transform.GetComponent<Puzzle>().InitiateHiddenObjectsPuzzle();
                IsInteracting = true;
            }
            else if (hitObj.transform.tag == "Guess Clothes Puzzle")
            {
                hitObj.transform.GetComponent<Puzzle>().InitiateGuessClothesPuzzle();
                IsInteracting = true;
            }
            else if (hitObj.transform.tag == "Escape Game")
            {
                hitObj.transform.GetComponent<SceneManagement>().LoadEscapeGame();
                IsInteracting = true;
            }
        }
    }
}
