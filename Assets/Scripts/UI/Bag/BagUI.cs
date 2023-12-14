using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : BasePanel
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private bool shouldUpdate = true; // 当物品数量出现变化时，更新背包UI
    [SerializeField] private GameObject item;

    private void Awake()
    {
        OpenPanel(UIConst.PlayerBag);
        GameManager.Instance.isPaused = true;
        Time.timeScale = 0f;

        slots = new GameObject[transform.GetChild(0).GetChild(1).childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = transform.GetChild(0).GetChild(1).GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldUpdate)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].transform.childCount == 2)
                { // 删除所有槽内无物体的item
                    slots[i].transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Destroy(slots[i].transform.GetChild(1).gameObject); //Destroy存在延迟，导致后方更新物品数量图片出现问题，先隐藏
                }
            }
            foreach (KeyValuePair<int, string> pair in GameManager.Instance.slotDict)
            {
                // 生成对应数量的item
                GameObject.Instantiate(item, slots[pair.Key].transform);

                // 更新背包中已有道具的格子，即slot下最后一个子物体
                slots[pair.Key].transform.GetChild(slots[pair.Key].transform.childCount-1).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slots[pair.Key].transform.GetChild(slots[pair.Key].transform.childCount - 1).GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.resourceDict[pair.Value].itemSprite;
                slots[pair.Key].transform.GetChild(slots[pair.Key].transform.childCount - 1).GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, 1);
                slots[pair.Key].transform.GetChild(slots[pair.Key].transform.childCount - 1).GetChild(0).GetChild(0).GetComponent<Text>().text = GameManager.Instance.itemsDict[pair.Value].ToString();
            }
            shouldUpdate = false;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            CloseBag();
        }
    }

    private void CloseBag()
    {
        if (!isRemove)
        {
            ClosePanel();
            GameManager.Instance.isPaused = false;
            Time.timeScale = 1f;
        }
    }

    public void UpdateBagUI()
    {
        shouldUpdate = true;
    }
}
