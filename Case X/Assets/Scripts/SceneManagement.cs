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
}
