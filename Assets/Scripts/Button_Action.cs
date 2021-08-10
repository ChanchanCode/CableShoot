using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Action : MonoBehaviour
{
    public List<GameObject> PowerObject;


    public Sprite[] buttonsprite;
    public Transform ButtonCheckPos1;
    public Transform ButtonCheckPos2;
    public Transform ButtonCheckPos3;
    public LayerMask playerlayer;
    public LayerMask groundlayer;
    bool ispressed;
    bool previspressed;
    float dummynum;
    SpriteRenderer sr;
    GameObject box;
    AudioSource aud;
    public AudioClip Aud_Button;
    // Start is called before the first frame update
    void Start()
    {
        ispressed = false;
        previspressed = ispressed;
        sr = GetComponent<SpriteRenderer>();
        aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ispressed = 
            Physics2D.OverlapCircle(ButtonCheckPos1.position, 0.0763f, playerlayer) ||
            Physics2D.OverlapCircle(ButtonCheckPos2.position, 0.0763f, playerlayer) ||
            Physics2D.OverlapCircle(ButtonCheckPos3.position, 0.0763f, playerlayer);
        if (!ispressed)
        {
            Collider2D[] col = Physics2D.OverlapCircleAll(ButtonCheckPos1.position, 0.0763f, groundlayer);
            foreach (Collider2D c in col)
            {
                if (c.gameObject.CompareTag("Box"))
                {
                    if (box != c.gameObject)
                        box = c.gameObject;
                    ispressed = true;
                }
                else if(c.gameObject.CompareTag("MovingBlock"))
                {
                    ispressed = true;
                }
            }
            col = Physics2D.OverlapCircleAll(ButtonCheckPos2.position, 0.0763f, groundlayer);
            foreach (Collider2D c in col)
            {
                if (c.gameObject.CompareTag("Box"))
                {
                    if (box != c.gameObject)
                        box = c.gameObject;
                    ispressed = true;
                }
                else if (c.gameObject.CompareTag("MovingBlock"))
                {
                    ispressed = true;
                }
            }
            col = Physics2D.OverlapCircleAll(ButtonCheckPos3.position, 0.0763f, groundlayer);
            foreach (Collider2D c in col)
            {
                if (c.gameObject.CompareTag("Box"))
                {
                    if (box != c.gameObject)
                        box = c.gameObject;
                    ispressed = true;
                }
                else if (c.gameObject.CompareTag("MovingBlock"))
                {
                    ispressed = true;
                }
            }
        }


        if (ispressed)
        {
            if (sr.color == Color.white)
            {
                sr.color = Main_Action.PowerCol;
            }
            if (dummynum < 2f)
            {
                dummynum += Time.deltaTime * 20;
            }
        }
        else
        {
            if (sr.color == Main_Action.PowerCol)
            {
                sr.color = Color.white;
            }
            if (dummynum > 0.1f)
            {
                dummynum -= Time.deltaTime * 20;
            }
        }
        if (ispressed && box != null)
        {
            box.GetComponent<SpriteRenderer>().color = Main_Action.PowerCol;
        }
        else if (!ispressed)
        {
            if (box != null)
            {
                box.GetComponent<SpriteRenderer>().color = Color.white;
                box = null;
            }
        }

        if (ispressed != previspressed)
        {
            previspressed = ispressed;
            if (PowerObject != null)
            {
                int a = ispressed ? 1 : -1;
                foreach (GameObject g in PowerObject)
                {
                    g.GetComponent<PowerBlock_Action>().powered += a;
                }
            }
            if (ispressed)
            {
                aud.PlayOneShot(Aud_Button, 0.2f);
            }
        }



        sr.sprite = buttonsprite[(Mathf.FloorToInt(Mathf.Clamp(dummynum, 0, 2)))];
    }
}
