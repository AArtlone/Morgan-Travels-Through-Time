using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Every audio source must have an audio type assigned in order to seperate
    // the volume controls for the player in the settings menu.
    public enum AudioType { Background, Sound }
    public AudioType AudioSourceType;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        // At the start of the scene, every audio controller will add itself to
        // the settings singleton where different audio types are stored for
        // volume manipulation during the scene's initialization and play.
        switch (AudioSourceType)
        {
            case AudioType.Background:
                SettingsManager.Instance.BackgroundSources.Add(_audioSource);
                _audioSource.volume = SettingsManager.Instance.SoundEffectsVolume / 100f;
                break;
            case AudioType.Sound:
                SettingsManager.Instance.SoundSources.Add(_audioSource);
                _audioSource.volume = SettingsManager.Instance.SoundEffectsVolume / 100f;
                break;
        }
    }
}
