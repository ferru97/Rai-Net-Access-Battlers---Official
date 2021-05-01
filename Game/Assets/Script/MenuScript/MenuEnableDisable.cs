using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuEnableDisable : MonoBehaviour {

    public GameObject Menu;
    public GameObject FirewallGO;
    public GameObject BoostGO;
    public GameObject VirusCheckerGO;
    public GameObject NotFoundGO;
    public int action;
    public GameObject titleGO;
    public GameObject playerGO;
    public GameObject gmGO;
    public GameObject tipTextGO;
    public GameObject btnYES;
    public GameObject btnNO;
    public GameObject resetGO;
    public GameObject manager;

    Firewall firewall;
    Boost boost;
    VirusChecker vCheck;
    NotFound notFound;
    GameManager gm;
    Text title;
    Text tipText;
    Player player;
    Reset reset;

    Animator anim;

    bool askOpen;


    // Use this for initialization

    void Awake()
    {
        firewall = FirewallGO.GetComponent<Firewall>();
        boost = BoostGO.GetComponent<Boost>();
        vCheck = VirusCheckerGO.GetComponent<VirusChecker>();
        notFound = NotFoundGO.GetComponent<NotFound>();
        player = playerGO.GetComponent<Player>();
        title = titleGO.GetComponent<Text>(); ;
        gm = gmGO.GetComponent<GameManager>();
        anim = Menu.GetComponent<Animator>();
        tipText = tipTextGO.GetComponent<Text>();
        reset = resetGO.GetComponent<Reset>();
    }


    void Start () {      
        action = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void yes()
    {
        askOpen = false;
        switch (action)
        {
            case 1:
                player.slotSelected = null;
                firewall.selected = true;
                hideMenu();
                if(!firewall.status)
                  showMenu(true, "Select an empty slot for the Firewall");
                break;
            case 2:
                player.cardSelected = null;
                boost.selected = true;
                hideMenu();
                if(!boost.status)
                    showMenu(true,"Select a card");
                break;
            case 3:
                player.cardSelected = null;
                vCheck.selected = true;
                hideMenu();
                showMenu(true, "Select an enemy card to reveal");
                break;
            case 4:
                if (player.cardSelected != null)
                {
                    player.cardSelected = null;
                    player.cardSelected.GetComponent<Card>().AnimCardDown();
                }
                notFound.selected = true;
                hideMenu();
                showMenu(true, "Select two cards");
                break;
            case 5:
                hideMenu();
                NotFoundGO.GetComponent<NotFound>().switchC = 1;
                break;
            case 6:
                player.cardSelected = null;
                reset.selected = true;
                hideMenu();
                showMenu(true, "Select an opponent card to Reset");
                break;
        }

        action = 0; 
    }

    public void no()
    {
        askOpen = false;
        switch (action)
        {
            case 1:
                firewall.AnimCardDown();
                break;
            case 2:
                boost.AnimCardDown();
                break;
            case 3:
                vCheck.AnimCardDown();
                break;
            case 4:
                notFound.AnimCardDown();
                break;
            case 5:
                notFound.switchC = 2;
                break;
            case 6:
                reset.AnimCardDown();
                break;
        }

        action = 0;

        player.emptySelection();
        hideMenu();
    }



    public void showMenu(bool tip,string titleSTR)
    {
        if (!tip)
        {
            askOpen = true;
            tipTextGO.SetActive(false);

            title.gameObject.SetActive(true);                   
            btnNO.SetActive(true);
            btnYES.SetActive(true);
            title.text = titleSTR;
        }
        else
        {
            askOpen = false;
            title.gameObject.SetActive(false);
            btnNO.SetActive(false);
            btnYES.SetActive(false);

            tipTextGO.SetActive(true);
            tipText.text = titleSTR;
        }
        anim.SetBool("show", true);
    }

    public void hideMenu()
    {
        askOpen = false;
        anim.SetBool("show", false);
    }

    public bool isOpen()
    {
        return askOpen;
    }

}
