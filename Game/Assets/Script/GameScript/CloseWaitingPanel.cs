using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWaitingPanel : MonoBehaviour
{
    private GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.roundPlayer != 'X')
            gameObject.SetActive(false);
    }
}
