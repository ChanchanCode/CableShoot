using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

static class State
{
    public const int Died = -1;
    public const int Normal = 0;
    public const int Shoot = 1;
    public const int HorizontalHanging = 2;
    public const int VerticalHanging = 3;
    public const int Cutscene = 4;
}

public class Char_Action : MonoBehaviour
{
    public Main_Action main;

    public bool debugmode;
    public int debugstage;
    public string debugdir;
    float xoffset = 0.015f;
    float yoffset = -0.255f;
    float xsize = 0.19f;
    float ysize = 0.02f;

    float xInput;
    float yInput;
    public float hangcount;
    public float hangtime;
    public float jumpbuffercount;
    public float jumpbuffertime;
    public float jumptime;
    public float shootplugtime;
    public float picktime;
    float walktime;

    public float charspeed;
    public float jumpspeed;
    public float shootspeed;
    public float throwforce;
    public float gravityscale;
    bool jumpcanceled = false;
    public LayerMask groundlayer;
    public LayerMask pluglayer;
    public LayerMask interactlayer;
    public bool isgrounded = true;
    bool onplatform = false;
    public Transform CharPlugLinePos;
    public Transform PlugLinePos;
    public Transform PlugColCheckPos;
    public GameObject Plug;
    public GameObject BorderTile;
    public GameObject BoxBorder;
    public GameObject BorderTileBack;
    public int shootdir = 1;
    public int state;
    public bool isboxpicked;
    public GameObject PickedBox;
    public GameObject PoweredObject;
    public GameObject PluggedSocket;
    public ParticleSystem CircleParticle;
    public Image CoverPanel;
    public int coverpanelstate;
    public bool issticked;
    GameObject Moveblock;


    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    LineRenderer lr;
    AudioSource aud;
    BoxCollider2D bc;


    float VerticalInput;
    float HorizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();
        aud = GetComponent<AudioSource>();
        bc = GetComponent<BoxCollider2D>();
        PoweredObject = null;
        PickedBox = null;
        PluggedSocket = null;
        sr.enabled = true;
        BorderTileBack.SetActive(true);

        issticked = false;
        Moveblock = null;
        state = State.Normal;
        
        Main_Action.stage = debugstage;
        Main_Action.Entereddir = debugdir;


        GetComponent<BoxCollider2D>().enabled = true;
        RestartCurtainEnd();
        jumptime = 0f;
        walktime = 0f;
        bc.edgeRadius = 0.022f;
        VerticalInput = 0f;
        HorizontalInput = 0f;
        BorderTile.GetComponent<Tilemap>().color = new Color(1f, 1f, 1f, 1f);

    }

    // Update is called once per frame조작법
    void Update()
    {

        if (!Main_Action.isInventoryOpened)
        {
            if (Input.GetKeyDown(KeyCode.C) && jumptime >= 0.2f && !Main_Action.isroommove)
            {
                if (state == State.Normal)
                {
                    jumpbuffercount = jumpbuffertime;
                }
                else if (state == State.HorizontalHanging || state == State.VerticalHanging)
                {
                    state = State.Normal;
                    PlugFail();
                    rb.gravityScale = gravityscale;
                    jumpcanceled = false;
                    Jump();
                }
            }
            else
            {
                jumpbuffercount -= Time.fixedDeltaTime;
            }
            if (Input.GetKeyDown(KeyCode.Z) && picktime > 0.15f && !Main_Action.isroommove)
            {
                Interact();
            }
            if (Input.GetKeyDown(KeyCode.X) && shootplugtime > 0.07f && !Main_Action.isroommove)
            {
                ShootPlug();
            }
            if (Input.GetKeyDown(KeyCode.R) && !Main_Action.isroommove)
            {
                if (state != State.Died && state != State.Cutscene)
                {
                    RestartCurtainStart();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) Main_Action.IA.InventoryControl("right");
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) Main_Action.IA.InventoryControl("left");
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) Main_Action.IA.InventoryControl("up");
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) Main_Action.IA.InventoryControl("down");
            else if (Input.GetKeyDown(KeyCode.Z)) Main_Action.IA.InventoryControl("select");
        }


        if (!Input.GetKey(KeyCode.C) && rb.velocity.y > 0 && !jumpcanceled)
        {
            jumpcanceled = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.Q) && !Main_Action.isroommove)
        {
            if (state != State.Died && state != State.Cutscene)
            {
                if (!Main_Action.isInventoryOpened)
                {
                    Main_Action.isInventoryOpened = true;
                    Main_Action.IA.InventoryOpen();
                }
                else
                {
                    Main_Action.isInventoryOpened = false;
                    Main_Action.IA.InventoryClose();
                }
            }
        }


        if (state != State.Cutscene && state != State.Died)
        {
            if (HorizontalInput > 0)
            {
                SpriteFlipX(false);
            }
            else if (HorizontalInput < 0)
            {
                SpriteFlipX(true);
            }
        }


        if (coverpanelstate == 1)
        {
            CoverPanel.fillAmount = Mathf.Lerp(CoverPanel.fillAmount, 1f, 6f * Time.deltaTime);
            if (1 - CoverPanel.fillAmount < 0.1f)
            {
                Restart();
            }
        }
        else if (coverpanelstate == 2)
        {
            CoverPanel.fillAmount = Mathf.Lerp(CoverPanel.fillAmount, 0f, 6f * Time.deltaTime);
            if (CoverPanel.fillAmount < 0.1f)
            {
                coverpanelstate = 0;
            }
        }

        if (state == State.Shoot || state == State.HorizontalHanging || state == State.VerticalHanging)
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

            if (Plug.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
            {
                if (Mathf.Abs(transform.position.x - PlugLinePos.position.x) > 0.05f)
                {
                    Plug.SetActive(false);
                    rb.gravityScale = gravityscale;
                    state = State.Normal;
                    PlugFail();
                }
            }
            else
            {
                if (Mathf.Abs(transform.position.y - PlugLinePos.position.y) > 0.15f)
                {
                    Plug.SetActive(false);
                    rb.gravityScale = gravityscale;
                    state = State.Normal;
                    PlugFail();
                }
            }
        }   // 선 그리기 && 좌표 강제로 바뀌면 끊어짐
        else
        {
            lr.enabled = false;
        }

        anim.SetFloat("xspeed", Mathf.Abs(rb.velocity.x) - (Moveblock != null && issticked ? Moveblock.GetComponent<Rigidbody2D>().velocity.x : 0));
        anim.SetFloat("yspeed", rb.velocity.y - (Moveblock != null && issticked ? Moveblock.GetComponent<Rigidbody2D>().velocity.y : 0));
        anim.SetBool("ispluggedandground", (state == State.HorizontalHanging || state == State.VerticalHanging) && !isgrounded ? true : false);

    }

    private void FixedUpdate()
    {
        CheckIsGround();
        if (isgrounded)
        {
            hangcount = hangtime;
            jumpcanceled = false;
            if (rb.velocity.y < 1f)
            {
                anim.SetBool("isjumping", false);
            }
        }
        else
        {
            hangcount -= Time.fixedDeltaTime;
            anim.SetBool("isjumping", true);
        }

        if (jumpbuffercount > 0f && hangcount > 0f && jumptime > 0.2f &&
            (state == State.HorizontalHanging || state == State.VerticalHanging || state == State.Normal))
        {
            if (state == State.HorizontalHanging || state == State.VerticalHanging)
            {
                state = State.Normal;
                PlugFail();
                rb.gravityScale = gravityscale;
                jumpcanceled = false;
                Jump();
            }
            else
            {
                Jump();
            }
        }


        if (state == State.Shoot)
        {
            if (Plug.GetComponent<Rigidbody2D>().velocity.magnitude < shootspeed - 2f && shootplugtime > 0.07f)
            {
                Collider2D[] col = Physics2D.OverlapCircleAll(PlugColCheckPos.position, 0.08f, groundlayer);
                bool pluged = false;
                foreach (Collider2D c in col)
                {
                    if (c.gameObject.CompareTag("Socket"))
                    {
                        CameraShaker.Instance.ShakeOnce(3f, 3f, 0.1f, 0.1f);
                        PoweredObject = c.gameObject;
                        PowerOn();
                        PluggedSocket = c.gameObject;
                        c.gameObject.GetComponent<Socket_Action>().ispowered = true;
                        aud.PlayOneShot(main.Aud_Pluggedin, 0.6f);
                        if (Plug.transform.rotation == Quaternion.Euler(0f, 0f, 90f))
                        {
                            state = State.VerticalHanging;
                        }
                        else
                        {
                            state = State.HorizontalHanging;
                        }
                        pluged = true;
                        break;
                    }
                    else if (c.gameObject.CompareTag("Box"))
                    {
                        state = State.Normal;
                        PlugFail();
                        aud.PlayOneShot(main.Aud_NotPluggedin, 0.4f);
                        break;
                    }
                }
                if (!pluged)
                {
                    state = State.Normal;
                    PlugFail();
                    rb.gravityScale = gravityscale;
                    aud.PlayOneShot(main.Aud_NotPluggedin, 0.4f);
                }

            }
            else if (shootplugtime > 0.7f)
            {
                state = State.Normal;
                PlugFail();
                rb.gravityScale = gravityscale;
            }
            else if (PlugColCheckPos.position.x < Main_Action.viewborder.x + 0.15f ||
                PlugColCheckPos.position.x > Main_Action.viewborder.y - 0.15f ||
                PlugColCheckPos.position.y < Main_Action.viewborder.z + 0.15f ||
                PlugColCheckPos.position.y > Main_Action.viewborder.w - 0.15f)
            {
                state = State.Normal;
                Plug.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                PlugFail();
                rb.gravityScale = gravityscale;
                aud.PlayOneShot(main.Aud_NotPluggedin, 0.4f);
            }
        }
        if (state != State.Cutscene && state != State.Died)
        {
            if (!Main_Action.isroommove)
            {
                VerticalInput = Input.GetAxisRaw("Vertical");
                HorizontalInput = Input.GetAxisRaw("Horizontal");
            }
            else if (Main_Action.isroommove)
            {
                if (Main_Action.roommovedir == 0)
                {
                    VerticalInput = Input.GetAxisRaw("Vertical");
                    HorizontalInput = Input.GetAxisRaw("Horizontal");
                }
                else
                {
                    HorizontalInput = Main_Action.roommovedir * 0.4f;
                }
            }
        }

        shootplugtime += Time.fixedDeltaTime;
        picktime += Time.fixedDeltaTime;
        jumptime += Time.fixedDeltaTime;



        if (state == State.VerticalHanging) //이동
        {
            rb.velocity = new Vector2(0f, VerticalInput * charspeed * (isboxpicked ? 0.6f : 1) * 2f);
        }
        else if (state == State.Normal || state == State.HorizontalHanging)
        {
            rb.velocity = new Vector2(HorizontalInput * charspeed * (isboxpicked ? 0.6f : 1) * (state == State.HorizontalHanging ? 2 : 1)
                + (Moveblock != null && issticked ? Moveblock.GetComponent<Rigidbody2D>().velocity.x : 0), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        if (state == State.Normal && bc.edgeRadius != 0.022f)
        {
            bc.edgeRadius = 0.022f;
        }
        else if ((state == State.HorizontalHanging || state == State.VerticalHanging) && bc.edgeRadius != 0f)
        {
            bc.edgeRadius = 0f;
        }

        if (!Plug.GetComponent<Rigidbody2D>().freezeRotation)
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

        if (isboxpicked)
        {
            PickedBox.transform.position = new Vector2(CharPlugLinePos.position.x + 0.2f * shootdir, CharPlugLinePos.position.y + 0.2f);
        }

        if (Mathf.Abs(rb.velocity.x) - (Moveblock != null && issticked ? Moveblock.GetComponent<Rigidbody2D>().velocity.x : 0) > 1f && isgrounded)
        {
            walktime += Time.fixedDeltaTime;
        }
        else
        {
            walktime = 0.39f;
        }
        if (walktime >= 0.4f)
        {
            walktime = 0f;
            aud.PlayOneShot(main.Aud_Walk, 0.4f);
        }

    }
    void Jump()
    {
        jumptime = 0f;
        shootplugtime = 0;
        hangcount = 0;
        jumpbuffercount = 0;
        if (state != State.Shoot)
        {
            if (VerticalInput >= 0f || !onplatform)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpspeed * (isboxpicked ? 0.6f : 1));
                aud.PlayOneShot(main.Aud_Jump, 0.4f);
                anim.SetBool("isjumping", true);
            }
            else
            {
                main.PlatformDownJump();
            }
        }

    }
    void ShootPlug()
    {
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            shootplugtime = 0;
            if (state == State.Normal && isgrounded)
            {
                if (!Physics2D.OverlapCircle(new Vector2(CharPlugLinePos.position.x, CharPlugLinePos.position.y + 0.3f), 0.1f, groundlayer))
                {
                    PlugReset();
                    state = State.Shoot;
                    rb.gravityScale = 0;
                    rb.velocity = Vector2.zero;
                    Plug.transform.position = new Vector2(CharPlugLinePos.position.x, CharPlugLinePos.position.y + 0.3f);
                    Plug.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, shootspeed);
                    Plug.transform.localScale = new Vector3(1, 1, 1);
                    Plug.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    aud.PlayOneShot(main.Aud_Shoot, 1f);
                }
            }
            else if (state == State.HorizontalHanging || state == State.VerticalHanging)
            {
                state = State.Normal;
                PlugFail();
                rb.gravityScale = gravityscale;
            }
        }
        else
        {
            shootplugtime = 0;
            if (state == State.Normal && isgrounded)
            {
                if (!Physics2D.OverlapCircle(new Vector2(CharPlugLinePos.position.x + 0.123f * shootdir, CharPlugLinePos.position.y), 0.1f, groundlayer))
                {
                    PlugReset();
                    state = State.Shoot;
                    rb.gravityScale = 0;
                    rb.velocity = Vector2.zero;
                    Plug.transform.position = new Vector2(CharPlugLinePos.position.x + 0.123f * shootdir, CharPlugLinePos.position.y);
                    Plug.GetComponent<Rigidbody2D>().velocity = new Vector2(shootspeed * shootdir, 0f);
                    Plug.transform.localScale = new Vector3(shootdir, 1, 1);
                    Plug.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    aud.PlayOneShot(main.Aud_Shoot, 1f);
                }
            }
            else if (state == State.HorizontalHanging || state == State.VerticalHanging)
            {
                state = State.Normal;
                PlugFail();
                rb.gravityScale = gravityscale;
            }
        }
    }
    void Interact()
    {
        if (state == State.Normal || state == State.HorizontalHanging || state == State.VerticalHanging)
        {
            Collider2D[] col = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 0.5f, interactlayer);
            foreach (Collider2D c in col)
            {
                if (c.gameObject.GetComponent<InteractTrigger>() != null)
                {
                    state = State.Cutscene;
                    rb.velocity = new Vector2(0f, rb.velocity.y);
                    Main_Action.CM.RunCutScene(c.gameObject.GetComponent<InteractTrigger>().interactnum);
                    if (c.gameObject.GetComponent<InteractTrigger>().eyecontact)
                    {
                        if (c.gameObject.transform.position.x > transform.position.x)
                        {
                            SpriteFlipX(false);
                            c.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                        }
                        else
                        {
                            SpriteFlipX(true);
                            c.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                        }
                    }
                    break;
                }
            }
        }
        Pick();
    }
    void Pick()
    {
        if (state == State.Normal || state == State.HorizontalHanging || state == State.VerticalHanging)
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
                        rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(jumpspeed * (isboxpicked ? 0.6f : 1), rb.velocity.y));
                        break;
                    }
                }
            }
            else if (!Physics2D.OverlapCircle(new Vector2(CharPlugLinePos.position.x + 0.4f * shootdir, CharPlugLinePos.position.y + 0.2f), 0.15f, groundlayer) &&
                !Physics2D.OverlapCircle(new Vector2(CharPlugLinePos.position.x + 0.25f * shootdir, CharPlugLinePos.position.y + 0.1f), 0.15f, groundlayer))
            {
                isboxpicked = false;
                PickedBox.transform.position = new Vector2(CharPlugLinePos.position.x + 0.3f * shootdir, CharPlugLinePos.position.y + 0.2f);
                PickedBox.transform.SetParent(null);
                PickedBox.GetComponent<Rigidbody2D>().gravityScale = gravityscale;
                PickedBox.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                PickedBox.GetComponent<Rigidbody2D>().AddForce(new Vector2(shootdir * 3f, 1f) * throwforce, ForceMode2D.Impulse);
                PickedBox.layer = 8;
                Invoke("TurnOnPickedUpBoxCollider", 0.07f);
                PickedBox.GetComponent<SpriteRenderer>().sortingOrder = 3;
            }
        }
    }
    public void PlugFail()
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
        Plug.GetComponent<SpriteRenderer>().sortingOrder = -2;
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
    void SpriteFlipX(bool b)
    {
        sr.flipX = b;
        if (b)
        {
            bc.offset = new Vector2(-Mathf.Abs(bc.offset.x), bc.offset.y);
            shootdir = -1;
        }
        else
        {
            bc.offset = new Vector2(Mathf.Abs(bc.offset.x), bc.offset.y);
            shootdir = 1;
        }
    }

    void CheckIsGround()
    {
        Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + xoffset * shootdir, transform.position.y + yoffset), new Vector2(xsize, ysize), 0f, groundlayer);
        if (col.Length > 0)
        {
            isgrounded = true;
            onplatform = false;
            foreach (Collider2D c in col)
            {
                if (c.CompareTag("Platform"))
                {
                    onplatform = true;
                }
            }
        }
        else isgrounded = false;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MapBorder"))
        {
            Main_Action.isroommove = true;
            if (state != State.Cutscene)
            {
                state = State.Normal;
            }
            PlugFail();
            Plug.SetActive(false);
            rb.gravityScale = gravityscale;
            Vector2 viewsize = collision.gameObject.GetComponent<BoxCollider2D>().size / 2f;
            Main_Action.viewborder = new Vector4(collision.gameObject.transform.position.x - viewsize.x, collision.gameObject.transform.position.x + viewsize.x,
                collision.gameObject.transform.position.y - viewsize.y, collision.gameObject.transform.position.y + viewsize.y);
            if (Main_Action.viewborder.z > transform.position.y)
            {
                
                rb.MovePosition(transform.position + Vector3.up * 0.5f);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(jumpspeed * 0.4f, rb.velocity.y));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Danger") && state != State.Died)
        {
            aud.PlayOneShot(main.Aud_Die, 0.6f);
            CameraShaker.Instance.ShakeOnce(5f, 7f, 0.1f, 0.3f);
            CircleParticle.Play();
            state = State.Died;
            rb.velocity = Vector2.zero;
            sr.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            PlugFail();
            Plug.SetActive(false);
            if (PickedBox != null)
            {
                isboxpicked = false;
                PickedBox.transform.SetParent(null);
                PickedBox.GetComponent<Rigidbody2D>().gravityScale = gravityscale;
                PickedBox.layer = 8;
                PickedBox.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                PickedBox.GetComponent<BoxCollider2D>().enabled = true;
                PickedBox.GetComponent<SpriteRenderer>().sortingOrder = 3;
            }
            Invoke("RestartCurtainStart", 0.6f);
        }
        if (collision.gameObject.CompareTag("MovingBlock") && isgrounded)
        {
            issticked = true;
            Moveblock = collision.gameObject;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingBlock") && collision.gameObject == Moveblock)
        {
            issticked = false;
            Moveblock = null;
        }
    }
    void Restart()
    {
        Main_Action.attempts += 1;
        PlugFail();
        Plug.SetActive(false);
        Main_Action.isroommove = false;
        SceneManager.LoadScene("MainScene");
    }
    void RestartCurtainStart()
    {
        CoverPanel.fillOrigin = (int)Image.OriginHorizontal.Left;
        coverpanelstate = 1;
    }
    void RestartCurtainEnd()
    {
        CoverPanel.gameObject.SetActive(true);
        CoverPanel.fillOrigin = (int)Image.OriginHorizontal.Right;
        CoverPanel.fillAmount = 1f;
        coverpanelstate = 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector2(transform.position.x + xoffset * shootdir, transform.position.y + yoffset), new Vector2(xsize, ysize) * 2f);
    }
}
