using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtificialIntelligence : MonoBehaviour {

    private GameManager manager;
    private int placedCards = 0;

    private Slot slotScript;
    private Card cardScript;

    public GameObject[] installationArea = new GameObject[8];
    public GameObject[] Card = new GameObject[8];

    public GameObject[] stackAreaVirus = new GameObject[4];
    public GameObject[] stackAreaLink = new GameObject[4];

    public Player player;

    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (PlayerPrefs.GetInt("Multiplayer") != 0)
            Destroy(gameObject);
        manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        RandomizeCard();
    }

    // Update is called once per frame
    int cardIndex;
    Slot dest;
    Card opponentCard;
    bool canExit = false;
    bool chackBack = false;
    int waitTime = -1;

    void Update () {

        if (!manager.endGame)
        {
            if(placedCards < 8) {
                placementRound();
            }
            else{              
                if(manager.roundPlayer == 'B')
                {
                    if (waitTime == -1)
                        StartCoroutine(Wait());

                    if(waitTime == 1)
                    {
                        RandomizeCard();                   
                        dest = null;
                        cardIndex = 0;
                        while ( dest == null && !canExit)
                        {
                            if (cardIndex == 8)
                            {
                                chackBack = true;
                                cardIndex = 0;
                            }                           

                            cardScript = Card[cardIndex].GetComponent<Card>();
                            if (!cardScript.cardPosition.tag.Equals("Stack"))
                            {
                                checkExit();
                                if(!canExit)
                                  dest = calculateMouve();

                                if (dest != null || canExit)
                                {
                                    if (canExit)
                                    {
                                        cardScript.stackCard(cardScript, stackAreaLink, stackAreaVirus, true);
                                        manager.opponentLinkCollected++;                                    
                                    }
                                    else
                                    {
                                        if (dest.placedCard != null)
                                        {
                                            opponentCard = dest.placedCard.GetComponent<Card>();
                                            if (opponentCard.type.Equals("Virus") || opponentCard.type.Equals("Link"))
                                            {
                                                cardScript.Mouve(true, dest, stackAreaLink, stackAreaVirus);
                                                if (opponentCard.type.Equals("Virus"))
                                                    manager.opponentVirusCollected++;
                                                else
                                                    manager.opponentLinkCollected++;
                                            }
                                        }
                                        else
                                            cardScript.Mouve(false, dest, null, null);
                                    }
                                
                                }
                            }                        
                            cardIndex++;                       
                        }
                        canExit = false;
                        chackBack = false;
                        waitTime = -1;
                        manager.nextRound();
                    }                
                }
            }
        }

        if (manager.endGame)
        {
            if (manager.playerLinkCollected == 4 || manager.opponentVirusCollected == 4)
                Debug.Log("AI Perso");
            else if (manager.opponentLinkCollected == 4 || manager.playerVirusCollected == 4)
                Debug.Log("AI Vinto");
        }

    }

    void RandomizeCard()
    {
        for (int i = 7; i > 0; i--)
        {
            int r = Random.Range(0, i+1);
            GameObject tmp = Card[i];
            Card[i] = Card[r];
            Card[r] = tmp;
        }
    }


    Slot result;
    int shift;
    Slot calculateMouve()
    {
        slotScript = cardScript.cardPosition.GetComponent<Slot>();

        //back
        if (chackBack && slotScript.boardCoords + 8 < 64)
        {
            if (manager.board[slotScript.boardCoords + 8].GetComponent<Slot>().placedCard != null)
                opponentCard = manager.board[slotScript.boardCoords + 8].GetComponent<Slot>().placedCard.GetComponent<Card>();
            else
                opponentCard = null;

            if (opponentCard == null || (!opponentCard.type.Equals("Virus-B") && !opponentCard.type.Equals("Link-B")))
                return result = manager.board[slotScript.boardCoords + 8].GetComponent<Slot>();
        }

        //try next
        if (slotScript.boardCoords - 8 >= 0)
        {
            result = manager.board[slotScript.boardCoords - 8].GetComponent<Slot>();
            if (!result.isFirewalledB() && 
                (result.placedCard==null || (!result.placedCard.GetComponent<Card>().type.Equals("Virus-B") && !result.placedCard.GetComponent<Card>().type.Equals("Link-B"))))
                return result;
        }

        shift = slotScript.boardCoords;
        while (shift > 8)
            shift = shift - 8;

        //Try DX
        if ((shift >= 5 || chackBack) && slotScript.boardCoords - 1 < 64)
        {
            if  (manager.board[slotScript.boardCoords - 1].GetComponent<Slot>().placedCard != null)
                opponentCard = manager.board[slotScript.boardCoords - 1].GetComponent<Slot>().placedCard.GetComponent<Card>();
            else
                opponentCard = null;
        
            if (opponentCard == null || (!opponentCard.type.Equals("Virus-B") && !opponentCard.type.Equals("Link-B")))
                return result = manager.board[slotScript.boardCoords - 1].GetComponent<Slot>();
        }
          
        //Try SX
        if ((shift <= 2 || chackBack) && slotScript.boardCoords + 1 >= 0)
        {
            if ( manager.board[slotScript.boardCoords + 1].GetComponent<Slot>().placedCard != null)                      
                opponentCard = manager.board[slotScript.boardCoords + 1].GetComponent<Slot>().placedCard.GetComponent<Card>();
            else
                opponentCard = null;

            if (opponentCard == null || (!opponentCard.type.Equals("Virus-B") && !opponentCard.type.Equals("Link-B")))
                return result = manager.board[slotScript.boardCoords + 1].GetComponent<Slot>();
        }
        
         return null; //change card
            
    }

    void checkExit()
    {
        //if link try exit
        if (cardScript.type.Equals("Link-B") && (cardScript.cardPosition.GetComponent<Slot>().boardCoords == 3 || cardScript.cardPosition.GetComponent<Slot>().boardCoords == 4))
            canExit = true;
    }


    void placementRound()
    {       
        bool placed = false;
        while (!placed)
        {
            int pos;
            pos = Random.Range(0,8);
            slotScript = installationArea[pos].GetComponent<Slot>();
            cardScript = Card[placedCards].GetComponent<Card>();

            if (slotScript.placedCard == null)
            {
                Card[placedCards].GetComponent<Card>().Mouve(false, slotScript, null, null);
                placedCards++;
                placed = true;
            }
        }
        if (placedCards < 8)
            placementRound();
        else
            manager.cardPlaced = true;
    }

    private IEnumerator Wait()
    {
        waitTime = 0;
        if (!player.lastActivationTerminal)
        {
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            player.lastActivationTerminal = false;
            yield return new WaitForSeconds(4f);
        }      
        waitTime = 1;
    }
}
