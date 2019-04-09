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
            SkipButton.SetActive(false);
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

    private void LoadGame()
    {
        AnimatorOfFade.gameObject.SetActive(false);
        Character.Instance.IsCutscenePassed = true;
        Character.Instance.RefreshJsonData();

        SceneManager.LoadScene("Language Selection");
    }
}
