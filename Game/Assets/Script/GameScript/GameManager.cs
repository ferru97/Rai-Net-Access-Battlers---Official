using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    public int round = 0;
    public char roundPlayer = 'X';
    public GameObject[] board = new GameObject[64];
    public GameObject[] opponentVirusCards = new GameObject[4];
    public GameObject[] opponentLinkCards = new GameObject[4];
    public GameObject[] opponentTerminalCards = new GameObject[5];
    public GameObject[] opponentStackLink = new GameObject[4];
    public GameObject[] opponentStackVirus = new GameObject[4];
    public GameObject menu;

    public int opponentVirusCollected = 0;
    public int opponentLinkCollected = 0;
    public int playerVirusCollected = 0;
    public int playerLinkCollected = 0;

    public bool multiplayer = true;
    public bool opponentReady = false; //True if opponent cooected
    public bool cardPlaced = false; // True if opponet have placed cards
    public MatchtSocket socket;

    public Sprite win_splash;
    public Sprite loose_splash;
    public Image end_splash;


    Player player;
    Text infoText;
    Text time;
    
    const int MAX_WAIT = 40;
    float seconds;
    bool counter = false;

    public bool endGame = false;

    public GameObject AI;

    public GameObject waitingPanelServer;
    public bool connection_break = false;
    public GameObject connErrorTxt; //Used by MultiplayerManager
    public GameObject errorBox;//Used by MultiplayerManager

    public Sprite switchSp;

    public GameObject infoSplash;
    public GameObject infoSplashText;
    
    public GameObject myNickTag;
    public GameObject opponentNickTag;
    string nickname = "";

    public GameObject light;

    private string version;

    // Use this for initialization
    void Start() {

        infoText = GameObject.FindWithTag("infoText").GetComponent<Text>();
        time = GameObject.FindWithTag("Time").GetComponent<Text>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        nickname = PlayerPrefs.GetString("nickname");
        version = PlayerPrefs.GetString("version");
        myNickTag.GetComponent<Text>().text = nickname;


        if (PlayerPrefs.GetInt("Multiplayer")>0)
        {
            socket = GameObject.FindWithTag("NetSocket").GetComponent<MatchtSocket>();
            socket.connect();
        }

       
        if (PlayerPrefs.GetInt("Multiplayer") == 0)
        {
            setOpponentNick("CPU");
            multiplayer = false;
            opponentReady = true;
        }
        else if (PlayerPrefs.GetInt("Multiplayer") == 1)
        {
            waitingPanelServer.SetActive(true);
            StartCoroutine(joinPublic());
        }
        else if (PlayerPrefs.GetInt("Multiplayer") == 2)
        {
            waitingPanelServer.SetActive(true);
            StartCoroutine(createPrivate());
        }
        else if (PlayerPrefs.GetInt("Multiplayer") == 3)
        {
            Debug.Log("Join private");
            string code = PlayerPrefs.GetString("code");
            waitingPanelServer.SetActive(true);
            StartCoroutine(joinPrivate(code));
        }

    }

    public void setOpponentNick(string nick)
    {
        opponentNickTag.GetComponent<Text>().text = nick;
    }

    public void showAlert(string msg)
    {
        infoSplashText.GetComponent<Text>().text = msg;
        infoSplash.SetActive(true);

    }

    public void hideAlert()
    {
        infoSplash.SetActive(false);
    }

    IEnumerator joinPublic()
    {
        //Wait for 3 seconds
        yield return new WaitForSeconds(4);
        socket.joinPublic(nickname, version);
    }

    IEnumerator createPrivate()
    {
        //Wait for 3 seconds
        yield return new WaitForSeconds(4);
        socket.createPrivate(nickname, version);
    }
    IEnumerator joinPrivate(string code)
    {
        //Wait for 3 seconds
        yield return new WaitForSeconds(4);
        socket.joinPrivate(code, nickname, version);
    }

    public void onMultiPrivateCreated(string code)
    {
        waitingPanelServer.SetActive(false);
        string message = "Private match created!\nJoining code: " + code;
        showAlert(message);
    }

    // Update is called once per frame
    void Update() {
        if (!connection_break)
        {

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (!menu.activeSelf)
                    menu.SetActive(true);
                else
                    menu.SetActive(false);
            }

            if (counter)
            {
                seconds = seconds - Time.deltaTime;
                time.text = ((int)seconds).ToString();
                if (seconds <= 0)
                {
                    stopCounter();
                    player.endCounter();
                }
            }
        }
        
    }

    public void openWeb()
    {
        Application.OpenURL("http://www.rainetdigital.com");
    }

    public void startCounter()
    {
        counter = true;
        seconds = MAX_WAIT;
        time.text = MAX_WAIT.ToString();
    }

    public void stopCounter()
    {
        counter = false;
        time.text = "--";
    }


    public void setInfoText(string text)
    {
        infoText.text = text;
    }

    public void nextRound()
    {
        checkEndGame();
        round ++;
        if (roundPlayer == 'A')
        {
            stopCounter();
            roundPlayer = 'B';
        }
        else
        {
            roundPlayer = 'A';
            startCounter();
        }

        if (multiplayer)
            socket.nextRouund();

        setInfoText("Round "+ round);
        Debug.Log(round+"/"+roundPlayer);
           
    }


    public bool checkEndGame()
    {
        if (opponentLinkCollected == 4 || playerVirusCollected == 4)
        {
            endGame = true;
            end_splash.GetComponent<Image>().sprite = loose_splash;
            end_splash.gameObject.SetActive(true);
            if (multiplayer)
            {
                socket.stop = true;
            }
            light.GetComponent<Light>().intensity = 0.25f;

            return true;
        }
        if (opponentVirusCollected == 4 || playerLinkCollected == 4)
        {
            if (multiplayer)
            {
                socket.emitMatchEnd("win");
                socket.stop = true;
            }
            endGame = true;
            end_splash.GetComponent<Image>().sprite = win_splash;
            end_splash.gameObject.SetActive(true);

            light.GetComponent<Light>().intensity = 0.25f;
            return true;
        }

        return false;
    }


    public  void setStart(){

        if (!multiplayer)
        {
            roundPlayer = 'A';
            startCounter();
        }
             
    }

    void OnApplicationQuit()
    {
        if (multiplayer)
            socket.emitExit();
    }

    public void exitGame()
    {
        if(multiplayer)
            socket.emitExit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    public void multiUpdatePlacedPos(string s)
    {
        if (multiplayer)
            socket.emitCardPlacement(s);
    }

    public void multiPLaceOpponentCards(string list)
    {
        string id;
        int i = 0;
        int c = 0;
        int tempPos = 0;
        char[] typeList = new char[8];
        while (i < list.Length)
        {
            if (list[i]!='-')
            {
                id = "";
                id = id + list[i];
                i++;

                if (i < list.Length && list[i] != '-')
                {
                    id = id + list[i];
                    i++;
                }

                tempPos = Int32.Parse(id);
                if (tempPos > 7)
                    tempPos -= 8  ;

                if (c < 4)
                    typeList[tempPos] = 'l';
                else
                    typeList[tempPos] = 'v';

                c++;
            }else
                 i++;
        }


        i = 0;
        Card cardScript;
        Slot slotScript;
        int virusIndex = 0;
        int linkINdex = 0;
        while (i < 8)
        {
            tempPos = i;
            if (typeList[tempPos] == 'v')
            {
                tempPos = 63 - tempPos;
                if (tempPos == 60 || tempPos == 59)
                    tempPos -= 8;

                cardScript = opponentVirusCards[virusIndex].GetComponent<Card>();
                slotScript = board[tempPos].GetComponent<Slot>();
                cardScript.Mouve(false,slotScript,opponentStackLink,opponentStackVirus);
                virusIndex++;
            }
            else
            {
                tempPos = 63 - tempPos;
                if (tempPos == 60 || tempPos == 59)
                    tempPos -= 8;

                cardScript = opponentLinkCards[linkINdex].GetComponent<Card>();
                slotScript = board[tempPos].GetComponent<Slot>();
                cardScript.Mouve(false, slotScript, opponentStackLink, opponentStackVirus);
                linkINdex++;
            }
            i++;
        }

        if(player.placedCards == 8)
            setInfoText("Round 0");
    }


    public void multiSetAction(string a)
    {
        if (multiplayer)
            socket.local_action = a;
    }

    public void multiSetFrom(int f)
    {
        if (multiplayer)
            socket.local_fromPos = f;
    }

    public void multiSetTo(int to)
    {
        if (multiplayer)
            socket.local_toPos = to;
    }

    public void multiSetNotFoundSwitched(bool f)
    {
        if (multiplayer)
            socket.local_notFoundSwitched = f;
    }


    private Slot slotScript;
    private Card cardScript;
    private Card cardScriptDest;
    private Boost boostScript;
    public void multiEquiBoost(int from)
    {
        from = 63 - from;
        slotScript = board[from].GetComponent<Slot>();
        cardScript = slotScript.placedCard.GetComponent<Card>();
        boostScript = opponentTerminalCards[2].GetComponent<Boost>();
        boostScript.startActivationB(cardScript);
    }


    public void multiDisableBoost()
    {
        boostScript = opponentTerminalCards[2].GetComponent<Boost>();
        boostScript.disableTerminalCard(false);
    }


    public void multiMouve(int fromPos, int toPos)
    {
        fromPos = 63 - fromPos;
        toPos = 63 - toPos;
        slotScript = board[fromPos].GetComponent<Slot>();
        cardScript = slotScript.placedCard.GetComponent<Card>();
        slotScript = board[toPos].GetComponent<Slot>();

        if (slotScript.placedCard == null)
            cardScript.Mouve(false, slotScript, opponentStackLink, opponentStackVirus);
        else
        {
            Debug.Log("KILL");
            cardScriptDest = slotScript.placedCard.GetComponent<Card>();
            cardScript.Mouve(true, slotScript, opponentStackLink, opponentStackVirus);
            if (cardScriptDest.type.Equals("Link"))
                opponentLinkCollected++;
            else
                opponentVirusCollected++;
        }
            
    }


    public void multiExit(int fromPos)
    {
        fromPos = 63 - fromPos;
        slotScript = board[fromPos].GetComponent<Slot>();
        cardScript = slotScript.placedCard.GetComponent<Card>();


        cardScript.stackCard(cardScript,opponentStackLink,opponentStackVirus,true);
        if (cardScript.type.Equals("Link-B"))
            opponentLinkCollected++;
        else
            opponentVirusCollected++;
        cardScript.showCard();

    }

    VirusChecker virusChecker;
    public void multiVirusChecker(int fromPos)
    {
        fromPos = 63 - fromPos;

        slotScript = board[fromPos].GetComponent<Slot>();
        virusChecker = opponentTerminalCards[1].GetComponent<VirusChecker>();
        virusChecker.startActivationB(slotScript.placedCard);
        player.checkedCard = slotScript.placedCard;

    }


    Reset reset;
    public void multiReset(int fromPos)
    {
        fromPos = 63 - fromPos;

        slotScript = board[fromPos].GetComponent<Slot>();
        reset = opponentTerminalCards[4].GetComponent<Reset>();
        reset.startActivationB(slotScript.placedCard);

    }


    public Material hideCard;
    public void multiNotFound(int posCard1, int posCard2, bool switched)
    {
        Card cardScript2;
        Slot slotScript2;


        posCard1 = 63 - posCard1;
        posCard2 = 63 - posCard2;

        slotScript = board[posCard1].GetComponent<Slot>();
        slotScript2 = board[posCard2].GetComponent<Slot>();

        cardScript = slotScript.placedCard.GetComponent<Card>();
        cardScript2 = slotScript2.placedCard.GetComponent<Card>();

        cardScript.hideCard();
        cardScript2.hideCard();

        Card boostCard = null;
        if (cardScript.boost != null)
        {
            cardScript.disableBoost();
            boostCard = cardScript2;
        }
        else if (cardScript2.boost != null)
        {
            cardScript2.disableBoost();
            boostCard = cardScript;
        }

        if (switched)
        {
            int posTemp = posCard1;
            posCard1 = posCard2;
            posCard2 = posTemp;
        }

        cardScript.MouveOnHand();
        cardScript2.MouveOnHand();

        slotScript = board[posCard1].GetComponent<Slot>();
        slotScript2 = board[posCard2].GetComponent<Slot>();

        GameObject.FindWithTag("MainCamera").GetComponent<CameraMouvement>().zoonTo(null, switchSp, true);
        StartCoroutine(PlaceCard(slotScript,slotScript2, cardScript, cardScript2));

        if(boostCard != null)
        {
            boostCard.equipBoost(opponentTerminalCards[2]);
        }

    }


    IEnumerator PlaceCard(Slot s1, Slot s2, Card c1, Card c2)
    {
        yield return new WaitForSeconds(2.0f);
        c1.Mouve(false, s1, null, null, true);
        c1.restoreRotation();
        s1.placedCard = c1.gameObject;


        c2.Mouve(false, s2, null, null, true);
        c2.restoreRotation();
        s2.placedCard = c2.gameObject;
    }


    Firewall firewall;
    public void multiEnableFirewall(int from)
    {
        from = 63 - from;

        firewall = opponentTerminalCards[3].GetComponent<Firewall>();
        firewall.startActivationB(board[from]);
    }


    public void multiDisableFirewall(int from)
    {
        from = 63 - from;
        Debug.Log("DISABLE FIREWALL");

        firewall = opponentTerminalCards[3].GetComponent<Firewall>();
        firewall.disableTerminalCard(false);
    }
}
