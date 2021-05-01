using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

    public GameObject placedCard = null;
    public string type;
    public int boardCoords;

    public Player player;
    GameManager manager;

    public bool firewallA = false;
    public bool firewallB = false;
    public GameObject FirewallCubeA;
    public GameObject FirewallCubeB;
    public GameObject FirewallCubeAB;

    public GameObject canMouveObj;
    public bool isTip = false;



    // Use this for initialization
    void Start () {

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        manager = manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        manager.board[boardCoords] = gameObject;
        canMouveObj = (GameObject)Resources.Load("CanMouveTip", typeof(GameObject));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        if(player.cardSelected != null || (placedCard == null && player.terminalCardSelected!=null
            && player.terminalCardSelected.tag.Equals("Firewall") && !gameObject.tag.Equals("Exit")))
        {
            player.slotSelected = gameObject;
        }
       
    }

    //Animazioni Firewal
    public void enableFriwallA()
    {
        firewallA = true;
        if (firewallB)
        {
            Destroy(gameObject.transform.GetChild(0).gameObject);
            Instantiate(FirewallCubeAB, transform);
        }
        else
        {
            Instantiate(FirewallCubeA, transform);
        }
            
    }
    public void disableFirewallA()
    {
        firewallA = false;
        Destroy(gameObject.transform.GetChild(0).gameObject);
        if (firewallB)
            Instantiate(FirewallCubeB, transform);

    }


    public void enableFriwallB()
    {
        firewallB = true;
        if (firewallA)
        {
            Destroy(gameObject.transform.GetChild(0).gameObject);
            Instantiate(FirewallCubeAB, transform);
        }
        else
        {
            Instantiate(FirewallCubeB, transform);
        }
    }
    public void disableFirewallB()
    {
        firewallB = false;
        Destroy(gameObject.transform.GetChild(0).gameObject);
        if (firewallA)
            Instantiate(FirewallCubeA, transform);

    }


    public bool isFirewalled()
    {
        if (firewallB)
            return true;
        else
            return false;
    }

    public bool isFirewalledB()
    {
        if (firewallA)
            return true;
        else
            return false;
    }


    public void spownTip()
    {
        if (!isTip)
        {
            Instantiate(canMouveObj,transform);
            isTip = true;
        }
        
    }

    int c;
    public void deleteTip()
    {
        c = 0;
        if (isTip == true)
        {
            while (isTip)
            {
                if (gameObject.transform.GetChild(c).gameObject.tag.Equals("TipSpown"))
                {
                    Destroy(gameObject.transform.GetChild(c).gameObject);
                    isTip = false;
                }
                c++;
                    
            }
        }
        
    }

}
