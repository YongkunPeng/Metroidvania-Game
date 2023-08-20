using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public bool isPaused; // 记录游戏是否暂停
    public Dictionary<Items, int> itemsDict = new Dictionary<Items, int>();
    public SlotController[] slots;

    public static GameManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<GameManager>();
                if (_Instance == null)
                {
                    GameObject gameObject = new GameObject("GameManager");
                    _Instance = gameObject.AddComponent<GameManager>();
                    DontDestroyOnLoad(gameObject);
                }
            }
            return _Instance;
        }
    }

    private void Awake()
    {
        // 确保只有一个实例存在
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 其他初始化逻辑
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
    }

    /// <summary>
    /// 向背包数据中添加物品，返回该物体是否可被拾取
    /// </summary>
    /// <param name="item">待添加的物品信息</param>
    public bool AddItem(Items item)
    {
        if (itemsDict.ContainsKey(item) && itemsDict[item] < 99)
        { // 背包中已经存在该物品，且数量小于99，则数量+1
            itemsDict[item] += 1;
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // 背包UI打开时，更新背包显示信息
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            return true;
        }
        else
        {
            if (itemsDict.Count <= 10 && !itemsDict.ContainsKey(item))
            { // 不存在该物体且空间有余
                itemsDict.Add(item, 1);
                if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
                { // 背包UI打开时，更新背包显示信息
                    GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
                }
                return true;
            }
        }
        Debug.LogWarning("无法拾取物品，该物品已超出上限！");
        return false;
    }


    /// <summary>
    /// 将背包数据中的指定物品丢弃，使之数量-1
    /// </summary>
    /// <param name="item">待添加的物品信息</param>
    public void RemoveItem(Items item)
    {
        if (itemsDict.ContainsKey(item))
        { // 存在该物品
            if (itemsDict[item] > 1)
            { // 数量大于1
                itemsDict[item] -= 1;
            }
            else
            { // 数量等于1
                itemsDict.Remove(item);
            }
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // 背包UI打开时，更新背包显示信息
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            ResetItem();
        }
        else
        { // 不存在该物体
            Debug.LogError("无法丢弃不存在的物体");
        }
    }

    /// <summary>
    /// 重置物品槽脚本中的slotItem
    /// </summary>
    public void ResetItem()
    {
        if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
        {
            #region 获取背包UI中的各个物品槽脚本
            GameObject bagUI = GameObject.FindGameObjectWithTag("Bag");
            slots = new SlotController[bagUI.transform.GetChild(0).GetChild(1).childCount];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = bagUI.transform.GetChild(0).GetChild(1).GetChild(i).GetComponent<SlotController>();
            }
            #endregion

            #region 更新各个物品槽的slotItem的值
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < itemsDict.Count)
                { // 槽内有物品
                    int index = 0;
                    foreach (KeyValuePair<Items, int> keyValue in itemsDict)
                    {
                        if (slots[i].slotID == index)
                        {
                            slots[i].slotItem = keyValue.Key;
                        }
                        index++;
                    }
                }
                else
                { // 槽内无物品
                    slots[i].slotItem = null;
                }
            }
            #endregion
        }
    }
}
