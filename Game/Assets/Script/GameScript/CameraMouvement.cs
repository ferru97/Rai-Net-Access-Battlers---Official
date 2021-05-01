using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class CameraMouvement : MonoBehaviour
{
    private const float ZOOMY_OFFSET = 1.8f;
    private const float ZOOMZ_OFFSET = +0.5f;
    private Vector3 DEFAULT_POS1 = new Vector3(-1.13f, 5.58f, 2.94f);
    private Vector3 DEFAULT_ROT_1 = new Vector3(62, 179, -1);
    private Vector3 DEFAULT_POS2 = new Vector3(-1.18f, 6.29f, 0.5f);
    private Vector3 DEFAULT_ROT_2 = new Vector3(90f, 179f, -1f);
    private Vector3 dest;
    private const int speed = 6;
    private bool mouve = false;
    private bool mouve2 = false;
    private bool stopped = false;
    private Vector2 imgMaxScale = new Vector2(120f,120f);
    private Vector2 imgMinScale = new Vector2(0f,0f);
    private Vector2 toScalse;
    private  RectTransform immObj;
    private Image immSprite;
    private Vector3 DEFAULT_POS;
    public GameObject audioEffect;
    public bool onZooming = false;

    public float fixedHorizontalFOV = 26;
    // Use this for initialization
    void Start()
    {
        immObj = GameObject.FindWithTag("TerminalSplash").GetComponent<RectTransform>();
        immSprite = GameObject.FindWithTag("TerminalSplash").GetComponent<Image>();
        DEFAULT_POS = gameObject.transform.position;

        Debug.Log(Application.platform);
        if (Application.platform == RuntimePlatform.Android)
        {
            GetComponent<Camera>().fieldOfView = 2 * Mathf.Atan(Mathf.Tan(fixedHorizontalFOV * Mathf.Deg2Rad * 0.5f) / GetComponent<Camera>().aspect) * Mathf.Rad2Deg;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mouve)
        {           
            transform.position = Vector3.MoveTowards(transform.position, dest, speed * Time.deltaTime);
            immObj.sizeDelta = Vector3.Lerp(immObj.sizeDelta, toScalse, speed * Time.deltaTime);
            if (transform.position == dest)
            {
                mouve = false;
                onZooming = false;
                if (dest != DEFAULT_POS)
                    StartCoroutine(restore());
                else
                    immObj.sizeDelta = imgMinScale;
            }
        }

        if (mouve2)
        {
            immObj.sizeDelta = Vector3.Lerp(immObj.sizeDelta, toScalse, speed * Time.deltaTime);
            if (!stopped)
            {
                stopped = true;
                StartCoroutine(restore2());
            }
        }
       
    }


    public void zoonTo(GameObject obj, Sprite splash, bool onlySplash=false)
    {
        immSprite.sprite = splash;
        toScalse = imgMaxScale;
        if (onlySplash)
        {
            toScalse = new Vector2(50f, 50f); ;
            mouve2 = true;
        }
        else
        {
            if (obj != null)
            {
                if (getCameraPos() == 1)
                    dest = new Vector3(obj.transform.position.x, obj.transform.position.y + ZOOMY_OFFSET, obj.transform.position.z + ZOOMZ_OFFSET);
                else
                    dest = new Vector3(obj.transform.position.x, obj.transform.position.y + ZOOMY_OFFSET, obj.transform.position.z);

                mouve = true;
            }
        }
          
        //splashT.startAnim();
        
    }

    IEnumerator restore()
    {
        yield return new WaitForSeconds(2.0f);
        dest = DEFAULT_POS;
        toScalse = imgMinScale;
        mouve = true;

    }

    IEnumerator restore2()
    {
        yield return new WaitForSeconds(3.0f);
        mouve2 = false;
        stopped = false;
        toScalse = imgMinScale;
        immObj.sizeDelta = toScalse;
    }



    public void cameraPos1()
    {
        if(DEFAULT_POS != DEFAULT_POS1)
        {
            DEFAULT_POS = DEFAULT_POS1;
            transform.eulerAngles = DEFAULT_ROT_1;
            dest = DEFAULT_POS1;
            mouve = true;
        }
    }

    public void cameraPos2()
    {
        if (DEFAULT_POS != DEFAULT_POS2)
        {
            DEFAULT_POS = DEFAULT_POS2;
            transform.eulerAngles = DEFAULT_ROT_2;
            dest = DEFAULT_POS2;
            mouve = true;
        }
    }

    public int getCameraPos()
    {
        if (DEFAULT_POS == DEFAULT_POS1)
            return 1;
        else
            return 2;
    }
}