using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : BasePanel
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private bool shouldUpdate = true; // ����Ʒ�������ֱ仯ʱ�����±���UI
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
                { // ɾ�����в����������item
                    slots[i].transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Destroy(slots[i].transform.GetChild(1).gameObject); //Destroy�����ӳ٣����º󷽸�����Ʒ����ͼƬ�������⣬������
                }
            }
            foreach (KeyValuePair<int, string> pair in GameManager.Instance.slotDict)
            {
                // ���ɶ�Ӧ������item
                GameObject.Instantiate(item, slots[pair.Key].transform);

                // ���±��������е��ߵĸ��ӣ���slot�����һ��������
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
