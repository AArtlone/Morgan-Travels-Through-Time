using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloset : MonoBehaviour
{

    private Animator _myAnimator;

    //audio variables
    private AudioSource _audioPlayer;
    public AudioClip[] AudioClip;

    private void Start()
    {
        _myAnimator = GetComponent<Animator>();
        _audioPlayer = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        if (_myAnimator.GetBool("Door_IsOpen"))
        {
            _myAnimator.SetBool("Door_IsOpen", false);
            _audioPlayer.clip = AudioClip[0];
        } else
        {
            _myAnimator.SetBool("Door_IsOpen", true);
            _audioPlayer.clip = AudioClip[1];
        }

        _audioPlayer.Play();
    }
}
