using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public string AudioSourceType;
    private Slider _slider;
    private float _volume;

    private void Start()
    {
        _slider = GetComponent<Slider>();

        switch (AudioSourceType)
        {
            case "Background":
                _slider.value = SettingsManager.Instance.BackgroundMusicVolume / 100f;
                break;
            case "Sound":
                _slider.value = SettingsManager.Instance.SoundEffectsVolume / 100f;
                break;
        }
    }

    /// <summary>
    /// This function runs when the player updates the volume in any of the audio
    /// types from the settings interface.
    /// </summary>
    public void SetVolume()
    {
        _volume = _slider.value;
        //Debug.Log(_volume + " | " + (int)(_volume * 100));
        switch (AudioSourceType)
        {
            case "Background":
                SettingsManager.Instance.BackgroundMusicVolume = (int)(_volume * 100);
                break;
            case "Sound":
                SettingsManager.Instance.SoundEffectsVolume = (int)(_volume * 100);
                break;
        }
        SettingsManager.Instance.UpdateAudioVolume();
    }
}
