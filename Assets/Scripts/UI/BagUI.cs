using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : BasePanel
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private bool shouldUpdate = true; // 当物品数量出现变化时，更新背包UI

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
            Debug.Log("更新背包");
            int index = 0;
            foreach (KeyValuePair<Items, int> item in GameManager.Instance.itemsDict)
            { // 更新背包中已有道具的格子
                slots[index].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slots[index].transform.GetChild(0).GetComponent<Image>().sprite = item.Key.itemSprite;
                slots[index].transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 1);
                slots[index].transform.GetChild(1).GetComponent<Text>().text = item.Value.ToString();
                index++;
            }
            for (int i = GameManager.Instance.itemsDict.Count; i < slots.Length; i++)
            { // 清空背包中无道具的格子
                slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 0);
                slots[i].transform.GetChild(1).GetComponent<Text>().text = null;
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
