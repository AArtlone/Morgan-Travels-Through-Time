using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;

    private void Awake()
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
            }
        }
    }

    public void LoadCharacterCustomization()
    {
        //Character.Instance.CharacterCreation = false;
        Character.Instance.LastScene = "Character Customization";
        Character.Instance.RefreshJsonData();

        SceneManager.LoadScene("Character Customization");
    }

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
}
