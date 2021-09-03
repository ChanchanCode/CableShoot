using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public GameObject DialoguePanelObject;
    public GameObject YesNoDialoguePanelObject;
    public TextMeshProUGUI DialogueNameText;
    public TextMeshProUGUI DialogueText;
    public TextMeshProUGUI YesNoText;
    public Image DialogueImage;
    public RectTransform NextButtonTransform;
    public Vector2 NextButtonPos;
    public float Nextbuttontime;
    public bool isskipmode;
    public bool isyesselected;

    int ARRAYSIZE = 100;
    string[] CutSceneArray;

    [System.Serializable]
    public struct PadangStructure
    {
        public Sprite Subak;
        public Sprite BBISuung;
    }

    [System.Serializable]
    public struct CharStructure
    {
        public Sprite smile;                    
    }
    [System.Serializable]
    public struct PropoStructure
    {
        public Sprite idle;
    }

    public PadangStructure PadangSprite;
    public CharStructure CharSprite;
    public PropoStructure PropoSprite;
    //Powercol HexCode : <color=#c8c8c8ff> </color>
    // Start is called before the first frame update
    void Start()
    {
        CutSceneArray = new string[ARRAYSIZE];
        Nextbuttontime = 0f;
        isskipmode = false;
        isyesselected = true;
        NextButtonPos = new Vector2(NextButtonTransform.anchoredPosition.x, NextButtonTransform.anchoredPosition.y);
        GenerateCutScene();
    }

    void GenerateCutScene()
    {
        CutSceneArray[0] = "CutScene";
    }

    // Update is called once per frame
    void Update()
    {
        if (!isskipmode && iskeypressedDown) isskipmode = true;
    }
    public bool iskeypressed
    {
        get {
            return Input.GetKey(KeyCode.Z)
              || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return)
              || Input.GetMouseButton(0) || Input.GetMouseButton(1);
        }
    }
    public bool iskeypressedDown
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Z)
              || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)
              || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        }
    }
    public IEnumerator WaitForKeyPress(bool isnextbuttonactive)
    {
        bool keyup = false;
        if (isnextbuttonactive)
        {
            NextButtonTransform.gameObject.SetActive(true);
            Nextbuttontime = 0;
        }

        while (true)
        {
            if (!keyup && !iskeypressed)
            {
                keyup = true;
            }
            if (isnextbuttonactive)
            {
                Nextbuttontime += 3f * Time.deltaTime;
                NextButtonTransform.anchoredPosition = new Vector2(NextButtonPos.x, NextButtonPos.y + 15f * Mathf.Sin(Nextbuttontime));
            }
            if (iskeypressedDown && keyup)
            {
                break;
            }
            yield return null;
        }
        DialoguePanelObject.SetActive(false);
        yield break;
    }
    public IEnumerator Dialogue(string name, string sentence, Sprite sprite)
    {
        bool highlighted = false;
        isskipmode = false;
        NextButtonTransform.gameObject.SetActive(false);
        YesNoDialoguePanelObject.SetActive(false);
        DialogueNameText.text = name;
        DialogueText.text = "";
        DialogueImage.sprite = sprite;
        DialoguePanelObject.SetActive(true);
        StopCoroutine("ShakePanel");
        StartCoroutine(ShakePanel());
        foreach (char letter in sentence)
        {
            if (letter == '*')
            {
                highlighted = !highlighted;
            }
            else
            {
                if (highlighted) DialogueText.text += "<color=#c8c8c8ff>" + letter + "</color>";
                else DialogueText.text += letter;
            }
            if (isskipmode)
            {
                DialogueText.text = StringHighlightExchange(sentence);
                yield return StartCoroutine(WaitForKeyPress(true));
                yield break;
            }
            else 
            {
                if (letter == '*' || letter == ' ') continue;
                else
                {
                    if (letter == '\n') yield return new WaitForSeconds(0.1f);
                    else yield return new WaitForSeconds(0.05f);
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(WaitForKeyPress(true));
        yield break;
    }

    public IEnumerator WaitForSelectYesNo()
    {
        DialoguePanelObject.SetActive(true);
        bool keyup = false;
        YesNoText.text = "<color=white>네  /</color>  <color=#808080ff>아니요</color>";
        Nextbuttontime = 0;
        while (true)
        {
            if (!keyup && !iskeypressed)
            {
                keyup = true;
            }
            if (keyup)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    isyesselected = !isyesselected;
                    StopCoroutine("ShakeYesNoPanel");
                    StartCoroutine(ShakeYesNoPanel());
                    if (isyesselected) YesNoText.text = "<color=white>네  /</color>  <color=#808080ff>아니요</color>";
                    else YesNoText.text = "<color=#808080ff>네  </color><color=white>/  아니요</color>";
                }
                if (iskeypressedDown)
                {
                    break;
                }
            }
            yield return null;
        }
        DialoguePanelObject.SetActive(false);
        yield break;
    }

    public IEnumerator YesNoDialogue(string name, string sentence, Sprite sprite)
    {
        bool highlighted = false;
        isskipmode = false;
        NextButtonTransform.gameObject.SetActive(false);
        DialogueNameText.text = name;
        DialogueText.text = "";
        DialogueImage.sprite = sprite;
        DialoguePanelObject.SetActive(true);
        StopCoroutine("ShakePanel");
        StartCoroutine(ShakePanel());
        foreach (char letter in sentence)
        {
            if (letter == '*')
            {
                highlighted = !highlighted;
            }
            else
            {
                if (highlighted)
                {
                    DialogueText.text += "<color=#c8c8c8ff>" + letter + "</color>";
                }
                else DialogueText.text += letter;
            }
            if (isskipmode)
            {
                DialogueText.text = StringHighlightExchange(sentence);
                yield return StartCoroutine(WaitForKeyPress(false));
                YesNoDialoguePanelObject.SetActive(true);
                isyesselected = true;
                yield return StartCoroutine(WaitForSelectYesNo());
                yield break;
            }
            else
            {
                if (letter == '*' || letter == ' ') continue;
                else
                {
                    if (letter == '\n') yield return new WaitForSeconds(0.1f);
                    else yield return new WaitForSeconds(0.05f);
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(WaitForKeyPress(false));
        YesNoDialoguePanelObject.SetActive(true);
        isyesselected = true;
        yield return StartCoroutine(WaitForSelectYesNo());
        yield break;
    }
    IEnumerator ShakePanel()
    {
        DialoguePanelObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        float shaketime = 0.5f;
        float zrot = 0f;
        while (shaketime <= 5f)
        {
            zrot = 1f * (Mathf.Cos(shaketime * 3f) / (shaketime * shaketime));
            DialoguePanelObject.transform.rotation = Quaternion.Euler(0f, 0f, zrot);
            shaketime += 0.1f;
            yield return null;
        }
        DialoguePanelObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        yield break;
    }

    IEnumerator ShakeYesNoPanel()
    {
        YesNoDialoguePanelObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        float shaketime = 0.5f;
        float zrot = 0f;
        while (shaketime <= 5f)
        {
            zrot = (isyesselected ? 1 : -1) * (Mathf.Cos(shaketime * 3f) / (shaketime * shaketime));
            YesNoDialoguePanelObject.transform.rotation = Quaternion.Euler(0f, 0f, zrot);
            shaketime += 0.1f;
            yield return null;
        }
        YesNoDialoguePanelObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        yield break;
    }
    public void CutsceneStartSetting()
    {
        Main_Action.Char.GetComponent<Char_Action>().PlugFail();
        Main_Action.Char.GetComponent<Char_Action>().Plug.SetActive(false);
        Main_Action.Char.GetComponent<Char_Action>().state = State.Cutscene;
    }
    public void CutsceneEndSetting()
    {
        Main_Action.Char.GetComponent<Char_Action>().state = State.Normal;
    }

    string StringHighlightExchange(string text)
    {
        string ret = null;
        bool highlighted = false;
        foreach(char s in text)
        {
            if (s == '*')
            {
                if (!highlighted)
                {
                    ret += "<color=#c8c8c8ff>";
                    highlighted = true;
                }
                else
                {
                    ret += "</color>";
                    highlighted = true;
                }
            }
            else
            {
                ret += s;
            }
        }
        return ret;
    }

    public IEnumerator CutScene()
    {
        CutsceneStartSetting();
        yield return Dialogue("프로포", "뭐야, 처음 보는 얼굴이네.", PropoSprite.idle);
        yield return Dialogue("토깽이", "...", CharSprite.smile);
        yield return YesNoDialogue("프로포", "할 일 없으면 가서 *식료품 상자*나 옮기지?", PropoSprite.idle);
        if (isyesselected) yield return Dialogue("프로포", "다녀와.", PropoSprite.idle);
        else yield return Dialogue("프로포", "..?", PropoSprite.idle);
        CutsceneEndSetting();
    }

    public void RunCutScene(int interactnum)
    {
        if (interactnum >= 0 && interactnum < ARRAYSIZE && CutSceneArray[interactnum] != null)
        {
            StartCoroutine(CutSceneArray[interactnum]);
        }
        else
        {
            StartCoroutine(CutSceneError());
        }
    }
    public IEnumerator CutSceneError()
    {
        CutsceneStartSetting();
        yield return Dialogue("에러 메세지", "음.. 오류가 난 것 같은데요?", PadangSprite.Subak);
        CutsceneEndSetting();
    }
}