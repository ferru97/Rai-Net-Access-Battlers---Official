using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : SuperCard
{

    public GameObject card = null;
    private Card cardScript = null;
    private Slot slotScript;

    public Sprite splashSprite;
    public Sprite splashSpriteDis;

    public Material defaultMat;
    public Material disabledMat;

    private bool startActivation = false;

    // Use this for initialization
    void Start () {
        initializeCard();
        enableTip = "Enable Line Boost?";
        disableTip = "Disable Line Boost?";
    }

    // Update is called once per frame
	void Update () {


        if (startActivation && !camera.onZooming)
        {
            startActivation = false;
            camera.onZooming = false;
            activeTerminalCard(cardScript);
        }
        else
        {
            if (selected)
            {
                if (status)
                {
                    player.lastActivationTerminal = true;
                    disableTerminalCard(true);
                    selected = false;
                    AnimCardDown();
                    player.emptySelection();
                }
                else
                {
                    if (player.cardSelected != null)
                    {
                        player.lastActivationTerminal = true;
                        cardScript = player.cardSelected.GetComponent<Card>();
                        slotScript = cardScript.cardPosition.GetComponent<Slot>();

                        camera.onZooming = true;
                        camera.zoonTo(cardScript.gameObject, splashSprite);
                        startActivation = true;

                        manager.multiSetAction("equip-boost");
                        manager.multiSetFrom(slotScript.boardCoords);
                        manager.nextRound();

                        cardScript.AnimCardDown();
                        player.emptySelection();                  
                        AnimCardDown();
                        selected = false;
   
                    }
                }
            }
        }
        

    }

    private void OnMouseDown()
    {
        selectCardMulti(2);
    }

    public void startActivationB(Card c)
    {
        cardScript = c;
        camera.onZooming = true;
        camera.zoonTo(c.gameObject, splashSprite);
        startActivation = true;
    }

    public void activeTerminalCard(Card c)
    {      
        GetComponent<MeshRenderer>().material = disabledMat;
        c.equipBoost(gameObject);
        status = true;
        card = c.gameObject;

        if (type != 'B')
            enableDisable.hideMenu();
    }

    public void disableTerminalCard(bool nextR, bool exit=false)
    {
        cardScript = card.GetComponent<Card>();
        if(!exit)
          camera.zoonTo(cardScript.gameObject, splashSpriteDis);
        GetComponent<MeshRenderer>().material = defaultMat;
        player.terminalCardSelected = null;
        status = false;        
        cardScript.disableBoost();
        card = null;
        
        if (nextR)
        {
            manager.multiSetAction("disable-boost");
            manager.nextRound();
        }
        
    }

}
