using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main_Action : MonoBehaviour
{
    public static Color PowerCol;
    public static int stage = 1;
    public static float borderx = -63f;
    public static float bordery = 0f;
    public static int attempts = 0;
    public static float second = 0;
    public static int minute = 0;
    public static int hour = 0;

    public GameObject RestartPositionManager;
    public List<GameObject> RestartPosition;
    public List<string> stageinfostring;

    public AudioClip Aud_Pluggedin;
    public AudioClip Aud_Walk;
    public AudioClip Aud_Jump;
    public AudioClip Aud_Die;
    public AudioClip Aud_Shoot;
    public AudioClip Aud_NotPluggedin;

    public Text StageInfoText;
    public Text TimerText;
    public Text StageText;
    public Text AttemptsText;

    public int RestartPositionManagerChildnum;

    string secondtext;
    string tt;
    string prevsecondtext;
    // Start is called before the first frame update
    void Start()
    {
        
        RestartPosition.Clear();
        stageinfostring.Clear();
        RestartPositionManagerChildnum = RestartPositionManager.transform.childCount;
        for (int i = 0; i < RestartPositionManagerChildnum; i++)
        {
            RestartPosition.Add(RestartPositionManager.transform.GetChild(i).gameObject);
            stageinfostring.Add(RestartPosition[i].GetComponent<StageInfo>().info);
        }
        StageSetting();
        PowerCol = new Color(255 / 255f, 187 / 255f, 0 / 255f);
        AttemptsText.text = "ATTEMPTS: " + attempts.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        second += Time.deltaTime;
        if (second >= 60)
        {
            second -= 60;
            minute++;
        }
        if (minute >= 60)
        {
            minute -= 60;
            hour++;
        }

        secondtext = Mathf.FloorToInt(second).ToString();
        if (secondtext.Length == 1)
        {
            secondtext = "0" + secondtext;
        }
        if (secondtext != prevsecondtext)
        {
            if (hour == 0)
            {
                TimerText.text = (minute.ToString().Length == 1 ? "0" + minute.ToString() : minute.ToString()) + ":" + secondtext;
            }
            else
            {
                TimerText.text = hour.ToString() + ":" + (minute.ToString().Length == 1 ? "0" + minute.ToString() : minute.ToString()) + ":" + secondtext;
            }
            prevsecondtext = secondtext;
        }
    }
    public void StageSetting()
    {
        StageText.text = "Stage " + stage.ToString();
        if (StageInfoText.text != stageinfostring[Mathf.Clamp(stage, 1, RestartPositionManagerChildnum) - 1])
        {
            StageInfoText.text = stageinfostring[Mathf.Clamp(stage, 1, RestartPositionManagerChildnum) - 1];
        }
    }
}
