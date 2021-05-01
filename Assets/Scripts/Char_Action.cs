using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Char_Action : MonoBehaviour
{
    float xInput;
    float yInput;
    public float hangcount;
    public float hangtime;
    public float jumpbuffercount;
    public float jumpbuffertime;
    public float shoottime;
    public float picktime;

    public float charspeed;
    public float jumpspeed;
    public float shootspeed;
    public float throwforce;
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
    public Transform BorderPos;
    public GameObject Plug;
    public GameObject BorderTile;
    public GameObject BorderTileBack;
    public int shootdir = 1;
    public int state;
    public bool isboxpicked;
    public GameObject PickedBox;
    public GameObject PoweredObject;
    public GameObject PluggedSocket;
    
    public string Entereddir;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        Entereddir = "left";
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();
        PoweredObject = null;
        PickedBox = null;
        PluggedSocket = null;
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

        if (Input.GetKeyDown(KeyCode.Z) && picktime > 0.15f)
        {
            Pick();
        }

        isgrounded = Physics2D.OverlapCircle(groundCheckPoint1.position, 0.015f, groundlayer) || Physics2D.OverlapCircle(groundCheckPoint2.position, 0.015f, groundlayer) || Physics2D.OverlapCircle(groundCheckPoint3.position, 0.015f, groundlayer);


        if (isgrounded)
        {
            hangcount = hangtime;
            jumpcanceled = false;
            anim.SetBool("isjumping", false);
        }
        else
        {
            hangcount -= Time.deltaTime;
        }


        if (jumpbuffercount > 0f && hangcount > 0f)
        {
            if (state == 2 || state == 3)
            {
                PlugFail();
                rb.gravityScale = gravityscale;
                state = 0;
                jumpcanceled = false;
                Jump();
            }
            else
            {
                Jump();
            }
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
                        CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 0.1f);
                        PoweredObject = c.gameObject;
                        PowerOn();
                        PluggedSocket = c.gameObject;
                        c.gameObject.GetComponent<Socket_Action>().ispowered = true;
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
            else if (shoottime > 0.5f)
            {
                PlugFail();
                rb.gravityScale = gravityscale;
                state = 0;
            }
            if (Plug.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
            {
                if (Mathf.Abs(transform.position.x - PlugLinePos.position.x) > 0.05f)
                {
                    Plug.SetActive(false);
                    PlugFail();
                    rb.gravityScale = gravityscale;
                    state = 0;
                }
            }
            else
            {
                if (Mathf.Abs(transform.position.y - PlugLinePos.position.y) > 0.1f)
                {
                    Plug.SetActive(false);
                    PlugFail();
                    rb.gravityScale = gravityscale;
                    state = 0;
                }
            }
        }

        shoottime += Time.deltaTime;
        picktime += Time.deltaTime;

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
        }   // 선 그리기
        else
        {
            lr.enabled = false;
        }


        if (state == 3) //이동
        {
            rb.velocity = new Vector2(0f, Input.GetAxisRaw("Vertical") * charspeed * (isboxpicked ? 0.6f : 1) * (state == 2 || state == 3 ? 2 : 1));
        }
        else
        {
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * charspeed * (state == 1 ? 0 : 1) * (isboxpicked ? 0.6f : 1) * (state == 2 || state == 3 ? 2 : 1), rb.velocity.y);
        }


        if (!Plug.GetComponent<Rigidbody2D>().freezeRotation)
        {
            if (Vector3.Distance(CharPlugLinePos.position, Plug.transform.position) < 0.5f)
            {
                if (Physics2D.OverlapCircle(groundCheckPoint1.position, 0.02f, pluglayer) || Physics2D.OverlapCircle(groundCheckPoint2.position, 0.02f, pluglayer) || Physics2D.OverlapCircle(groundCheckPoint3.position, 0.02f, pluglayer))
                {
                    Plug.SetActive(false);
                }
            }
            else
            {
                Collider2D[] col = Physics2D.OverlapCircleAll(Plug.transform.position, 0.121f, groundlayer);
                foreach (Collider2D c in col)
                {
                    if (c.gameObject.CompareTag("Box"))
                    {
                        if (c.transform.position.y > Plug.transform.position.y + 0.25f)
                        {
                            Plug.SetActive(false);
                        }
                    }
                }
            }
        }

        if (isboxpicked)
        {
            PickedBox.transform.position = new Vector2(CharPlugLinePos.position.x + 0.2f * shootdir, CharPlugLinePos.position.y + 0.2f);
        }

        anim.SetFloat("xspeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yspeed", rb.velocity.y);
        anim.SetBool("ispluggedandground", (state == 2 || state == 3) && !isgrounded ? true : false);
    }
    void Jump()
    {
        shoottime = 0;
        rb.velocity = new Vector2(rb.velocity.x, jumpspeed * (isboxpicked ? 0.6f : 1));
        hangcount = 0;
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
                    rb.velocity = Vector2.zero;
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
                    rb.velocity = Vector2.zero;
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
    void Pick()
    {
        picktime = 0;
        if (!isboxpicked)
        {
            Collider2D[] col = Physics2D.OverlapCircleAll(new Vector2(CharPlugLinePos.position.x + 0.123f * shootdir, CharPlugLinePos.position.y), 0.1f, groundlayer);
            foreach (Collider2D c in col)
            {
                if (c.gameObject.CompareTag("Box"))
                {
                    isboxpicked = true;
                    PickedBox = c.gameObject;
                    PickedBox.transform.SetParent(transform);
                    PickedBox.GetComponent<BoxCollider2D>().enabled = false;
                    PickedBox.GetComponent<Rigidbody2D>().gravityScale = 0;
                    PickedBox.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    PickedBox.layer = 10;
                }
            }
        }
        else if (!Physics2D.OverlapCircle(new Vector2(CharPlugLinePos.position.x + 0.4f * shootdir, CharPlugLinePos.position.y + 0.2f), 0.15f, groundlayer))
        {
            isboxpicked = false;
            PickedBox.transform.position = new Vector2(CharPlugLinePos.position.x + 0.3f * shootdir, CharPlugLinePos.position.y + 0.2f);
            PickedBox.transform.SetParent(null);
            PickedBox.GetComponent<Rigidbody2D>().gravityScale = gravityscale;
            PickedBox.GetComponent<Rigidbody2D>().AddForce(new Vector2(shootdir*3f, 1f) * throwforce, ForceMode2D.Impulse);
            PickedBox.layer = 8;
            Invoke("TurnOnPickedUpBoxCollider", 0.07f);
            PickedBox.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
        
    }
    void PlugFail()
    {
        PowerOff();
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
    void TurnOnPickedUpBoxCollider()
    {
        PickedBox.GetComponent<BoxCollider2D>().enabled = true;
        PickedBox = null;
    }
    void PowerOn()
    {
        if (PoweredObject != null)
        {
            PoweredObject.GetComponent<SpriteRenderer>().color = Main_Action.PowerCol;
            lr.startColor = Main_Action.PowerCol;
            lr.endColor = Main_Action.PowerCol;
            Plug.GetComponent<SpriteRenderer>().color = Main_Action.PowerCol;
        }
    }
    void PowerOff()
    {
        if (PoweredObject != null)
        {
            PoweredObject.GetComponent<SpriteRenderer>().color = Color.white;
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            Plug.GetComponent<SpriteRenderer>().color = Color.white;
            PoweredObject = null;
            if (PluggedSocket != null)
            {
                PluggedSocket.GetComponent<Socket_Action>().ispowered = false;
                PluggedSocket = null;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BorderLine"))
        {

        }
    }
}
