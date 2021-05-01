using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket_Action : MonoBehaviour
{
    public bool ispowered;
    public List<GameObject> PowerObject;
    public bool previspowered;

    // Start is called before the first frame update
    void Start()
    {
        ispowered = false;
        previspowered = ispowered;
    }

    // Update is called once per frame
    void Update()
    {
        if (ispowered != previspowered)
        {
            previspowered = ispowered;
            if (PowerObject != null)
            {
                foreach (GameObject g in PowerObject)
                {
                    g.GetComponent<PowerBlock_Action>().ispowered = ispowered;
                }
            }
        }
    }
}
