using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    [Header("The object it will show up once the player clicks on this one.")]
    public GameObject ReplaceWith;

    private CameraBehavior _cameraBehaviour;

    private void Start()
    {
        _cameraBehaviour = FindObjectOfType<CameraBehavior>();
    }
    
    public void Replace()
    {
        if (_cameraBehaviour.IsEntityTappedOn == false)
        {
            // We can also generate it, but for the sake of performance I am
            // using a pre-defined one in the scene hierarchy and enable between them.
            ReplaceWith.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
