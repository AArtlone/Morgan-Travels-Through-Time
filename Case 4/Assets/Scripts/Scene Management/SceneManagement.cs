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

        // If the player has already created a character then 
        // we just start the main menu instead.
        if (Character.Instance.CharacterCreation && SceneManager.GetActiveScene().name != "Character Customization")
        {
            if (Character.Instance.TutorialCompleted)
            {
                if (Character.Instance.LastScene != "Tutorial Map Area" && Character.Instance.LastMapArea != "Tutorial Map Area" && Character.Instance.LastScene != "Guess Clothing Puzzle" && Character.Instance.LastScene != "Spelling Puzzle" && Character.Instance.LastScene != "Hidden Objects Puzzle" && Character.Instance.LastScene != "Escape Game")
                {
                    Debug.Log("aaaaaaaaa");
                    sceneToLoad = Character.Instance.LastScene;
                }
                else if (Character.Instance.LastMapArea == "Tutorial Map Area")
                {
                    sceneToLoad = Character.Instance.LastMapArea;
                } else
                {
                    sceneToLoad = Character.Instance.LastMapArea;
                }
                Character.Instance.RefreshJsonData();
            }
            else
            {
                sceneToLoad = Character.Instance.LastMapArea;
            }
        }
        else if (SceneManager.GetActiveScene().name == "Character Customization")
        {
            sceneToLoad = Character.Instance.LastMapArea;
        }
        else if (SceneManager.GetActiveScene().name == "Main Map")
        {
            sceneToLoad = Character.Instance.LastScene;
        } else if (SceneManager.GetActiveScene().name == "Logo Introduction" && Character.Instance.CharacterCreation)
        {
            SceneManager.LoadScene(Character.Instance.LastMapArea);
        } else if (SceneManager.GetActiveScene().name == "Logo Introduction" && Character.Instance.CharacterCreation == false)
        {
            sceneToLoad = "Introduction";
        }

        LoadingScreen.SetActive(true);
        Debug.Log(Character.Instance.LastMapArea);
        Debug.Log(Character.Instance.LastScene);
        Debug.Log(sceneToLoad);
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
        string sceneToLoad = string.Empty;
        if (newScene == "Logo Introduction")
        {
            SceneManager.LoadScene(newScene);
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "Tutorial Map Area" || SceneManager.GetActiveScene().name == "Castle Area" || SceneManager.GetActiveScene().name == "Jacob's House" || SceneManager.GetActiveScene().name == "Escape Game")
            {
                Character.Instance.LastMapArea = newScene;
                sceneToLoad = Character.Instance.LastMapArea;
            }
            else
            {
                Character.Instance.LastScene = newScene;
                sceneToLoad = newScene;
            }

            Character.Instance.RefreshJsonData();
            sceneToLoad = newScene;
        }

        LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneToLoad);

        yield return new WaitForEndOfFrame();
    }

    public void RemoveSingletonInstance()
    {
        Destroy(Instance);
        Destroy(gameObject);
    }
}
