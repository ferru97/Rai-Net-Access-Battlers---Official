using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperCard : MonoBehaviour
{

    public char type;
    public GameObject menuEnablerDisabler;
    public bool selected = false;
    public bool status = false;

    protected MenuEnableDisable enableDisable;
    protected GameManager manager;
    protected Player player;
    protected CameraMouvement camera;
    protected Animator anim;

    protected string enableTip;
    protected string disableTip;

    protected bool activated;

    // Start is called before the first frame update

    protected void initializeCard()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        anim = gameObject.GetComponent<Animator>();
        camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraMouvement>();
        if (type != 'B')
            enableDisable = menuEnablerDisabler.GetComponent<MenuEnableDisable>();
    }

    protected void selectCardMulti(int enableDisableAction)
    {
        if (manager.roundPlayer == 'A' && manager.cardPlaced && !selected && player.terminalCardSelected == null && player.placedCards==8)//!!
        {
            enableDisable.action = enableDisableAction;
            if (!status)
                enableDisable.showMenu(false, enableTip);
            else
                enableDisable.showMenu(false, disableTip);

            player.emptySelection();
            player.terminalCardSelected = gameObject;
            AnimCardUp();
        }
    }

    protected void selectCardSolo(int enableDisableAction)
    {
        if (manager.roundPlayer == 'A' && manager.cardPlaced && !activated && player.terminalCardSelected == null)//!!
        {
            enableDisable.action = enableDisableAction;
            enableDisable.showMenu(false, enableTip);

            player.emptySelection();
            player.terminalCardSelected = gameObject;

            AnimCardUp();
        }
    }

    public void AnimCardUp()
    {
        anim.SetBool("up", true);
    }

    public void AnimCardDown()
    {
        anim.SetBool("up", false);
    }

}
