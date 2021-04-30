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
    public float charspeed;
    public float jumpspeed;
    public float maxspeed;
    bool jumpcanceled = false;
    public LayerMask groundlayer;
    public bool isgrounded = true;
    public Transform groundCheckPoint1;
    public Transform groundCheckPoint2;
    public Transform groundCheckPoint3;
    
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
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
        if(Input.GetKeyDown(KeyCode.C))
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
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            sr.flipX = true;
        }
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * charspeed, rb.velocity.y);
        anim.SetFloat("xspeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yspeed", rb.velocity.y);


    }
}
