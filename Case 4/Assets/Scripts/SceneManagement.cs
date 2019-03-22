using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void LoadCharacterCustomization()
    {
        //Character.Instance.CharacterCreation = false;
        Character.Instance.RefreshJsonData();
        SceneManager.LoadScene("Character Customization");
    }

    public void LoadMainMap()
    {
        SceneManager.LoadScene("Main Map");
    }

    public void LoadEscapeGame()
    {
        SceneManager.LoadScene("Escape Game");
    }

    public void LoadMapArea2()
    {
        SceneManager.LoadScene("Map Area 2");
    }
}
