using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Action : MonoBehaviour
{
    float xInput;
    float yInput;
    public float charspeed;
    public float jumpspeed;
    public float maxspeed;
    bool isgrounded = true;
    Transform groundCheckPoint1;
    Transform groundCheckPoint2;

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame조작법
    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        

    }
    void FixedUpdate()
    {
        isgrounded = Physics2D.OverlapCircle(groundCheckPoint1.position, 0.1f, )
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * charspeed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.C) && isgrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpspeed);
            isgrounded = false;
        }
        if (Input.GetKeyUp(KeyCode.C) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    

    }
}
