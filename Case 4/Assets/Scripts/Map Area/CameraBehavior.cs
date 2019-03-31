using UnityEngine;
using System;

public class CameraBehavior : MonoBehaviour
{
    #region Camera screen movement variables
    public GameObject Background;
    [NonSerialized]
    public Bounds BackgroundBounds;
    #endregion

    #region Camera screen interaction properties
    [Range(0, 100)]
    [SerializeField]
    private float _cameraSpeed = 30f;
    private bool _isCameraTravelling = false;
    private Vector3 _tapPosition;
    public bool IsInteracting = false;
    #endregion

    // Camera components
    private MapEnvironmentManager _mapEnvironmentManager;

    private void Start()
    {
        BackgroundBounds = Background.GetComponent<SpriteRenderer>().bounds;
        _mapEnvironmentManager = FindObjectOfType<MapEnvironmentManager>();
    }

    private void Update()
    {
        bool IsInterfaceElementSelected = false;
        if (Input.touchCount > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(Input.GetTouch(0).position, Vector2.down, 1000);
            
            if (hit.transform != null)
            {
                if (hit.transform.gameObject.layer == 10)
                {
                    IsInterfaceElementSelected = true;
                }
            }
        }

        if (IsInteracting == false && IsInterfaceElementSelected == false)
        {
            if (Input.touchCount > 0 || _isCameraTravelling == false)
            {
                if (Input.touchCount > 0)
                {
                    _tapPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                }

                //GameObject prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //prim.transform.position = _tapPosition;

                _isCameraTravelling = true;
            }
            else
            {
                if (Vector3.Distance(transform.position, _tapPosition) > 1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(_tapPosition.x, transform.position.y, transform.position.z), _cameraSpeed * Time.deltaTime * 0.5f);

                    Vector3 newPosition = transform.position;
                    newPosition.x = Mathf.Clamp(transform.position.x, BackgroundBounds.min.x + 5, BackgroundBounds.max.x - 5);
                    transform.position = newPosition;
                }
                else
                {
                    _isCameraTravelling = false;
                }
            }
        }

        if (Input.touchCount > 0 && IsInteracting == false && IsInterfaceElementSelected == false)
        {
            RaycastHit2D hitObj = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector3.forward, 1000);

            if (hitObj.transform != null)
            {
                if (hitObj.transform.tag == "NPC")
                {
                    InterfaceManager.Instance.LoadCharacterDialogueAppearance();
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
                    hitObj.transform.GetComponent<SceneManagement>().LoadScene("Escape Game");
                    IsInteracting = true;
                } else if (hitObj.transform.tag == "Sign")
                {
                    _mapEnvironmentManager.EnterAreaPart(hitObj.transform.GetComponent<SignController>().NewArea);
                }
            }
        }
    }
}
