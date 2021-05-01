using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour {

    private Player player;
    public int boardCoords;
    GameManager manager;
    // Use this for initialization
    void Start () {
        manager = manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        //manager.board[boardCoords] = gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnMouseDown()
    {
        if (player.cardSelected != null)
        {
            player.slotSelected = gameObject;
        }

    }
}
