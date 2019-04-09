using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip SoundPuzzleCompleted;

    [Header("These clips are only for hidden objects puzzles")]
    public AudioClip HintUsed;
    public AudioClip ItemFoundUsed;

    [Header("These clips are only for the refugee game")]
    public AudioClip RefugeeSaved;

    [Header("These clips are only for camera interaction")]
    public AudioClip HitScreen;

    [Header("These clips are only for the diary")]
    public AudioClip NewPageInDiary;

    [Header("UI/Interface related sound clips here only")]
    public AudioClip CloseWindow;
    public AudioClip ButtonPress;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        _audioSource.volume = SettingsManager.Instance.SoundEffectsVolume / 100f;
        _audioSource.PlayOneShot(clip);
    }
}
