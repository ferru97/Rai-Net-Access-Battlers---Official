using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class GameMenu : MonoBehaviour {

    public GameObject gamePanel;
    public GameManager manager;
    MatchtSocket socket;

    public void Exit()
    {
        Debug.Log("EXIT");
        socket = GameObject.FindWithTag("NetSocket").GetComponent<MatchtSocket>();
        socket.emitExit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        
    }

    public void Continue()
    {
        gameObject.SetActive(false);
    }

    public void GamePanel()
    {
        gamePanel.SetActive(true);
    }


}
