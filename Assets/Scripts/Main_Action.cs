using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Main_Action : MonoBehaviour
{
    public static Color PowerCol;
    public static int stage = 1;
    public static int attempts = 0;
    public static float second = 0;
    public static int minute = 0;
    public static int hour = 0;
    public static string Entereddir = "left";
    public int maxhp = 2;
    public int hp = 1;
    int mainmaxhp;
    int mainhp;

    public GameObject RestartPositionManager;
    public List<GameObject> RestartPosition;
    public List<string> stageinfostring;

    public static GameObject Char;
    public static CutsceneManager CM;
    public static float roommovedir;
    public static Vector4 viewborder;
    public static Vector2 camerasize;
    public static bool isroommove;

    public static bool isplatformflipped;
    float flippingtime;

    public AudioClip Aud_Pluggedin;
    public AudioClip Aud_Walk;
    public AudioClip Aud_Jump;
    public AudioClip Aud_Die;
    public AudioClip Aud_Shoot;
    public AudioClip Aud_NotPluggedin;

    public int RestartPositionManagerChildnum;

    public GameObject TileMap;
    public TextMeshProUGUI BatteryText;
    public GameObject BatteryPanel;
    public GameObject BatteryPrefab;
    public GameObject BatteryBlackPanel;
    public Transform BatteryTransform;

    public List<GameObject> Batteries;
    Vector2 BatteryOriginalPos = new Vector3(26.8f, 0f);
    float BatteryXoffset = 57.4f - 26.8f;
    // Start isalled before the first frame update
    private void Awake()
    {
        Char = GameObject.Find("Char");
        CM = GameObject.Find("CutSceneManager").GetComponent<CutsceneManager>();
    }
    void Start()
    {
        viewborder = new Vector4(-6f, 6f, -3.75f, 3.75f);
        CM = GameObject.Find("CutSceneManager").GetComponent<CutsceneManager>();
        Char = GameObject.Find("Char");
        roommovedir = 0f;
        camerasize = new Vector2(12f, 7.5f) / 2;
        isroommove = false;
        RestartPosition.Clear();
        stageinfostring.Clear();
        RestartPositionManagerChildnum = RestartPositionManager.transform.childCount;
        for (int i = 0; i < RestartPositionManagerChildnum; i++)
        {
            RestartPosition.Add(RestartPositionManager.transform.GetChild(i).gameObject);
            stageinfostring.Add(RestartPosition[i].GetComponent<StageInfo>().info);
        }
        //StageSetting();
        PowerCol = new Color(200 / 255f, 200 / 255f, 200 / 255f);
    }

    // Update is called once per frame
    void Update()
    {
        //SettingUpdate();
        if (isplatformflipped)
        {
            flippingtime -= Time.deltaTime;
            if (flippingtime < 0f) isplatformflipped = false;
        }
        if (mainmaxhp != maxhp || mainhp != hp)
        {
            mainmaxhp = maxhp;
            mainhp = hp;
            BatteryUpdate();
        }
    }
    public void PlatformDownJump()
    {
        isplatformflipped = true;
        flippingtime = 0.3f;
    }

    void BatteryUpdate()
    {
        if (Batteries.Count < maxhp)
        {
            BatteryCreate();
        }
        else if (Batteries.Count > maxhp)
        {
            BatteryPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(88.45f, 53f) + (maxhp - 2) * new Vector2(BatteryXoffset, 0f);
        }
        float xpossum = 0;
        for (int i = 0; i < Batteries.Count; i++)
        {
            if (i < hp)
            {
                Batteries[i].SetActive(true);
                xpossum += Batteries[i].GetComponent<RectTransform>().anchoredPosition.x;
            }
            else if (i < maxhp)
            {
                Batteries[i].SetActive(false);
                xpossum += Batteries[i].GetComponent<RectTransform>().anchoredPosition.x;
            }
            else
            {
                Batteries[i].SetActive(false);
            }
        }
        BattteryTextLocate(xpossum);
        BatteryText.text = hp.ToString() + "/" + maxhp.ToString();
    }
    void BatteryCreate()
    {
        while (Batteries.Count != maxhp)
        {
            GameObject B = Instantiate(BatteryPrefab, BatteryTransform);
            B.GetComponent<RectTransform>().anchoredPosition = BatteryOriginalPos + new Vector2(BatteryXoffset, 0f) * Batteries.Count;
            Batteries.Add(B);
        }
        BatteryPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(88.45f, 53f) + (maxhp - 2) * new Vector2(BatteryXoffset, 0f);

    }
    void BattteryTextLocate(float xpossum)
    {
        float m = maxhp;
        if (maxhp >= 10)
        {
            BatteryBlackPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(89.2f, BatteryBlackPanel.GetComponent<RectTransform>().sizeDelta.y);
            BatteryBlackPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(xpossum / m - 6f, 0f, 0f);
        }
        else
        {
            BatteryBlackPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(59.11f, BatteryBlackPanel.GetComponent<RectTransform>().sizeDelta.y);
            BatteryBlackPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(xpossum / m - 5.7f, 0f, 0f);
        }

    }
}
  