using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Inventory_Action : MonoBehaviour
{
    Color PowerCol;
    public GameObject Inventory;
    public GameObject InventoryPanel;
    public Transform InventoryItemTransform;
    public Transform InventoryInfoNameTransform;

    float inventoryshaketime;
    float inventoryinfoshaketime;

    public TextMeshProUGUI InventoryInfoText;
    public TextMeshProUGUI InventoryItemNameText;
    public GameObject InventoryItemPrefab;

    public List<string> ItemStringList;
    public List<GameObject> ItemGameObjectList;
    int selectitemnum;
    int shakedir;

    public Sprite testspr;

    public GameObject SelectPanel;
    public GameObject SelectYesNoPanel;
    public TextMeshProUGUI SelectInfoText;
    public TextMeshProUGUI SelectYesNoText;
    int selectstate;  //0 : 선택상태 아님 / 2 : 선택중 / 1 : 네 / -1 : 아니요
    bool isselectyes;
    int selecteditem;

    int selectshakedir;
    float selectpanelshaketime;
    float selectyesnopanelshaketime;

    // Start is called before the first frame update
    void Start()
    {
        PowerCol = new Color(200 / 255f, 200 / 255f, 200 / 255f);
        inventoryshaketime = 10f;
        inventoryinfoshaketime = 10f;
        selectpanelshaketime = 10f;
        selectyesnopanelshaketime = 10f;
        shakedir = 1;
        selectshakedir = 1;
        selectitemnum = 0;
        selectstate = 0;
        isselectyes = true;
        selecteditem = 0;
        InventoryItemCreate("소금구이덮밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("참치김밥");
        InventoryItemCreate("뉴뉴");

        InventorySetOrder();

        Main_Action.isInventoryOpened = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryshaketime < 5f)
        {
            InventoryItemTransform.rotation = Quaternion.Euler(0f, 0f, 1.5f * (Mathf.Cos(inventoryshaketime * 3f) / (inventoryshaketime * inventoryshaketime)));
            inventoryshaketime += 0.1f;

        }
        else if (inventoryshaketime != 10f)
        {
            InventoryItemTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            inventoryshaketime = 10f;
        }

        if (inventoryinfoshaketime < 5f)
        {
            InventoryInfoNameTransform.rotation = Quaternion.Euler(0f, 0f, shakedir * 1.5f * (Mathf.Cos(inventoryinfoshaketime * 3f) / (inventoryinfoshaketime * inventoryinfoshaketime)));
            inventoryinfoshaketime += 0.1f;

        }
        else if (inventoryinfoshaketime != 10f)
        {
            InventoryInfoNameTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            inventoryinfoshaketime = 10f;
        }

        if (selectpanelshaketime < 5f)
        {
            SelectPanel.transform.rotation = Quaternion.Euler(0f, 0f, 1.5f * (Mathf.Cos(selectpanelshaketime * 3f) / (selectpanelshaketime * selectpanelshaketime)));
            selectpanelshaketime += 0.1f;

        }
        else if (selectpanelshaketime != 10f)
        {
            SelectPanel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            selectpanelshaketime = 10f;
        }

        if (selectyesnopanelshaketime < 5f)
        {
            SelectYesNoPanel.transform.rotation = Quaternion.Euler(0f, 0f, selectshakedir * 1.5f * (Mathf.Cos(selectyesnopanelshaketime * 3f) / (selectyesnopanelshaketime * selectyesnopanelshaketime)));
            selectyesnopanelshaketime += 0.1f;

        }
        else if (selectyesnopanelshaketime != 10f)
        {
            SelectYesNoPanel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            selectyesnopanelshaketime = 10f;
        }

        if (selectstate == 1)
        {
            SelectAction(selecteditem);
            selectstate = 0;
            SelectPanel.SetActive(false);
            selectitemnum = Mathf.Clamp(selectitemnum, 1, ItemGameObjectList.Count);
            InventoryItemSelect(selectitemnum - 1);
        }
        else if (selectstate == -1)
        {
            selectstate = 0;
            SelectPanel.SetActive(false);
        }
    }

    public void InventoryOpen()
    {
        Inventory.SetActive(true);
        shakedir = 1;
        inventoryshaketime = 0.5f;
        inventoryinfoshaketime = 0.5f;
        Time.timeScale = 0f;
    }
    public void InventoryClose()
    {
        if (selectstate == 2)
        {
            selectstate = 0;
            SelectPanel.SetActive(false);
        }
        Inventory.SetActive(false);
        Time.timeScale = 1f;
    }
    public void InventoryItemCreate(string itemname)
    {
        Sprite spr = testspr;

        GameObject item = Instantiate(InventoryItemPrefab, InventoryPanel.transform);
        item.transform.GetChild(0).GetComponent<Image>().sprite = spr;
        item.GetComponent<Image>().color = Color.grey;
        item.transform.GetChild(0).GetComponent<Image>().color = Color.grey;
        ItemGameObjectList.Add(item);
        ItemStringList.Add(itemname);

        if (selectitemnum == 0)
        {
            selectitemnum = 1;
            InventoryItemSelect(0);
        }
    }
    public void InventorySetOrder()
    {
        float itemnum = ItemGameObjectList.Count;
        int x = 0;
        int y = 0;
        int itemrow = Mathf.CeilToInt(itemnum / 9f);
        foreach (GameObject item in ItemGameObjectList)
        {
            if (itemnum < 9f)
            {
                item.GetComponent<RectTransform>().anchoredPosition = new Vector2(-45f * (itemnum - 1) + 90f * x, 45f * (itemrow - 1) - 90f * y);
            }
            else
            {
                item.GetComponent<RectTransform>().anchoredPosition = new Vector2(-45f * 8f + 90f * x, 45f * (itemrow - 1) - 90f * y);
            }
            x++;

            if (x >= 9)
            {
                y++;
                x = 0;
            }
        }
    }
    public void InventoryControl(string input)
    {
        if (ItemGameObjectList.Count > 0)
        {
            int prefselect = selectitemnum;
            if (selectitemnum == 0) selectitemnum = 1;
            if (selectstate == 0)
            {
                if (input == "right")
                {
                    selectitemnum++;
                }
                else if (input == "left")
                {
                    selectitemnum--;
                }
                else if (input == "up")
                {
                    if (selectitemnum > 9) selectitemnum -= 9;

                }
                else if (input == "down")
                {
                    if (selectitemnum + 9 <= ItemGameObjectList.Count) selectitemnum += 9;
                }
                selectitemnum = Mathf.Clamp(selectitemnum, 1, ItemGameObjectList.Count);
                if (input == "select")
                {
                    SelectTextOn(selectitemnum - 1);
                    selectpanelshaketime = 0.5f;
                    return;
                }
            }
            else if (selectstate == 2)
            {
                if (input == "right")
                {
                    isselectyes = !isselectyes;
                    selectshakedir = -1 * selectshakedir;
                    selectyesnopanelshaketime = 0.5f;
                }
                else if (input == "left")
                {
                    isselectyes = !isselectyes;
                    selectshakedir = -1 * selectshakedir;
                    selectyesnopanelshaketime = 0.5f;
                }
                if (isselectyes) SelectYesNoText.text = "<color=white>네  /</color>  <color=#808080ff>아니요</color>";
                else SelectYesNoText.text = "<color=#808080ff>네  </color><color=white>/  아니요</color>";
                if (input == "select")
                {
                    if (isselectyes) 
                    {
                        selectstate = 1;
                        shakedir = 1;
                        inventoryshaketime = 0.5f;
                        inventoryinfoshaketime = 0.5f;
                    }
                    else selectstate = -1;
                }
            }

            if (selectitemnum != prefselect)
            {
                InventoryItemSelect(selectitemnum - 1);
                shakedir = shakedir * -1;
                inventoryinfoshaketime = 0.5f;
            }
        }
        else
        {
            selectitemnum = 0;
        }
    }
    void InventoryItemSelect(int index)
    {
        for (int i = 0; i < ItemGameObjectList.Count; i++)
        {
            ItemGameObjectList[i].GetComponent<Image>().color = index == i ? PowerCol : Color.grey;
            ItemGameObjectList[i].transform.GetChild(0).GetComponent<Image>().color = index == i ? Color.white : Color.grey;
            if (index == i)
            {
                ItemGameObjectList[i].GetComponent<Image>().color = PowerCol;
                ItemGameObjectList[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                InventoryItemNameText.text = ItemStringList[i];
                InventoryInfoText.text = ItemInfoData(ItemStringList[i]);
            }
            else
            {
                ItemGameObjectList[i].GetComponent<Image>().color = Color.grey;
                ItemGameObjectList[i].transform.GetChild(0).GetComponent<Image>().color = Color.grey;
            }

        }
    }
    void SelectTextOn(int itemindex)
    {
        selectshakedir = 1;
        string itemname = ItemStringList[itemindex];
        if (itemname == "소금구이덮밥")
        {
            selectstate = 2;
            selecteditem = itemindex;
            isselectyes = true;
            SelectYesNoText.text = "<color=white>네  /</color>  <color=#808080ff>아니요</color>";
            SelectPanelOn("맛있는 소금구이덮밥을 드시겠습니까??");
        }
    }
    void SelectAction(int itemindex)
    {
        string itemname = ItemStringList[itemindex];
        if (itemname == "소금구이덮밥")
        {
            Main_Action.maxhp += 1;
            Main_Action.hp += 1;
            ItemStringList.RemoveAt(itemindex);
            ItemGameObjectList.RemoveAt(itemindex);
            InventorySetOrder();
        }

    }


    public void SelectPanelOn(string info)
    {
        SelectInfoText.text = info;
        SelectPanel.SetActive(true);
    }

    string ItemInfoData(string itemname)
    {
        if (itemname == "소금구이덮밥") return "맛있는 소금구이덮밥이다.";
        return "엥 오륜데요";
    }
}
