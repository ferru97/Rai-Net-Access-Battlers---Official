using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenuNew : MonoBehaviour {

    // toggle buttons

	// sliders
	public GameObject musicSlider;
    public GameObject effectSlider;

	private float sliderValue = 0.0f;
    private float eff_sliderValue = 0.0f;
    private float sliderValueXSensitivity = 0.0f;
	private float sliderValueYSensitivity = 0.0f;
	private float sliderValueSmoothing = 0.0f;

    //MARGHERITA COTTO E PATATINE
	public void  Start (){
        PlayerPrefs.SetInt("NormalDifficulty",1);
        PlayerPrefs.SetFloat("MusicVolume", 0.7f);

		// check slider values
		musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");

	}

	public void  Update (){
		sliderValue = musicSlider.GetComponent<Slider>().value;
        eff_sliderValue = effectSlider.GetComponent<Slider>().value;

        PlayerPrefs.SetFloat("EffectVolume", eff_sliderValue);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
	}


	public void MusicSlider (){
		PlayerPrefs.SetFloat("MusicVolume", sliderValue);
	}

    public void EffectSlider()
    {
        PlayerPrefs.SetFloat("EffectVolume", eff_sliderValue);
    }

}