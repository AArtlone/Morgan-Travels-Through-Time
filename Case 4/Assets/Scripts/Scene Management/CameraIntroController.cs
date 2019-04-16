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

    /// <summary>
    /// This function, at the beginning of the game, initiates a scene based on the
    /// current progress and last destinations of the player in the game.
    /// </summary>
    public void LoadGame()
    {
        AnimatorOfFade.gameObject.SetActive(false);

        //Debug.Log(Character.Instance.LastMapArea);
        if (Character.Instance.IsCutscenePassed && Character.Instance.CharacterCreation)
        {
            SceneManager.LoadScene(Character.Instance.LastMapArea);
        } else if (Character.Instance.IsCutscenePassed && Character.Instance.CharacterCreation == false)
        {
            SceneManager.LoadScene("Beginning Character Creation");
        } else
        {
            Character.Instance.IsCutscenePassed = true;
            Character.Instance.RefreshJsonData();
            SceneManager.LoadScene("Beginning Character Creation");
        }
    }
}
