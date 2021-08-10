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

    public PadangStructure PadangSprite;
    public CharStructure CharSprite;


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
        isskipmode = false;
        NextButtonTransform.gameObject.SetActive(false);
        YesNoDialoguePanelObject.SetActive(false);
        DialogueNameText.text = name;
        DialogueText.text = "";
        DialogueImage.sprite = sprite;
        DialoguePanelObject.SetActive(true);
        foreach (char letter in sentence)
        {
            DialogueText.text += letter;
            if (isskipmode)
            {
                DialogueText.text = sentence;
                yield return StartCoroutine(WaitForKeyPress(true));
                yield break;
            }
            else 
            {
                if (letter == '\n') yield return new WaitForSeconds(0.2f);
                else yield return new WaitForSeconds(0.05f);
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
        isskipmode = false;
        NextButtonTransform.gameObject.SetActive(false);
        DialogueNameText.text = name;
        DialogueText.text = "";
        DialogueImage.sprite = sprite;
        DialoguePanelObject.SetActive(true);
        foreach (char letter in sentence)
        {
            DialogueText.text += letter;
            if (isskipmode)
            {
                DialogueText.text = sentence;
                yield return StartCoroutine(WaitForKeyPress(false));
                YesNoDialoguePanelObject.SetActive(true);
                isyesselected = true;
                yield return StartCoroutine(WaitForSelectYesNo());
                yield break;
            }
            else
            {
                if (letter == '\n') yield return new WaitForSeconds(0.1f);
                else yield return new WaitForSeconds(0.05f);
            }
        }
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(WaitForKeyPress(false));
        YesNoDialoguePanelObject.SetActive(true);
        isyesselected = true;
        yield return StartCoroutine(WaitForSelectYesNo());
        yield break;
    }

    public void CutsceneStartSetting()
    {
        Main_Action.Char.GetComponent<Char_Action>().PlugFail();
        Main_Action.Char.GetComponent<Char_Action>().Plug.SetActive(false);
        Main_Action.Char.GetComponent<Char_Action>().state = State.Cutscene;
        Debug.Log("시작");
    }
    public void CutsceneEndSetting()
    {
        Main_Action.Char.GetComponent<Char_Action>().state = State.Normal;
        Debug.Log("끝");
    }


    public IEnumerator CutScene()
    {
        CutsceneStartSetting();
        yield return Dialogue("파댕파댕", "ㅋㅋ안녕하세요~~~\n이번에 처음 들어온 신입이에요!!", PadangSprite.Subak);
        yield return Dialogue("창범", "..!", CharSprite.smile);
        yield return YesNoDialogue("파댕파댕", "혹시 밥 드실래요?", PadangSprite.Subak);
        if (isyesselected) yield return Dialogue("파댕파댕", "같이 먹죠!", PadangSprite.BBISuung);
        else yield return Dialogue("파댕파댕", "어이없네..\n먹지마세요", PadangSprite.BBISuung);
        CutsceneEndSetting();
    }

    public void RunCutScene(int interactnum)
    {
        if (interactnum >= 0 && interactnum < ARRAYSIZE && CutSceneArray[interactnum] != null)
        {
            StartCoroutine(CutSceneArray[interactnum]);
            Debug.Log("zz");
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