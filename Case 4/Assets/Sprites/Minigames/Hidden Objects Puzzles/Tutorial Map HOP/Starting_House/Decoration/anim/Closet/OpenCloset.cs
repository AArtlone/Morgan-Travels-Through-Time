using UnityEngine;

public class OpenCloset : MonoBehaviour
{
    private Animator _myAnimator;

    //audio variables
    private AudioSource _audioPlayer;
    public AudioClip[] AudioClip;
    private HiddenObjectsPuzzle _gameInteface;

    private void Start()
    {
        _myAnimator = GetComponent<Animator>();
        _audioPlayer = GetComponent<AudioSource>();
        _gameInteface = FindObjectOfType<HiddenObjectsPuzzle>();
    }

    private void OnMouseDown()
    {
        Debug.Log(_gameInteface._isInterfaceToggledOn);
        Debug.Log(_gameInteface._isEndPopupOn);
        if (_gameInteface._isInterfaceToggledOn == false && _gameInteface._isEndPopupOn == false)
        {
            if (_myAnimator.GetBool("Door_IsOpen"))
            {
                _myAnimator.SetBool("Door_IsOpen", false);
                _audioPlayer.clip = AudioClip[0];
            }
            else
            {
                _myAnimator.SetBool("Door_IsOpen", true);
                _audioPlayer.clip = AudioClip[1];
            }

            _audioPlayer.Play();
        }
    }
}
