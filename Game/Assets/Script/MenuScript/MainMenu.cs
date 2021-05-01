using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainMenu : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject OptionMenu;
    public GameObject inputIP;

    public float version = 0;
    public GameObject panelUpdate;
    public GameObject up_ver;
    public GameObject up_info;

    public bool requesting = false;
    public GameObject privateMatchCode;

    public GameObject messagePanel;
    public GameObject messagePanelText;

    private string id;
    public GameObject idUIText;
    public GameObject nicknameObj;
    string nickname = "";

    public GameObject versionText;

    public GameObject loadingSpinner;
    public GameObject buttonsGroup;
    public void Start()
    {
        versionText.GetComponent<Text>().text = "v" + version.ToString().Replace(",",".");
        PlayerPrefs.SetString("version",version.ToString().Replace(",","."));

        if (!PlayerPrefs.HasKey("player-uuid") || (PlayerPrefs.HasKey("player-uuid") && !PlayerPrefs.GetString("player-uuid").Contains("ms")))
        {
            this.id = Guid.NewGuid().ToString();
            System.Random rnd = new System.Random();
            int pos = rnd.Next(0, this.id.Length);
            this.id = this.id.Insert(pos, "ms");
            PlayerPrefs.SetString("player-uuid", this.id);
        }
        else
        {
            this.id = PlayerPrefs.GetString("player-uuid");
        }

        if (PlayerPrefs.HasKey("nickname") && PlayerPrefs.GetString("nickname") != "")
            nickname = PlayerPrefs.GetString("nickname");
        else
            nickname = "Player";

        nicknameObj.GetComponent<InputField>().text = nickname;
        PlayerPrefs.SetString("nickname", nickname);

        idUIText.GetComponent<Text>().text = "ID: "+this.id;

        if (PlayerPrefs.GetInt("first_start") != 1)
        {
            messagePanel.SetActive(true);
            buttonsGroup.SetActive(true);
            loadingSpinner.SetActive(false);
            PlayerPrefs.SetInt("first_start", 1);
        }
        else
        {
            StartCoroutine(GetRequest(NetInfo.HTTP_SERVER_INFO));
        }
    }

    public void playGame()
    {
        transform.SetAsFirstSibling();
        PlayerPrefs.SetInt("Multiplayer", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void playGameHost()
    {
        PlayerPrefs.SetInt("Multiplayer", 1);
        SceneManager.LoadScene("Game");
    }

    public void createPrivateGame()
    {
        PlayerPrefs.SetInt("Multiplayer", 2);
        SceneManager.LoadScene("Game");

    }

    public void joinPrivateGame()
    {
        
        if(privateMatchCode.GetComponent<Text>().text.Length > 0)
        {
            PlayerPrefs.SetInt("Multiplayer", 3);
            PlayerPrefs.SetString("code", privateMatchCode.GetComponent<Text>().text);
            SceneManager.LoadScene("Game");
        }
    }


    public void openWeb()
    {
        Application.OpenURL(NetInfo.WEBSITE_ADDR);
    }
    public void openDonate()
    {
        Application.OpenURL(NetInfo.PAYPAL_ADDR);
    }

    public void openDiscord()
    {
        Application.OpenURL(NetInfo.DISCORD_ADDR);
    }

    public void exit()
    {
        Application.Quit();
    }



    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        uwr.timeout = 8;
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.responseCode!=200)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            messagePanel.SetActive(true);
            messagePanelText.GetComponent<Text>().text = "ERROR!\n\nThe server may be offline...\nJoin the Discord server for more info";
            buttonsGroup.SetActive(true);
            loadingSpinner.SetActive(false);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            JSONObject x = new JSONObject(uwr.downloadHandler.text);
            if (float.Parse(x.GetField("v").ToString().Replace("\"", "")) > version)
            {
                Debug.Log(float.Parse(x.GetField("v").ToString().Replace("\"", "")));
                string alert = x.GetField("nv_msg").ToString().Replace("\"", "").Replace("\\n", "\n");
                messagePanelText.GetComponent<Text>().text = alert;
                messagePanel.SetActive(true);

            }
            else
            {
                string msg = x.GetField("msg").ToString().Replace("\"", "").Replace("\\n", "\n");
                if (msg != "-")
                {
                    messagePanelText.GetComponent<Text>().text = msg;
                    messagePanel.SetActive(true);
                }
            }
            
            buttonsGroup.SetActive(true);
            loadingSpinner.SetActive(false);
        }
    }

    public void copyID()
    {
        GUIUtility.systemCopyBuffer = this.id;
    }

    public void setNickname()
    {
        string newNick = nicknameObj.GetComponent<InputField>().text;
        if (newNick == "")
            newNick = "Player";

        nickname = newNick;
        Debug.Log("set nickname" + nickname);
        PlayerPrefs.SetString("nickname", newNick);
    }

}
