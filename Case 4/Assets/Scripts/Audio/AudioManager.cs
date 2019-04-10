using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    #region Audio clips that can be played from the PlaySound function.
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
    #endregion

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            // We want to be able to access the dialogue information from any scene.
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Allows you to play clips inside the audio manager by specifying which audio
    /// clip you want. The clip will be played at the player's configured volume.
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySound(AudioClip clip)
    {
        _audioSource.volume = SettingsManager.Instance.SoundEffectsVolume / 100f;
        _audioSource.PlayOneShot(clip);
    }
}
