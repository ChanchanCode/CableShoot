﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock_Action : MonoBehaviour
{
    public string dir;
    public int powered;
    PowerBlock_Action PA;
    SpriteRenderer sr;
    Rigidbody2D rb;
    float offset = 0.25f;
    float playerheight = 8f;
    float playerwidth = 0.12f;
    public LayerMask playerlayer;
    public LayerMask groundlayer;
    Vector3 originpos;
    bool istouched;
    // Start is called before the first frame update
    void Start()
    {
        PA = GetComponent<PowerBlock_Action>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originpos = transform.position;
        istouched = false;
    }

    // Update is called once per frame
    void Update()
    {
        powered = PA.powered;
        sr.color = powered > 0 ? Main_Action.PowerCol : Color.white;
        if (powered > 0)
        {
            if (dir == "up")
            {
                if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + offset + 0.04f), new Vector2(offset, 0.02f), 0f, playerlayer))
                {
                    if (!Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + offset + 0.35f), new Vector2(offset, 0.02f), 0f, groundlayer))
                    {
                        rb.velocity = Vector2.up * 1f;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else
                {
                    Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y + offset - 0.01f), new Vector2(offset, 0.000001f), 0f, groundlayer);
                    istouched = false;
                    foreach (Collider2D c in col)
                    {
                        if (c.gameObject != gameObject && !c.gameObject.CompareTag("Box"))
                        {
                            istouched = true;
                        }
                    }
                    if (!istouched)
                    {
                        rb.velocity = Vector2.up * 1f;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
            else if (dir == "down")
            {
                if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - offset - 0.04f), new Vector2(offset, 0.02f), 0f, playerlayer))
                {
                    if (!Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - offset - 0.35f), new Vector2(offset, 0.02f), 0f, groundlayer))
                    {
                        rb.velocity = Vector2.down * 1f;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else
                {
                    Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y - offset + 0.01f), new Vector2(offset, 0.000001f), 0f, groundlayer);
                    istouched = false;
                    foreach (Collider2D c in col)
                    {
                        if (c.gameObject != gameObject)
                        {
                            istouched = true;
                        }
                    }
                    if (!istouched)
                    {
                        rb.velocity = Vector2.down * 1f;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
            else if (dir == "left")
            {
                if (Physics2D.OverlapBox(new Vector2(transform.position.x - offset - 0.04f, transform.position.y), new Vector2(0.02f, offset), 0f, playerlayer))
                {
                    if (!Physics2D.OverlapBox(new Vector2(transform.position.x - offset - 0.3208f, transform.position.y), new Vector2(0.02f, offset), 0f, groundlayer))
                    {
                        rb.velocity = Vector2.left * 1f;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else
                {
                    Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x - offset + 0.005f, transform.position.y), new Vector2(0.000001f, offset), 0f, groundlayer);
                    istouched = false;
                    foreach (Collider2D c in col)
                    {
                        if (c.gameObject != gameObject)
                        {
                            istouched = true;
                        }
                    }
                    if (!istouched)
                    {
                        rb.velocity = Vector2.left * 1f;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
            else if (dir == "right")
            {
                if (Physics2D.OverlapBox(new Vector2(transform.position.x + offset + 0.04f, transform.position.y), new Vector2(0.02f, offset), 0f, playerlayer))
                {
                    if (!Physics2D.OverlapBox(new Vector2(transform.position.x + offset + 0.3208f, transform.position.y), new Vector2(0.02f, offset), 0f, groundlayer))
                    {
                        rb.velocity = Vector2.right * 1f;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else
                {
                    Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + offset - 0.005f, transform.position.y), new Vector2(0.000001f, offset), 0f, groundlayer);
                    istouched = false;
                    foreach (Collider2D c in col)
                    {
                        if (c.gameObject != gameObject)
                        {
                            istouched = true;
                        }
                    }
                    if (!istouched)
                    {
                        rb.velocity = Vector2.right * 1f;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }

        }
        else
        {
            
            if (Vector3.Magnitude(transform.position - originpos) > 0.01f)
            {
                if (dir == "up")
                {
                    if (transform.position.y > originpos.y)
                    {
                        if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - offset - 0.04f), new Vector2(offset, 0.02f), 0f, playerlayer))
                        {
                            if (!Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - offset - 0.35f), new Vector2(offset, 0.02f), 0f, groundlayer))
                            {
                                rb.velocity = Vector2.down * 1f;
                            }
                            else
                            {
                                rb.velocity = Vector2.zero;
                            }
                        }
                        else
                        {
                            Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y - offset + 0.01f), new Vector2(offset, 0.000001f), 0f, groundlayer);
                            istouched = false;
                            foreach (Collider2D c in col)
                            {
                                if (c.gameObject != gameObject)
                                {
                                    istouched = true;
                                }
                            }
                            if (!istouched)
                            {
                                rb.velocity = Vector2.down * 1f;
                            }
                            else
                            {
                                rb.velocity = Vector2.zero;
                            }
                        }
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }

                }
                else if (dir == "down")
                {
                    if (transform.position.y < originpos.y)
                    {
                        if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + offset + 0.04f), new Vector2(offset, 0.02f), 0f, playerlayer))
                        {
                            if (!Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + offset + 0.35f), new Vector2(offset, 0.02f), 0f, groundlayer))
                            {
                                rb.velocity = Vector2.up * 1f;
                            }
                            else
                            {
                                rb.velocity = Vector2.zero;
                            }
                        }
                        else
                        {
                            Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y + offset - 0.01f), new Vector2(offset, 0.000001f), 0f, groundlayer);
                            istouched = false;
                            foreach (Collider2D c in col)
                            {
                                if (c.gameObject != gameObject && !c.gameObject.CompareTag("Box"))
                                {
                                    istouched = true;
                                }
                            }
                            if (!istouched)
                            {
                                rb.velocity = Vector2.up * 1f;
                            }
                            else
                            {
                                rb.velocity = Vector2.zero;
                            }
                        }
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else if (dir == "left")
                {
                    if (transform.position.x < originpos.x)
                    {
                        if (Physics2D.OverlapBox(new Vector2(transform.position.x + offset + 0.04f, transform.position.y), new Vector2(0.02f, offset), 0f, playerlayer))
                        {
                            if (!Physics2D.OverlapBox(new Vector2(transform.position.x + offset + 0.3208f, transform.position.y), new Vector2(0.02f, offset), 0f, groundlayer))
                            {
                                rb.velocity = Vector2.right * 1f;
                            }
                            else
                            {
                                rb.velocity = Vector2.zero;
                            }
                        }
                        else
                        {
                            Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + offset - 0.005f, transform.position.y), new Vector2(0.000001f, offset), 0f, groundlayer);
                            istouched = false;
                            foreach (Collider2D c in col)
                            {
                                if (c.gameObject != gameObject && c.gameObject.CompareTag("Box"))
                                {
                                    istouched = true;
                                }
                            }
                            if (!istouched)
                            {
                                rb.velocity = Vector2.right * 1f;
                            }
                            else
                            {
                                rb.velocity = Vector2.zero;
                            }
                        }
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
                else if (dir == "right")
                {
                    if (transform.position.x > originpos.x)
                    {
                        if (Physics2D.OverlapBox(new Vector2(transform.position.x - offset - 0.04f, transform.position.y), new Vector2(0.02f, offset), 0f, playerlayer))
                        {
                            if (!Physics2D.OverlapBox(new Vector2(transform.position.x - offset - 0.3208f, transform.position.y), new Vector2(0.02f, offset), 0f, groundlayer))
                            {
                                rb.velocity = Vector2.left * 1f;
                            }
                            else
                            {
                                rb.velocity = Vector2.zero;
                            }
                        }
                        else
                        {
                            Collider2D[] col = Physics2D.OverlapBoxAll(new Vector2(transform.position.x - offset + 0.005f, transform.position.y), new Vector2(0.000001f, offset), 0f, groundlayer);
                            istouched = false;
                            foreach (Collider2D c in col)
                            {
                                if (c.gameObject != gameObject)
                                {
                                    istouched = true;
                                }
                            }
                            if (!istouched)
                            {
                                rb.velocity = Vector2.left * 1f;
                            }
                            else
                            {
                                rb.velocity = Vector2.zero;
                            }
                        }
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
            else
            {
                transform.position = originpos;
                rb.velocity = Vector2.zero;
            }

            
        }
    }
}
