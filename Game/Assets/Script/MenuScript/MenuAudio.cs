using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour {
    private const float DEFAULT_EFFECT_VOLUME = 1f;
    private const float DEFAULT_MUSIC_VOLUME = 0.7f;

    public GameObject effectSound1;
    public GameObject effectSound2;
    public GameObject effectSound3;
    public GameObject effectSound4;

    // Use this for initialization
    void Start () {

        if (!PlayerPrefs.HasKey("EffectVolume"))
            PlayerPrefs.SetFloat("EffectVolume", DEFAULT_EFFECT_VOLUME);

        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", DEFAULT_MUSIC_VOLUME);
    }
	
	// Update is called once per frame
	void Update () {

        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        effectSound1.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("EffectVolume");
        effectSound2.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("EffectVolume");
        effectSound3.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("EffectVolume");
        effectSound4.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("EffectVolume");

    }
}
