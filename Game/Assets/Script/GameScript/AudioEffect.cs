using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffect : MonoBehaviour {

    public GameObject DealingCard;
    public GameObject CardSelected;
    public GameObject CardCollect;

    public GameObject mainSong;

    private const float DEFAULT_EFFECT_VOLUME = 1f;
    private const float DEFAULT_MUSIC_VOLUME = 0.7f;

    // Use this for initialization
    void Start () {

        if(!PlayerPrefs.HasKey("EffectVolume"))
            PlayerPrefs.SetFloat("EffectVolume", DEFAULT_EFFECT_VOLUME);

        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", DEFAULT_MUSIC_VOLUME);

    }
	
	// Update is called once per frame
	void Update () {

        mainSong.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        DealingCard.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("EffectVolume");
        CardSelected.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("EffectVolume");
        CardCollect.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("EffectVolume");

    }


    public void playDealingCard()
    {
        DealingCard.GetComponent<AudioSource>().Play();
    }

    public void playCardSelected()
    {
        CardSelected.GetComponent<AudioSource>().Play();
    }

    public void playCardCollect()
    {
        CardCollect.GetComponent<AudioSource>().Play();
    }
}
