using UnityEngine;

public class FadeScreenController : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // These two functions start either fade effects from a transition
    public void FadeInCamera()
    {
        _animator.SetBool("Fade", false);
    }

    public void FadeOutCamera()
    {
        _animator.SetBool("Fade", true);
    }

    // These two will start/stop the effects once they are first initiated. Once
    // the fade bool is initiated, it will transition from fade out to fade in
    // but then has to be stopped from looping back and so on.
    public void StartTransition()
    {
        _animator.SetBool("IsTransitionComplete", false);
    }

    public void EndTransition()
    {
        _animator.SetBool("IsTransitionComplete", true);
    }
}
