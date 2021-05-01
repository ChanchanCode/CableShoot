using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Action : MonoBehaviour
{
    public static Color PowerCol;
    public static int stage = 0;
    public static float borderx = -0.5f;
    public static float bordery = 0f;
    public Transform[] RestartTransform;

    public AudioClip Aud_Pluggedin;
    public AudioClip Aud_Walk;
    public AudioClip Aud_Jump;
    public AudioClip Aud_Die;
    public AudioClip Aud_Shoot;
    public AudioClip Aud_NotPluggedin;

    // Start is called before the first frame update
    void Start()
    {
        PowerCol = new Color(255 / 255f, 187 / 255f, 0 / 255f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
