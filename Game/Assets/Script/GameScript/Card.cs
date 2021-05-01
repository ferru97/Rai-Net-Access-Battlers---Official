using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

    public GameObject cardPosition = null;
    public string type;
    private GameManager manager;
    private Player player;
    public GameObject boost = null;
    public Material hideMat;
    public Material showMat;

    public GameObject checkedEye;
    public bool cardShowed = false;

    Animator anim;

    private GameObject camera;
    private Vector3 cardOnHand;

    AudioEffect audioEffect;

    public CameraMouvement cameraPos;

    private MenuEnableDisable menuOpen;

    // Use this for initialization
    void Start () {
        manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        anim = gameObject.GetComponent<Animator>();
        audioEffect = GameObject.FindWithTag("AudioEffect").GetComponent<AudioEffect>();
        menuOpen = GameObject.FindWithTag("LateralPanel").GetComponent<MenuEnableDisable>();
        
        camera = GameObject.FindWithTag("MainCamera");
        cardOnHand = new Vector3(camera.transform.position.x, camera.transform.position.y-2, camera.transform.position.z-0.1f);
        cameraPos = camera.GetComponent<CameraMouvement>();

    }

    // Update is called once per frame
    private bool mouving = false;
    private bool mouvingOnHand = false;
    private int speed = 5;
    private Vector3 dest;
    private bool selected = false;
    void Update () {

        if (player.cardSelected == gameObject)
            selected = true;

        if(selected==true && (player.cardSelected==null || player.cardSelected != gameObject) && (player.terminalCardSelected==null || !player.terminalCardSelected.tag.Equals("404NotFound")))
        {
            AnimCardDown();
            selected = false;
        }

        if (manager.roundPlayer.Equals("B") && anim.GetBool("up") == true)
            AnimCardDown();

        if (mouving)
        {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, dest, speed*Time.deltaTime);
            if (transform.parent.position == dest)
            {
                mouving = false;
                if (mouvingOnHand)
                    mouvingOnHand = false;
                else
                    AnimCardDown();
            }
                
        }



	}

    private Card cardScript;
    private int temp_oord;
    private void OnMouseDown()
    {
        if (manager.roundPlayer == 'A' && !menuOpen.isOpen() )
        {
            if(player.terminalCardSelected!=null && player.terminalCardSelected.tag.Equals("Reset") && (type.Equals("Link-B") || type.Equals("Virus-B")))
            {

                temp_oord = cardPosition.GetComponent<Slot>().boardCoords + 8;
                if(temp_oord >=0 && temp_oord <= 63 && manager.board[temp_oord].GetComponent<Slot>().placedCard == null)
                    player.cardSelected = gameObject;

            }
            else if(manager.cardPlaced && player.placedCards==8)
            {
                if (((!type.Equals("Link-B") && !type.Equals("Virus-B") && (player.terminalCardSelected == null || !player.terminalCardSelected.tag.Equals("VirusChecker"))) 
                    || (player.terminalCardSelected != null && player.terminalCardSelected.tag.Equals("VirusChecker") && (type.Equals("Link-B") || type.Equals("Virus-B"))) 
                    && !cardPosition.gameObject.tag.Equals("Stack")) && (player.terminalCardSelected==null || !player.terminalCardSelected.tag.Equals("Firewall")))
                {
                    if (player.cardSelected != null)
                    {
                        cardScript = player.cardSelected.GetComponent<Card>();
                        cardScript.AnimCardDown();
                    }

                    if (!type.Equals("Link-B") && !type.Equals("Virus-B"))
                        AnimCardUp();

                    player.slotSelected = null;
                    player.cardSelected = gameObject;

                    player.deleteCanMouveTip();
                    if(player.terminalCardSelected==null)
                        player.canMouveTip();
                }
                else
                {
                    if(player.terminalCardSelected==null || (player.terminalCardSelected != null && !player.terminalCardSelected.tag.Equals("Firewall")))
                          player.slotSelected = cardPosition;
                }
               

            }
        }

        
    }


    public void MouveOnHand()
    {
        audioEffect.playDealingCard();
        /*if(cameraPos.getCameraPos()==1)
            gameObject.transform.parent.eulerAngles = new Vector3(25f, 0f, 0f);*/
        dest = cardOnHand;
        mouving = true;
        mouvingOnHand = true;

    }
    public void restoreRotation()
    {
        gameObject.transform.parent.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    private Slot slotScript;
    private Slot oldSlotScript;
    private Card killed;
    public void Mouve(bool kill, Slot destination, GameObject[] link, GameObject[] virus, bool multi404=false)
    {

        if (kill)
        {
            killed = destination.placedCard.GetComponent<Card>();
            if(killed.type.Equals("Link-B") || killed.type.Equals("Virus-B"))
            {             
                killed.AnimShowCard();
            }
            
            stackCard(killed,link, virus,kill);
        }

        if (type.Equals("Link-B") || type.Equals("Virus-B"))
        {
            AnimCardUp();
        }
        dest = destination.transform.position;
        mouving = true;

        if (cardPosition != null &&!multi404)
        {
            oldSlotScript = cardPosition.GetComponent<Slot>(); // Free old slot
            oldSlotScript.placedCard = null;
        }


        destination.placedCard = gameObject;
        cardPosition = destination.gameObject;

    }

    private Boost boostScript;
    private int empty;
    private StackArea stackAreaScript;
    public void stackCard(Card c, GameObject[] link, GameObject[] virus, bool exit = false)
    {
        audioEffect.playCardCollect();
        if (c.cardShowed && (c.type.Equals("Link") || c.type.Equals("Virus")))
            c.deleteEye();

        if (c.boost != null )
        {
            boostScript = c.boost.GetComponent<Boost>();
            boostScript.disableTerminalCard(false,exit);
            c.boost = null;
        }

        oldSlotScript = c.cardPosition.GetComponent<Slot>();
        oldSlotScript.placedCard = null;

        if (c.type.Equals("Link") || c.type.Equals("Link-B"))
        {
            empty = lastEmptyPOsition(link);
            c.transform.parent.position = new Vector3(link[empty].transform.position.x, link[empty].transform.position.y+0.01f, link[empty].transform.position.z);
            stackAreaScript = link[empty].GetComponent<StackArea>();
            stackAreaScript.card = c.gameObject;
            c.cardPosition = link[empty];

        }
        else
        {
            empty = lastEmptyPOsition(virus);
            c.transform.parent.position = new Vector3(virus[empty].transform.position.x, link[empty].transform.position.y + 0.01f, virus[empty].transform.position.z);
            stackAreaScript = virus[empty].GetComponent<StackArea>();
            stackAreaScript.card = c.gameObject;
            c.cardPosition = virus[empty];
        }

        if (exit)
            audioEffect.playCardCollect();
    }


    private bool stop;
    private int pos;
    private int lastEmptyPOsition(GameObject[] array)
    {
        stop = false;
        pos = 0;
        while (pos<4 && !stop)
        {
            stackAreaScript = array[pos].GetComponent<StackArea>();
            if ( stackAreaScript.card == null)
                stop = true;
            else
                pos++;
        }
        return pos;
    }


    public void equipBoost(GameObject boostObj)
    {
        var emission = gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().emission;
        emission.enabled = true;

        boost = boostObj;
        
    }


    public void disableBoost()
    {
        var emission = gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().emission;
        emission.enabled = false;
        boost = null;
    }



    public IEnumerator Wait(float sec)
    {
        yield return new WaitForSeconds(sec);
    }


    public void AnimCardUp()
    {
        audioEffect.playCardSelected();
        anim.SetBool("up", true);
    }

    public void AnimCardDown()
    {   
        
        anim.SetBool("up", false);
    }

    public void AnimShowCard()
    {     
        anim.SetBool("show", true);
    }


    public void showCard()
    {
        GetComponent<MeshRenderer>().material = showMat;
        anim.SetBool("show", false);
    }

    public void hideCard()
    {
        if (cardShowed)
        {
            if (type.Equals("Link") || type.Equals("Virus"))
                deleteEye();
            else
                gameObject.GetComponent<MeshRenderer>().material = hideMat;
            cardShowed = false;
        }
    }


    public void InstanziateEye()
    {
        GameObject temp = Instantiate(checkedEye, transform);
        temp.tag = "eye";
        cardShowed = true;
    }


    public void deleteEye()
    {
        Destroy(gameObject.transform.GetChild(1).gameObject);
        cardShowed = false;
    }

}
