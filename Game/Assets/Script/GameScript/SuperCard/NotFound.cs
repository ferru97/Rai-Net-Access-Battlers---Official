using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotFound : SuperCard {

    Slot slotScript;
    Card cardScript;
    public Material disabledMat;
    GameObject card1;
    GameObject card2;
    int c2BoardCoords;
    public int switchC;
    bool used;
    public Sprite splashSprite;

    // Use this for initialization
    void Start () {
        initializeCard();
        enableTip = "Use 404 Not Found?";
        selected = false;
        switchC = 0;
        used = false;
        card1 = null;
        card2 = null;
    }

    // Update is called once per frame
    void Update () {
        if (selected && player.cardSelected != null)
        {
            if (!activated)
                  activated = true;              
            if(card1 == null)
            {
                card1 = player.cardSelected;
                player.cardSelected = null;
            }
            else if (card2 == null)
            {
                card2 = player.cardSelected;
                player.cardSelected = null;
                if (card2 != card1)
                {
                    selected = false;
                    active();
                }
                else
                    card2 = null;
            }

        }
        if (!used && card1!=null && card2!=null && switchC!=0)
        {
            used = true;
            if (switchC == 1)
                endActivation(true);
            else
                endActivation(false);

        }
            
    }

    public void active()
    {
        camera.zoonTo(null,splashSprite,true);
        enableDisable.showMenu(false, "Switch the selected cards?");
        enableDisable.action = 5;       
    }

    
    private void endActivation(bool s)
    {
        player.lastActivationTerminal = true;
        Card c1 = card1.GetComponent<Card>();
        Card c2 = card2.GetComponent<Card>();
        c1.hideCard();
        c2.hideCard();

        GameObject boostObj = null;
        Card boostCard = null;
        if (c1.boost != null)
        {
            boostObj = c1.boost;
            c1.disableBoost();
            boostCard = c2;
            boostObj.GetComponent<Boost>().card = c2.gameObject;
        }
        else if(c2.boost != null)
        {
            boostObj = c2.boost;
            c2.disableBoost();
            boostCard = c1;
            boostObj.GetComponent<Boost>().card = c1.gameObject;
        }
        

        if (s == false)
        {
            c1.AnimCardDown();
            c2.AnimCardDown();
        }
        else
        {
            Slot destTemp = c1.cardPosition.GetComponent<Slot>();
            c1.Mouve(false, c2.cardPosition.GetComponent<Slot>(), null, null);
            c2.Mouve(false, destTemp, null, null);
            
        }

        if (boostCard != null)
        {
            boostCard.equipBoost(boostObj);
        }

        manager.multiSetAction("active-404notfound");
        manager.multiSetFrom(c1.cardPosition.GetComponent<Slot>().boardCoords);
        manager.multiSetTo(c2.cardPosition.GetComponent<Slot>().boardCoords);
        if (s)
            manager.multiSetNotFoundSwitched(true);
        else
            manager.multiSetNotFoundSwitched(false);

        manager.nextRound();
        GetComponent<MeshRenderer>().material = disabledMat;
        AnimCardDown();
        player.emptySelection();

        if (type != 'B')
            enableDisable.hideMenu();
    }



    private void OnMouseDown()
    {
        selectCardSolo(4);
    }
}
