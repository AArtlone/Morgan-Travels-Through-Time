using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle_Candle : MonoBehaviour
{
    private GameObject _glow;
    private GameObject _flame;
    public ParticleSystem _smokeParticle;

    public AudioClip[] Clip;
    private AudioSource _candleAudio;

    private void Start()
    {
        _glow = gameObject.transform.parent.GetChild(1).gameObject;
        _flame = gameObject.transform.parent.GetChild(3).gameObject;

        _candleAudio = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        if (_glow.activeSelf == true)
        {
            _smokeParticle.Play();
            _glow.SetActive(false);
            _flame.SetActive(false);
            _candleAudio.clip = Clip[0];
        } else
        {
            _glow.SetActive(true);
            _flame.SetActive(true);
            _candleAudio.clip = Clip[1];
        }

        _candleAudio.Play();
    }

}
