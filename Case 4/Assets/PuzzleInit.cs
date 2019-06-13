using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleInit : MonoBehaviour
{
    public string SceneToLoad;

    public void LoadScene()
    {
        StartCoroutine(LoadSceneCo());
    }

    public IEnumerator LoadSceneCo()
    {
        InterfaceManager.Instance.LoadingScreen.SetActive(true);

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(SceneToLoad);

        yield return new WaitForEndOfFrame();
    }
}
