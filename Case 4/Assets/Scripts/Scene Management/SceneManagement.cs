using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;

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

    /// <summary>
    /// This function makes sure the player's last game session is restored (last
    /// location or scenes he was at) when he restarts the game and enters it.
    /// </summary>
    public void ResumeGame()
    {
        // If the player has already created a character then 
        // we just start the main menu instead.
        if (Character.Instance.CharacterCreation && SceneManager.GetActiveScene().name != "Character Customization")
        {
            if (Character.Instance.TutorialCompleted)
            {
                if (Character.Instance.LastScene != "Tutorial Map Area" && Character.Instance.LastMapArea != "Tutorial Map Area")
                {
                    LoadScene(Character.Instance.LastScene);
                }
                else if (Character.Instance.LastMapArea == "Tutorial Map Area")
                {
                    LoadScene(Character.Instance.LastMapArea);
                }
                Character.Instance.RefreshJsonData();
            }
            else
            {
                Character.Instance.LastMapArea = "Tutorial Map Area";
                Character.Instance.RefreshJsonData();
                LoadScene("Tutorial Map Area");
            }
        }
        else if (SceneManager.GetActiveScene().name == "Character Customization")
        {
            LoadScene(Character.Instance.LastMapArea);
        }
        else if (SceneManager.GetActiveScene().name == "Main Map")
        {
            LoadScene(Character.Instance.LastScene);
        } else if (SceneManager.GetActiveScene().name == "Logo Introduction" && Character.Instance.CharacterCreation)
        {
            SceneManager.LoadScene(Character.Instance.LastMapArea);
        } else if (SceneManager.GetActiveScene().name == "Logo Introduction" && Character.Instance.CharacterCreation == false)
        {
            SceneManager.LoadScene("Introduction");
        }
    }

    public void LoadCharacterCustomization()
    {
        //Character.Instance.CharacterCreation = false;
        Character.Instance.LastScene = "Character Customization";
        Character.Instance.RefreshJsonData();

        SceneManager.LoadScene("Character Customization");
    }

    /// <summary>
    /// This function loads a new scene through mostly the editor and saves the
    /// last two in the player's configuration file for later loading at the 
    /// start of the game.
    /// </summary>
    /// <param name="newScene"></param>
    public void LoadScene(string newScene)
    {
        if (newScene == "Logo Introduction")
        {
            SceneManager.LoadScene(newScene);
        }
        else
        {
            if (newScene == "Main Map" || newScene == "Tutorial Map Area" || newScene == "Castle Area")
            {
                Character.Instance.LastMapArea = newScene;
            }
            else
            {
                Character.Instance.LastScene = newScene;
            }

            Character.Instance.RefreshJsonData();
            SceneManager.LoadScene(newScene);
        }
    }

    public void DestroySceneManagementInstance()
    {
        Destroy(Instance);
        Destroy(gameObject);
    }
}
