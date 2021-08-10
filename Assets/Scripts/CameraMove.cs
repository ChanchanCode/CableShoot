using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class CameraMove : MonoBehaviour
{
    float speed = 0.4f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CameraRoomMove();
    }
    void CameraRoomMove()
    {
        float viewx = Mathf.Clamp(Main_Action.Char.transform.position.x, Main_Action.viewborder.x + Main_Action.camerasize.x, Main_Action.viewborder.y - Main_Action.camerasize.x);
        float viewy = Mathf.Clamp(Main_Action.Char.transform.position.y, Main_Action.viewborder.z + Main_Action.camerasize.y, Main_Action.viewborder.w - Main_Action.camerasize.y);
        string s = viewx.ToString() + " : " + viewy.ToString();
        if (Main_Action.isroommove)
        {
            Vector3 cameradir = new Vector3(viewx, viewy + 0.25f, -10f) - transform.position;
            //Debug.Log(Vector3.Magnitude(transform.position - new Vector3(viewx, viewy + 0.25f, -10f)));
            if (cameradir.y > 0 && Mathf.Abs(cameradir.x) < 0.2f)
            {
                transform.position = new Vector3(transform.position.x, viewy + 0.25f, -10f);
            }
            if (Vector3.Magnitude(cameradir) < 0.4f)
            {
                transform.position = new Vector3(viewx, viewy + 0.25f, -10f);
                CameraShaker.Instance.RestPositionOffset = transform.position;
                Main_Action.isroommove = false;
            }
            else
            {
                Vector3 dir = (new Vector3(viewx, viewy + 0.25f, -10f) - transform.position).normalized;
                Main_Action.roommovedir = viewx - transform.position.x > 0.1f ? 1 : (viewx - transform.position.x < -0.1f ? -1 : 0);
                transform.position += dir * speed;
                CameraShaker.Instance.RestPositionOffset = transform.position;
            }
        }
        else
        {
           
            transform.position = Vector3.Lerp(transform.position, new Vector3(viewx, viewy + 0.25f, -10f), 7f * Time.fixedDeltaTime);
            CameraShaker.Instance.RestPositionOffset = transform.position;
        }
    }
}
