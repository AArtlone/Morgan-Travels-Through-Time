using UnityEngine;

public class FadeScreenController : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void FadeInCamera()
    {
        _animator.SetBool("Fade", false);
    }
    public void FadeOutCamera()
    {
        _animator.SetBool("Fade", true);
    }
    public void StartTransition()
    {
        _animator.SetBool("IsTransitionComplete", false);
    }
    public void EndTransition()
    {
        _animator.SetBool("IsTransitionComplete", true);
    }
}
