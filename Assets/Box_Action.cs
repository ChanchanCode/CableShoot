using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Action : MonoBehaviour
{
    float offset = 0.25f;
    public LayerMask groundlayer;
    bool issticked;
    bool nowsticked;
    GameObject Moveblock;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        issticked = false;
        nowsticked = false;
        Moveblock = null;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Plug"))
        {
            GameObject plug = collision.gameObject;
            if (!plug.GetComponent<Rigidbody2D>().freezeRotation)
            {
                if (plug.transform.position.y < gameObject.transform.position.y - 0.25f)
                {
                    plug.SetActive(false);
                }
            }
        }
    }
}
