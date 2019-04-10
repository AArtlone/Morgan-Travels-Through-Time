using UnityEngine;

public class IntroSounds : MonoBehaviour
{
    public AudioSource ASource;
    public AudioClip BackgroundMusic;

    public void PlayBackgroundMusic()
    {
        ASource.PlayOneShot(BackgroundMusic, 1f);
    }
}
