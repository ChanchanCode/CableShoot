using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Action : MonoBehaviour
{
    PlatformEffector2D PE;
    SpriteRenderer SR;
    bool isflipped;
    public bool leftopen = true, rightopen = true;
    public LayerMask groundlayer;
    public LayerMask NormalMask;
    public LayerMask FlippedMask;
    public Sprite[] spr;
    void Start()
    {
        isflipped = false;
        PE = GetComponent<PlatformEffector2D>();
        SR = GetComponent<SpriteRenderer>();
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position + new Vector3(-0.25f, 0.25f, 0f), 0.15f, groundlayer);

        foreach (Collider2D c in col)
        {
            if (c.gameObject.CompareTag("Platform") && c.gameObject != gameObject)
            {
                leftopen = false;
                break;
            }
        }
        col = Physics2D.OverlapCircleAll(transform.position + new Vector3(0.25f, 0.25f, 0f), 0.15f, groundlayer);
        foreach (Collider2D c in col)
        {
            if (c.gameObject.CompareTag("Platform") && c.gameObject != gameObject)
            {
                rightopen = false;
                break;
            }
        }
        if (leftopen)
        {
            if (rightopen) SR.sprite = spr[3];
            else SR.sprite = spr[0];
        }
        else
        {
            if (rightopen) SR.sprite = spr[2];
            else SR.sprite = spr[1];
        }
    }

    void Update()
    {
        if (Main_Action.isplatformflipped != isflipped)
        {
            isflipped = Main_Action.isplatformflipped;
            PE.colliderMask = isflipped ? FlippedMask : NormalMask;
        }
    }
}
