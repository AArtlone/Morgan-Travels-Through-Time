using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraIntroController : MonoBehaviour
{
    public GameObject TargetPosition;
    [Range(0, 10)]
    public float CameraSpeed;
    public GameObject SkipButton;

    // FADE MECHANIC references
    public Animator AnimatorOfFade;

    private void Start()
    {
        if (Character.Instance.IsCutscenePassed)
        {
            SkipButton.SetActive(true);
        } else
        {
            // True only for the playtest
            SkipButton.SetActive(true);
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, TargetPosition.transform.position) > 1f)
        {
            Vector3 NewPosition = Vector2.MoveTowards(transform.position, TargetPosition.transform.position, Time.deltaTime * CameraSpeed);
            NewPosition.z = -1;
            transform.position = NewPosition;
        } else
        {
            AnimatorOfFade.gameObject.SetActive(true);
            Invoke("LoadGame", 1f);
        }
    }

    public IEnumerator LoadGameCo()
    {
        string sceneToLoad = string.Empty;
        if (Character.Instance.IsCutscenePassed && Character.Instance.CharacterCreation)
        {
            sceneToLoad = Character.Instance.LastScene;
        }
        else if (Character.Instance.IsCutscenePassed && Character.Instance.CharacterCreation == false)
        {
            sceneToLoad = "Beginning Character Creation";
        }
        else
        {
            Character.Instance.IsCutscenePassed = true;
            Character.Instance.RefreshJsonData();
            sceneToLoad = "Language Selection";
        }

        InterfaceManager.Instance.LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneToLoad);

        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// This function, at the beginning of the game, initiates a scene based on the
    /// current progress and last destinations of the player in the game.
    /// </summary>
    public void LoadGame()
    {
        StartCoroutine(LoadGameCo());
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.parent.tag == "Intro Picture Trigger")
        {
            col.transform.parent.GetComponent<FrameController>().InitiateFrameVisualization();
        }
    }
}
