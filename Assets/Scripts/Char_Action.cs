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
    public LayerMask pluglayer;
    public bool isgrounded = true;
    public Transform groundCheckPoint1;
    public Transform groundCheckPoint2;
    public Transform groundCheckPoint3;
    public Transform CharPlugLinePos;
    public Transform PlugLinePos;
    public Transform PlugColCheckPos;
    public GameObject Plug;
    public int shootdir = 1;
    public int state;
    public bool isboxpicked;


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
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (state == 0)
            {
                jumpbuffercount = jumpbuffertime;
            }
            else if (state == 2 || state == 3)
            {
                PlugFail();
                rb.gravityScale = gravityscale;
                state = 0;
                jumpcanceled = false;
                Jump();
            }
        }
        else
        {
            jumpbuffercount -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.X) && shoottime > 0.07f)
        {
            Shoot();
        }
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


        if (jumpbuffercount > 0f && hangcounter > 0f)
        {
            Jump();
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


        if (state == 1)
        {
            if (Plug.GetComponent<Rigidbody2D>().velocity.magnitude < shootspeed-2f && shoottime > 0.07f)
            {
                Collider2D[] col = Physics2D.OverlapCircleAll(PlugColCheckPos.position, 0.15f, groundlayer);
                bool pluged = false;
                foreach (Collider2D c in col)
                {
                    if (c.gameObject.CompareTag("Socket"))
                    {
                        if (Plug.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
                        {
                            state = 3;
                        }
                        else
                        {
                            state = 2;
                        }
                        pluged = true;
                        break;
                    }
                    else if (c.gameObject.CompareTag("Box"))
                    {
                        PlugFail();
                        state = 0;
                        break;
                    }
                }
                if (!pluged)
                {
                    PlugFail();
                    rb.gravityScale = gravityscale;
                    state = 0;
                }
                
            }
            else if (shoottime > 1f)
            {
                PlugFail();
                rb.gravityScale = gravityscale;
                state = 0;
            }

        }

        shoottime += Time.deltaTime;

        if (state == 1 || state == 2 || state == 3)
        {
            lr.enabled = true;
            if (Plug.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
            {
                lr.SetPosition(0, new Vector3(PlugLinePos.position.x, CharPlugLinePos.position.y, 0f));
                lr.SetPosition(1, new Vector3(PlugLinePos.position.x, PlugLinePos.position.y, 0f));
            }
            else
            {
                lr.SetPosition(0, new Vector3(CharPlugLinePos.position.x, PlugLinePos.position.y, 0f));
                lr.SetPosition(1, new Vector3(PlugLinePos.position.x, PlugLinePos.position.y, 0f));
            }

        }
        else
        {
            lr.enabled = false;
        }
        if (state == 3)
        {
            rb.velocity = new Vector2(0f, Input.GetAxisRaw("Vertical") * charspeed);
        }
        else
        {
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * charspeed * (state == 1 ? 0 : 1), rb.velocity.y);
        }

        if (!Plug.GetComponent<Rigidbody2D>().freezeRotation && Vector3.Distance(CharPlugLinePos.position, Plug.transform.position) < 0.5f)
        {
            if(Physics2D.OverlapCircle(groundCheckPoint1.position, 0.02f, pluglayer) || Physics2D.OverlapCircle(groundCheckPoint2.position, 0.02f, pluglayer) || Physics2D.OverlapCircle(groundCheckPoint3.position, 0.02f, pluglayer))
            {
                Plug.SetActive(false);
            }
        }

        anim.SetFloat("xspeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yspeed", rb.velocity.y);
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpspeed);
        hangcounter = 0;
        jumpbuffercount = 0;
        anim.SetBool("isjumping", true);
    }
    void Shoot()
    {
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            shoottime = 0;
            if (state == 0 && isgrounded)
            {
                if (!Physics2D.OverlapCircle(new Vector2(CharPlugLinePos.position.x, CharPlugLinePos.position.y + 0.6f), 0.1f, groundlayer))
                {
                    PlugReset();
                    state = 1;
                    rb.gravityScale = 0;
                    Plug.transform.position = new Vector2(CharPlugLinePos.position.x, CharPlugLinePos.position.y + 0.6f);
                    Plug.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, shootspeed);
                    Plug.transform.localScale = new Vector3(1, 1, 1);
                    Plug.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                }
            }
            else if (state == 2 || state == 3)
            {
                PlugFail();
                rb.gravityScale = gravityscale;
                state = 0;
            }
        }
        else
        {
            shoottime = 0;
            if (state == 0 && isgrounded)
            {
                if (!Physics2D.OverlapCircle(new Vector2(CharPlugLinePos.position.x + 0.123f * shootdir, CharPlugLinePos.position.y), 0.1f, groundlayer))
                {
                    PlugReset();
                    state = 1;
                    rb.gravityScale = 0;
                    Plug.transform.position = new Vector2(CharPlugLinePos.position.x + 0.123f * shootdir, CharPlugLinePos.position.y);
                    Plug.GetComponent<Rigidbody2D>().velocity = new Vector2(shootspeed * shootdir, 0f);
                    Plug.transform.localScale = new Vector3(shootdir, 1, 1);
                    Plug.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            else if (state == 2 || state == 3)
            {
                PlugFail();
                rb.gravityScale = gravityscale;
                state = 0;
            }
        }
    }
    void PlugFail()
    {
        Plug.GetComponent<Rigidbody2D>().gravityScale = gravityscale;
        Plug.GetComponent<Rigidbody2D>().freezeRotation = false;
        Plug.GetComponent<Rigidbody2D>().mass = 0.5f;
        Plug.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1f, 1f));
        if (Plug.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
        {
            Plug.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-0.2f, 0.2f), -0.2f), ForceMode2D.Impulse);
        }
        else
        {
            Plug.GetComponent<Rigidbody2D>().AddForce(new Vector2(-shootdir, Random.Range(0.1f, 2f)), ForceMode2D.Impulse);
        }
        
    }
    void PlugReset()
    {
        Plug.SetActive(true);
        Plug.GetComponent<SpriteRenderer>().sortingOrder = 1;
        Plug.GetComponent<Rigidbody2D>().mass = 3f;
        Plug.GetComponent<Rigidbody2D>().gravityScale = 0;
        Plug.GetComponent<Rigidbody2D>().freezeRotation = true;
    }
}
