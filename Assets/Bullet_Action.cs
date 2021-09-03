using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Action : MonoBehaviour
{
    public LayerMask groundlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.layer);
        Debug.Log(groundlayer.value);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

}
