using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Action : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

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
