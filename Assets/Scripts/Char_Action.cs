using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Action : MonoBehaviour
{
    float xInput;
    float yInput;
    public float hangcounter;
    public float hangtime;
    public float jumpbuffercount;
    public float jumpbuffertime;
    public float shoottime;
    public float charspeed;
    public float jumpspeed;
    public float shootspeed;
    public float gravityscale;
    bool jumpcanceled = false;
    public LayerMask groundlayer;
    public bool isgrounded = true;
    public Transform groundCheckPoint1;
    public Transform groundCheckPoint2;
    public Transform groundCheckPoint3;
    public Transform CharPlugLinePos;
    public Transform PlugLinePos;
    public GameObject Plug;
    public int shootdir = 1;
    public int state;


    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame조작법
    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
    }
    void FixedUpdate()
    {
        isgrounded = Physics2D.OverlapCircle(groundCheckPoint1.position, 0.015f, groundlayer) || Physics2D.OverlapCircle(groundCheckPoint2.position, 0.015f, groundlayer) || Physics2D.OverlapCircle(groundCheckPoint3.position, 0.015f, groundlayer);
        if (isgrounded)
        {
            hangcounter = hangtime;
            jumpcanceled = false;
            anim.SetBool("isjumping", false);
        }
        else
        {
            hangcounter -= Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.C) && state == 0)
        {
            jumpbuffercount = jumpbuffertime;
        }
        else
        {
            jumpbuffercount -= Time.deltaTime;
        }
        
        if (jumpbuffercount > 0f && hangcounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpspeed);
            hangcounter = 0;
            jumpbuffercount = 0;
            anim.SetBool("isjumping", true);

        }
        if (!Input.GetKey(KeyCode.C) && rb.velocity.y > 0 && !jumpcanceled)
        {
            jumpcanceled = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            sr.flipX = false;
            shootdir = 1;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            sr.flipX = true;
            shootdir = -1;
        }
        if (Input.GetKeyDown(KeyCode.X) && shoottime > 0.2F)
        {
            shoottime = 0;
            if (state == 0 && isgrounded)
            {
                if (!Physics2D.OverlapCircle(new Vector2(CharPlugLinePos.position.x + 0.123f * shootdir, CharPlugLinePos.position.y), 0.1f, groundlayer))
                {
                    Plug.SetActive(true);
                    state = 1;
                    rb.gravityScale = 0;
                    Plug.transform.position = new Vector2(CharPlugLinePos.position.x + 0.123f * shootdir, CharPlugLinePos.position.y);
                    Plug.GetComponent<Rigidbody2D>().velocity = new Vector2(shootspeed * shootdir, 0f);
                    Plug.transform.localScale = new Vector3(shootdir, 1, 1);
                }
            }
            else if (state == 1)
            {
                Plug.SetActive(false);
                rb.gravityScale = gravityscale;
                state = 0;
            }

        }
        shoottime += Time.deltaTime;


        if (state == 1 || state == 2)
        {
            lr.enabled = true;
            lr.SetPosition(0, new Vector3(CharPlugLinePos.position.x, PlugLinePos.position.y, 0f));
            lr.SetPosition(1, new Vector3(PlugLinePos.position.x, PlugLinePos.position.y, 0f));
        }
        else
        {
            lr.enabled = false;
        }


        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * charspeed, rb.velocity.y);
        anim.SetFloat("xspeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yspeed", rb.velocity.y);


    }
}
