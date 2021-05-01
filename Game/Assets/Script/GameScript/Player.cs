using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private GameManager manager;
    public int placedCards = 0;

    private Slot slotScript;
    private Slot oldSlotScript;
    private Card cardScript;
    private Card opponentcardScript2;

    public GameObject slotSelected = null;
    public GameObject cardSelected = null;

    public GameObject[] stackAreaVirus = new GameObject[4];
    public GameObject[] stackAreaLink = new GameObject[4];
    public GameObject[] IAslots = new GameObject[8];

    private StackArea stackAreaScript;

    public GameObject[] Cards = new GameObject[12];

    public GameObject terminalCardSelected = null;
    private int newCoords;

    VirusChecker virusChecker;
    public GameObject checkedCard = null;

    NotFound notFound;

    Firewall firewall;

    bool firstRound = true;
    bool firstReady = true;
    bool automouve = false;

    public GameObject canExitTip;

    public bool lastActivationTerminal = false;
  

    // Use this for initialization
    void Start () {
        manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!manager.connection_break && manager.opponentReady && terminalCardSelected==null && !manager.endGame)
        {
            if (placedCards < 8)
            {
                if (firstReady)
                {
                    firstReady = false;
                    manager.startCounter();
                }
                placementRound();
            }
            else
            {
                if (manager.cardPlaced && !automouve)
                {                  

                    if (firstRound)
                    {
                        if (manager.roundPlayer == 'A')
                            manager.startCounter();
                        firstRound = false;
                    }

                    if (manager.roundPlayer == 'A')
                    {
                        if (cardSelected != null && slotSelected != null)
                        {
                            deleteCanMouveTip();                              
                            cardScript = cardSelected.GetComponent<Card>();
                            slotScript = slotSelected.GetComponent<Slot>();
                            oldSlotScript = cardScript.cardPosition.GetComponent<Slot>();

                            if (slotSelected.tag.Equals("ServerB"))
                            {
                                if (canExit())
                                {
                                    manager.multiSetAction("exit");
                                    manager.multiSetFrom(oldSlotScript.boardCoords);

                                    cardScript.stackCard(cardScript, stackAreaLink, stackAreaVirus,true);
                                    if (cardScript.type.Equals("Virus"))
                                        manager.playerVirusCollected++;
                                    else
                                        manager.playerLinkCollected++;
                                    manager.nextRound();
                                }

                            }
                            else
                            {
                                if (canMouve())
                                {
                                    manager.multiSetAction("mouve");
                                    manager.multiSetFrom(oldSlotScript.boardCoords);
                                    manager.multiSetTo(slotScript.boardCoords);

                                    if (slotScript.placedCard != null)
                                    {
                                        opponentcardScript2 = slotScript.placedCard.GetComponent<Card>();
                                        if (opponentcardScript2.type.Equals("Virus-B") || opponentcardScript2.type.Equals("Link-B"))
                                        {
                                            cardScript.Mouve(true, slotScript, stackAreaLink, stackAreaVirus);
                                            if (opponentcardScript2.type.Equals("Virus-B"))
                                                manager.playerVirusCollected++;
                                            else
                                                manager.playerLinkCollected++;
                                            manager.nextRound();
                                        }
                                    }
                                    else
                                    {
                                        cardScript.Mouve(false, slotScript, stackAreaLink, stackAreaVirus);
                                        manager.nextRound();
                                    }

                                }
                            }
                            cardSelected = null;
                            slotSelected = null;

                        }
                    }
                }
                

            }
        }   

    }

    int posCheck;
    public void canMouveTip()
    {
        posCheck = 0;
        while(posCheck < 64)
        {
            if (canMouve(manager.board[posCheck].GetComponent<Slot>()))
                manager.board[posCheck].GetComponent<Slot>().spownTip();
            posCheck++;
        }
        if (canExit() && !canExitTip.active)
            canExitTip.SetActive(true);

    }

    public void deleteCanMouveTip()
    {
        posCheck = 0;
        while (posCheck < 64)
        {
            manager.board[posCheck].GetComponent<Slot>().deleteTip();
            posCheck++;
        }
        if (canExitTip.active)
            canExitTip.SetActive(false);

    }


    bool canExit()
    {
        if (oldSlotScript.type.Equals("Exit"))
        {
            return true;
        }
        else
        {
            if (cardScript.boost != null){
                if (oldSlotScript.boardCoords==51 || oldSlotScript.boardCoords == 52)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }


    int limit;
    int noJumpBoost;
    bool canMouve(Slot check=null)
    {
        if (check != null)
            slotScript = check;
        noJumpBoost = -1;

        cardScript = cardSelected.GetComponent<Card>();
        oldSlotScript = cardScript.cardPosition.GetComponent<Slot>();
        limit = oldSlotScript.boardCoords;

        while (limit >= 8)
            limit -= 8;

        if (((slotScript.boardCoords == oldSlotScript.boardCoords+8) || (slotScript.boardCoords == oldSlotScript.boardCoords -8 ) ||
            (slotScript.boardCoords == oldSlotScript.boardCoords + 1) || (slotScript.boardCoords == oldSlotScript.boardCoords -1)) && !slotScript.isFirewalled())
        {
            if (slotScript.boardCoords == oldSlotScript.boardCoords + 1 && limit == 7)
                return false;

            if (slotScript.boardCoords == oldSlotScript.boardCoords - 1 && limit == 0)
                return false;


            if (slotScript.placedCard != null)
            {
                opponentcardScript2 = slotScript.placedCard.GetComponent<Card>();
                if (opponentcardScript2.type.Equals("Virus-B") || opponentcardScript2.type.Equals("Link-B"))
                    return true;
                else
                    return false;
            }
            else
                 return true;
        }
        else{
            if (cardScript.boost!=null )
            {
                if (((slotScript.boardCoords == oldSlotScript.boardCoords + 16) || (slotScript.boardCoords == oldSlotScript.boardCoords - 16) ||
                    (slotScript.boardCoords == oldSlotScript.boardCoords + 2) || (slotScript.boardCoords == oldSlotScript.boardCoords - 2)  ||
                    (slotScript.boardCoords == oldSlotScript.boardCoords + 7) || (slotScript.boardCoords == oldSlotScript.boardCoords + 9)  ||
                    (slotScript.boardCoords == oldSlotScript.boardCoords - 7) || (slotScript.boardCoords == oldSlotScript.boardCoords - 9) ) && !slotScript.isFirewalled())
                {
                    if (slotScript.boardCoords == oldSlotScript.boardCoords + 2 && limit >= 6)
                        return false;

                    if (slotScript.boardCoords == oldSlotScript.boardCoords - 2 && limit <= 1)
                        return false;

                    //noJumpBoost position where may be a firewall
                    if (slotScript.boardCoords == oldSlotScript.boardCoords + 16)
                        noJumpBoost = oldSlotScript.boardCoords + 8;

                    if (slotScript.boardCoords == oldSlotScript.boardCoords - 16)
                        noJumpBoost = oldSlotScript.boardCoords - 8;

                    if (slotScript.boardCoords == oldSlotScript.boardCoords + 2)
                        noJumpBoost = oldSlotScript.boardCoords + 1;

                    if (slotScript.boardCoords == oldSlotScript.boardCoords - 2)
                        noJumpBoost = oldSlotScript.boardCoords - 1;


                    if (noJumpBoost>=0 && (manager.board[noJumpBoost].GetComponent<Slot>().firewallA || manager.board[noJumpBoost].GetComponent<Slot>().firewallB))
                        return false;

                    if (slotScript.placedCard != null)
                    {
                        opponentcardScript2 = slotScript.placedCard.GetComponent<Card>();
                        if (opponentcardScript2.type.Equals("Virus-B") || opponentcardScript2.type.Equals("Link-B"))
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        
    }


    void placeCard()
    {
        cardScript.Mouve(false, slotScript, null, null);
        //cardScript.restoreRotation();
        placedCards++;
        manager.multiUpdatePlacedPos("-" + slotScript.boardCoords.ToString());
        cardSelected = null;
        slotSelected = null;
    }
    void placementRound()
    {

        if (cardSelected == null)
        {         
            cardSelected = Cards[placedCards];
            cardScript = cardSelected.GetComponent<Card>();
            cardScript.MouveOnHand();

        }
        else
        {
            if (slotSelected != null)
            {
                slotScript = slotSelected.GetComponent<Slot>();         
                if (slotScript.placedCard == null && slotScript.type == "IA")
                    placeCard();
                else
                    slotSelected = null;
            }
        }
        if (placedCards == 8)
        {
            cardSelected = null;
            endPlacementRound();
        }
            
    }

    void endPlacementRound()
    {
        manager.stopCounter();
        if (manager.cardPlaced)
            manager.setInfoText("Round 0");
        //Animazione moneta
        manager.setStart();
    }


    public void endCounter()
    {
        if (placedCards < 8)
            autoPlacementRound();
        else
            autoMouveCard();
    }


    void autoPlacementRound()
    {
        int c = 0;
        int k = 0;
        bool done;

        while (c < 8)
        {
            cardScript = Cards[c].GetComponent<Card>();
            if(cardScript.cardPosition == null)
            {
                done = false;
                while(k<8 && !done)
                {
                    slotScript = IAslots[k].GetComponent<Slot>();
                    if(slotScript.placedCard == null)
                    {
                        cardSelected = cardScript.gameObject;
                        slotSelected = slotScript.gameObject;
                        placeCard();
                        done = true;
                    }
                    k++;
                }
            }
            c++;
        }
        endPlacementRound();
    }


    void autoMouveCard()
    {
        if(manager.roundPlayer == 'A')
        {
            bool mouved = false;
            automouve = true;
            int k = 0;
            int s;
            RandomizeCard();
            while (!mouved && k<8)
            {                             
                cardSelected = Cards[k];
                cardScript = cardSelected.GetComponent<Card>();
                if (!cardScript.cardPosition.tag.Equals("Stack"))
                {
                    s = 63;
                    while (s >= 0 && !mouved)
                    {
                        slotSelected = manager.board[s];
                        slotScript = slotSelected.GetComponent<Slot>();
                        if (canMouve())
                        {
                            automouve = false;
                            mouved = true;
                        }
                        else
                            s--;
                    }
                }
                k++;
            }
        }
    }


    void RandomizeCard()
    {
        for (int i = 7; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            GameObject tmp = Cards[i];
            Cards[i] = Cards[r];
            Cards[r] = tmp;
        }
    }


    public void emptySelection()
    {
        terminalCardSelected = null;
        cardSelected = null;
        slotSelected = null;
        deleteCanMouveTip();
    }

}
