using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewall : SuperCard
{

    Slot slotScript;

    public Material defaultMat;
    public Material disabledMat;


    public Sprite splashSprite;
    public Sprite splashSpriteDis;

    private bool startActivation = false;

    // Use this for initialization
    void Start()
    {
        initializeCard();
        enableTip = "Enable Firewall?";
        disableTip = "Disable Firewall?";
    }

    // Update is called once per frame
    void Update()
    {

        if (startActivation && !camera.onZooming)
        {
            startActivation = false;
            camera.onZooming = false;
            enableTerminalCard(slotScript.gameObject);
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
                    if (player.slotSelected != null)
                    {
                        player.lastActivationTerminal = true;
                        camera.onZooming = true;
                        camera.zoonTo(player.slotSelected, splashSprite);
                        startActivation = true;

                        slotScript = player.slotSelected.GetComponent<Slot>();
                        manager.multiSetAction("enable-firewall");
                        manager.multiSetFrom(slotScript.boardCoords);
                        manager.nextRound();

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
        selectCardMulti(1);
    }

    public void startActivationB(GameObject slot)
    {
        slotScript = slot.GetComponent<Slot>();
        camera.onZooming = true;
        camera.zoonTo(slot, splashSprite);
        startActivation = true;
    }


    public void enableTerminalCard(GameObject slot)
    {
        GetComponent<MeshRenderer>().material = disabledMat;
        slotScript = slot.GetComponent<Slot>();

        if (type == 'A')
            slotScript.enableFriwallA();
        else
            slotScript.enableFriwallB();
        status = true;

        if (type != 'B')
            enableDisable.hideMenu();
    }


    public void disableTerminalCard(bool nextR)
    {
        camera.zoonTo(slotScript.gameObject, splashSpriteDis);
        GetComponent<MeshRenderer>().material = defaultMat;
        player.terminalCardSelected = null;
        status = false;
        if (type == 'A')
            slotScript.disableFirewallA();
        else
            slotScript.disableFirewallB();

        manager.multiSetAction("disable-firewall");
        manager.multiSetFrom(slotScript.boardCoords);
        if (nextR)
        {
            manager.nextRound();
        }

    }


}
