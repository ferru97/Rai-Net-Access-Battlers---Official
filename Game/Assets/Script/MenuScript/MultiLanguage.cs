using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiLanguage : MonoBehaviour {

    public  int lang = 0; //0-Eng 1-IT
    private const int MAX_LANG = 1;

    public GameObject multiplayers;
    public GameObject multiplayers1;
    public GameObject multiplayers2;

    public GameObject singleplayer;
    public GameObject singleplayer1;
    public GameObject singleplayer2;
    public GameObject singleplayer3;

    public GameObject about;
    public GameObject exit;
    public GameObject exit1;
    public GameObject exit2;
    public GameObject exit3;

    public GameObject settings;
    public GameObject settings1;
    public GameObject settings2;
    public GameObject settings3;
    public GameObject settings4;
    public GameObject settings5;
    public GameObject language;
    public GameObject language1;



    // Use this for initialization
    void Start () {
        setMenuEnglish();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setMenuEnglish()
    {
        multiplayers.GetComponent<Text>().text = "MULTIPLAYERS";
        multiplayers1.GetComponent<Text>().text = "Play";
        multiplayers1.GetComponent<Text>().text = "Join Game";
        singleplayer.GetComponent<Text>().text = "SINGLEPLAYER";
        singleplayer1.GetComponent<Text>().text = "Easy";
        singleplayer2.GetComponent<Text>().text = "---";
        singleplayer3.GetComponent<Text>().text = "---";
        about.GetComponent<Text>().text = "ABOUT & ROULES";
        exit.GetComponent<Text>().text = "EXIT";
        exit1.GetComponent<Text>().text = "Are You Sure?";
        exit2.GetComponent<Text>().text = "Yes";
        exit3.GetComponent<Text>().text = "No";
        //settings.GetComponent<Text>().text = "SETTINGS";
        settings1.GetComponent<Text>().text = "SETTINGS";
        settings2.GetComponent<Text>().text = "Camera Position";
        settings3.GetComponent<Text>().text = "Music Volume";
        settings4.GetComponent<Text>().text = "Effects Volume";
        settings5.GetComponent<Text>().text = "Return";
        language.GetComponent<Text>().text = "Language";
        language1.GetComponent<Text>().text = "English";

    }


    public void setMenuItalian()
    {
        multiplayers.GetComponent<Text>().text = "MULTIPLAYERS";
        multiplayers1.GetComponent<Text>().text = "Gioca";
        multiplayers1.GetComponent<Text>().text = "Host Game";
        singleplayer.GetComponent<Text>().text = "SINGLEPLAYER";
        singleplayer1.GetComponent<Text>().text = "Facile";
        singleplayer2.GetComponent<Text>().text = "Normale(In Arrivo)";
        singleplayer3.GetComponent<Text>().text = "Difficile(In Arrivo)";
        about.GetComponent<Text>().text = "INFO & REGOLE";
        exit.GetComponent<Text>().text = "ESCI";
        exit1.GetComponent<Text>().text = "Sei Sicuro?";
        exit2.GetComponent<Text>().text = "Si";
        exit3.GetComponent<Text>().text = "No";
        settings.GetComponent<Text>().text = "IMPOSTAZIONI";
        settings1.GetComponent<Text>().text = "IMPOSTAZIONI";
        settings2.GetComponent<Text>().text = "Posizione Camera";
        settings3.GetComponent<Text>().text = "Volume Musica";
        settings4.GetComponent<Text>().text = "Volume Effetti";
        settings5.GetComponent<Text>().text = "Indietro";
        language.GetComponent<Text>().text = "Lingua";
        language1.GetComponent<Text>().text = "Italiano";

    }


    public void changeLang()
    {
        if (lang < MAX_LANG)
            lang++;
        else
            lang = 0;
        PlayerPrefs.SetInt("Language", lang);

        switch (lang)
        {
            case 0:
                setMenuEnglish();
                break;
            case 1:
                setMenuItalian();
                break;
        }
  
    }

}
