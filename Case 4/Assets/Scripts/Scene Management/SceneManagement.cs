using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;
    public GameObject LoadingScreen;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            // We want to be able to access the dialogue information from any scene.
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ResumeGameFromProgress()
    {
        StartCoroutine(ResumeGame());
    }

    /// <summary>
    /// This function makes sure the player's last game session is restored (last
    /// location or scenes he was at) when he restarts the game and enters it.
    /// </summary>
    public IEnumerator ResumeGame()
    {
        string sceneToLoad = string.Empty;
        sceneToLoad = Character.Instance.LastScene;

        LoadingScreen.SetActive(true);
        //Debug.Log(Character.Instance.LastMapArea);
        //Debug.Log(Character.Instance.LastScene);
        //Debug.Log(sceneToLoad);
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneToLoad);

        yield return new WaitForEndOfFrame();
    }

    public void LoadCharacterCustomization()
    {
        StartCoroutine(LoadCharacterCustomizationCo());
    }

    public IEnumerator LoadCharacterCustomizationCo()
    {
        //Character.Instance.CharacterCreation = false;
        Character.Instance.LastScene = "Character Customization";
        Character.Instance.RefreshJsonData();

        LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(Character.Instance.LastScene);

        yield return new WaitForEndOfFrame();
    }

    public void LoadScene(string newScene)
    {
        StartCoroutine(LoadSceneCo(newScene));
    }

    /// <summary>
    /// This function loads a new scene through mostly the editor and saves the
    /// last two in the player's configuration file for later loading at the 
    /// start of the game.
    /// </summary>
    /// <param name="newScene"></param>
    public IEnumerator LoadSceneCo(string newScene)
    {
        LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(newScene);

        yield return new WaitForEndOfFrame();
    }

    public void RemoveSingletonInstance()
    {
        Destroy(Instance);
        Destroy(gameObject);
    }
}
