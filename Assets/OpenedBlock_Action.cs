using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenedBlock_Action : MonoBehaviour
{
    PowerBlock_Action PA;
    int powered;
    public Sprite[] LBS;
    SpriteRenderer sr;
    BoxCollider2D bc;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        PA = GetComponent<PowerBlock_Action>();
    }

    // Update is called once per frame
    void Update()
    {
        powered = PA.powered;

        sr.color = powered > 0 ? Main_Action.PowerCol : Color.white;
        sr.sprite = powered > 0 ? LBS[1] : LBS[0];
        bc.enabled = (powered > 0);
        
    }
}
