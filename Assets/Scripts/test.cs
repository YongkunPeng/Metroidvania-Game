using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public GameObject bagUI;
    public GameObject[] slots;
    public Items item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
        {
            bagUI = GameObject.FindGameObjectWithTag("Bag");
            slots = new GameObject[bagUI.transform.GetChild(0).GetChild(1).childCount];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = bagUI.transform.GetChild(0).GetChild(1).GetChild(i).gameObject;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                slots[0].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slots[0].transform.GetChild(0).GetComponent<Image>().sprite = item.itemSprite;

                slots[0].transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 1);
                slots[0].transform.GetChild(1).GetComponent<Text>().text = 5.ToString();
            }
        }
        else
        {
            bagUI = null;
            slots = null;
        }
    }
}
