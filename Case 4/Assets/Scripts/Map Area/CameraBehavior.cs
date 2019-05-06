using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class CameraBehavior : MonoBehaviour
{
    #region Camera screen movement variables
    public GameObject Background;
    public MapPart MapPart;
    [NonSerialized]
    public Bounds BackgroundBounds;
    #endregion

    #region Camera screen interaction properties
    [Range(0, 100)]
    [SerializeField]
    private float _cameraSpeed = 30f;
    private bool _isCameraTravelling = false;
    public Vector3 TapPosition;
    public bool IsInteracting = false;
    public bool IsEntityTappedOn = false;
    public bool IsInterfaceElementSelected = false;
    public bool IsUIOpen = false;
    #endregion

    // Camera components
    private MapEnvironmentManager _mapEnvironmentManager;

    private void Start()
    {
        BackgroundBounds = Background.GetComponent<SpriteRenderer>().bounds;
        _mapEnvironmentManager = FindObjectOfType<MapEnvironmentManager>();
        InterfaceManager.Instance.CameraBehavior = this;
        Character.Instance.CameraBehavior = this;
    }

    private void Update()
    {
        //IsEntityTappedOn = false;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.HitScreen);

            if (InterfaceManager.Instance.ItemActionsWindow != null)
            {
                if (InterfaceManager.Instance.ItemActionsWindow.activeSelf == true)
                {
                    InterfaceManager.Instance.ClosePopup(InterfaceManager.Instance.ItemActionsWindow);
                }
            }

            RaycastHit2D hitCanvas = Physics2D.Raycast(Input.GetTouch(0).position, Vector2.down, 1000);

            RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.down, 1000);

            if (hitCanvas.transform != null)
            {
                // Layer 11 is IgnoreCamera meant only for this script!
                if (hitCanvas.transform.gameObject.layer == 11)
                {
                    //Debug.Log(hitCanvas.transform.name);
                    IsInterfaceElementSelected = true;
                }
            }

            if (hit2D.transform != null)
            {
                // Layer 11 is IgnoreCamera meant only for this script!
                // We also check if we clicked over an entity in the map area, because
                // we want it to be interactable unlike the canvas elements. This
                // includes the second if statement after!
                if (hit2D.transform.gameObject.layer == 11 &&
                    (hit2D.transform.tag != "Item Drop" &&
                    hit2D.transform.tag != "NPC" &&
                    hit2D.transform.tag != "Hidden Objects Puzzle" &&
                    hit2D.transform.tag != "Guess Clothes Puzzle" &&
                    hit2D.transform.tag != "Escape Game" &&
                    hit2D.transform.tag != "Sign"))
                {
                    IsInterfaceElementSelected = true;
                }
            }
        }

        //Debug.Log(IsInteracting + " " + IsInterfaceElementSelected + " " + IsEntityTappedOn + " " + IsUIOpen);

        if (IsInteracting == false && IsInterfaceElementSelected == false && IsEntityTappedOn == false && IsUIOpen == false)
        {
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || _isCameraTravelling == false)
            {
                if (Input.touchCount > 0)
                {
                    if (IsUIOpen == false)
                    {
                        TapPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    }
                }

                //GameObject prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //prim.transform.position = _tapPosition;

                _isCameraTravelling = true;
            }
            else
            {
                if (Vector3.Distance(transform.position, TapPosition) > 1f)
                {
                    //transform.position = Vector3.MoveTowards(transform.position, new Vector3(TapPosition.x, transform.position.y, transform.position.z), _cameraSpeed * Time.deltaTime * 0.5f);

                    //Vector3 newPosition = transform.position;
                    //newPosition.x = Mathf.Clamp(transform.position.x, BackgroundBounds.min.x + 5, BackgroundBounds.max.x - 5);
                    //transform.position = newPosition;
                }
                else
                {
                    _isCameraTravelling = false;
                }
            }
        }

        if (SceneManager.GetActiveScene().name != "Escape Game")
        {
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && IsInteracting == false && IsUIOpen == false && IsEntityTappedOn == false && DialogueManager.Instance.DialogueTemplate.gameObject.activeSelf == false)
            {
                RaycastHit2D hitObj = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector3.forward, 1000);

                if (hitObj.transform != null)
                {
                    //Debug.Log(hitObj.transform.tag);
                    switch (hitObj.transform.tag)
                    {
                        case "NPC":
                            InterfaceManager.Instance.LoadCharacterDialogueAppearance();
                            // Replace with interaction mechanic in the future
                            hitObj.transform.GetComponent<NPC>().ContinueDialogue();
                            IsEntityTappedOn = true;
                            IsInteracting = true;
                            IsUIOpen = true;
                            break;
                        case "Hidden Objects Puzzle":
                            hitObj.transform.GetComponent<Puzzle>().InitiateHiddenObjectsPuzzle();
                            IsEntityTappedOn = true;
                            IsInteracting = true;
                            IsUIOpen = true;
                            break;
                        case "Guess Clothes Puzzle":
                            hitObj.transform.GetComponent<Puzzle>().InitiateGuessClothesPuzzle();
                            IsEntityTappedOn = true;
                            IsInteracting = true;
                            IsUIOpen = true;
                            break;
                        case "Escape Game":
                            Character.Instance.LastMapArea = SceneManager.GetActiveScene().name;
                            Character.Instance.LastScene = "Escape Game";
                            Character.Instance.RefreshJsonData();
                            IsInteracting = true;
                            IsUIOpen = true;
                            SceneManager.LoadScene("Escape Game");
                            break;
                        case "Item Drop":
                            Item hitItemDrop = hitObj.transform.GetComponent<Item>();
                            InterfaceManager.Instance.ItemSelected = hitItemDrop;
                            InterfaceManager.Instance.DisplayActionsMenu();
                            break;
                        case "Area Interactable":
                            hitObj.transform.GetComponent<ToggleObject>().Replace();
                            break;
                    }
                }
            }
        }
    }
        
}
