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
        }
    }

    private void Start()
    {
        // If the player has already created a character then 
        // we just start the main menu instead.
        if (Character.Instance.CharacterCreation && SceneManager.GetActiveScene().name != "Character Customization")
        {
            if (Character.Instance.TutorialCompleted)
            {
                LoadScene("Main Map");
            }
            else
            {
                LoadScene("Tutorial Map Area");
            }
        }
    }

    public void LoadCharacterCustomization()
    {
        //Character.Instance.CharacterCreation = false;
        Character.Instance.RefreshJsonData();
        SceneManager.LoadScene("Character Customization");
    }
    
    public void LoadScene(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }
}
