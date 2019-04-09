using UnityEngine;

public class AudioController : MonoBehaviour
{
    public enum AudioType { Background, Sound }
    public AudioType AudioSourceType;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

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
