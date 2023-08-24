using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public bool isPaused; // 记录游戏是否暂停
    public Dictionary<int, string> slotDict = new Dictionary<int, string>(); // 物品槽所含物品
    public Dictionary<string, int> itemsDict = new Dictionary<string, int>(); // 玩家背包数据
    public Dictionary<string, Items> resourceDict = new Dictionary<string, Items>();
    private Items[] itemsResource;
    public List<Mission> missionList = new List<Mission>(); // 玩家任务数据
    public SlotController[] slots;
    // public UserData userData;
    // public string username;

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

    private void OnEnable()
    {
        // userData = LocalConfig.LoadUserData(username); // 加载用户数据

        itemsResource = Resources.LoadAll<Items>("ItemData");
        foreach (Items item in itemsResource)
        {
            resourceDict.Add(item.itemName, item);
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
        if (itemsDict.ContainsKey(item.itemName) && itemsDict[item.itemName] < 99)
        { // 背包中已经存在该物品，且数量小于99，则数量+1
            itemsDict[item.itemName] += 1;
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // 背包UI打开时，更新背包显示信息
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            return true;
        }
        else if (!itemsDict.ContainsKey(item.itemName) && slotDict.Count < 10 && itemsDict.Count < 10)
        { // 背包中不含该物品，且还有空间
            for (int i = 0; i < 10; i++)
            {
                bool canUseSlot = true; // 表示该槽是否为空
                foreach(KeyValuePair<int, string> pair in slotDict)
                {
                    if (i == pair.Key)
                    { // 该槽已被占用，跳出内循环，进入下一个外循环
                        canUseSlot = false;
                        break;
                    }
                }

                if (canUseSlot)
                { // 当前i所指物品槽未被占用，跳出循环
                    slotDict.Add(i, item.itemName);
                    itemsDict.Add(item.itemName, 1);
                    return true;
                }
            }
            Debug.LogWarning("无法拾取物品，已无空位！");
            return false;
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
        if (itemsDict.ContainsKey(item.itemName))
        { // 存在该物品
            if (itemsDict[item.itemName] > 1)
            { // 数量大于1
                itemsDict[item.itemName] -= 1;
            }
            else
            { // 数量等于1
                foreach (KeyValuePair<int, string> pair in slotDict)
                {
                    if (pair.Value == item.itemName)
                    {
                        slotDict.Remove(pair.Key);
                        break;
                    }
                }
                itemsDict.Remove(item.itemName);
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
                if (slotDict.ContainsKey(i))
                {
                    slots[i].slotItem = resourceDict[slotDict[i]];
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
