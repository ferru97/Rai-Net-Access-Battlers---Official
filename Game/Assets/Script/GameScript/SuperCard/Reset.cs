using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : SuperCard {
    Card cardScript;
    Slot slotScript;

    public Material disabledMat;
    public Sprite splashSprite;
    private bool startActivation = false;

    // Use this for initialization
    void Start () {
        initializeCard();
        enableTip = "Enable Reset?";
        selected = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(startActivation && !camera.onZooming)
        {
            startActivation = false;
            camera.onZooming = false;
            active(cardScript.gameObject);
        }
        else
        {
            if (selected && player.cardSelected != null)
            {
                cardScript = player.cardSelected.GetComponent<Card>();
                slotScript = cardScript.cardPosition.GetComponent<Slot>();
                if (cardScript.type.Equals("Link-B") || cardScript.type.Equals("Virus-B"))
                {
                    player.lastActivationTerminal = true;
                    camera.onZooming = true;
                    camera.zoonTo(cardScript.gameObject, splashSprite);
                    startActivation = true;

                    manager.multiSetAction("active-reset");
                    manager.multiSetFrom(slotScript.boardCoords);
                    manager.nextRound();

                    player.emptySelection();
                    AnimCardDown();
                    selected = false;
                }
            }
        }
        
    }


     public void startActivationB(GameObject card)
    {
        cardScript = card.GetComponent<Card>();
        camera.onZooming = true;
        camera.zoonTo(card, splashSprite);
        startActivation = true;
    }

    private int slot_mouve;
    public void active(GameObject card)
    {

        GetComponent<MeshRenderer>().material = disabledMat;
        activated = true;
        cardScript = card.GetComponent<Card>();

        //Mouve
        if (type != 'B')
            slot_mouve = cardScript.cardPosition.GetComponent<Slot>().boardCoords + 8;
        else
            slot_mouve = cardScript.cardPosition.GetComponent<Slot>().boardCoords - 8;

        cardScript.Mouve(false, manager.board[slot_mouve].GetComponent<Slot>(), null, null);

        if (type != 'B')
            enableDisable.hideMenu();

    }


    private void OnMouseDown()
    {
        selectCardSolo(6);
    }
}
