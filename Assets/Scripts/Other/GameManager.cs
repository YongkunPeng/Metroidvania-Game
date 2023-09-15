using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public bool shouldTransmit; // 是否需要将玩家移动到出生点
    public bool isPaused; // 记录游戏是否暂停
    public Dictionary<int, string> slotDict; // 物品槽所含物品
    public Dictionary<string, int> itemsDict; // 玩家背包数据
    public Dictionary<string, Items> resourceDict = new Dictionary<string, Items>();
    private Items[] itemsResource;
    public List<Mission> missionList = new List<Mission>(); // 玩家任务数据
    public SlotController[] slots;
    public UserData userData;
    public string username;

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
                }
            }
            return _Instance;
        }
    }

    private void OnEnable()
    {
        // 加载所有ScriptsObject物品数据
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
        
    }

    private void FixedUpdate()
    {
        #region 更新收集任务状态
        foreach ( Mission mission in missionList )
        {
            if (mission.missionStatus == Mission.MissionStatus.Accepted && mission.missionType == Mission.MissionType.Collect && itemsDict.ContainsKey(mission.collectItemName))
            { // 物品数量字典中有该物体，且任务未完成
                if (itemsDict[mission.collectItemName] >= mission.goalCnt)
                {
                    mission.UpdateMissionComplete();
                }
            }
            else if (mission.missionStatus == Mission.MissionStatus.Completed && mission.missionType == Mission.MissionType.Collect)
            { // 对于收集任务，进度完成但未领奖，期间数量有变化导致未达标时
                if (!itemsDict.ContainsKey(mission.collectItemName) || itemsDict[mission.collectItemName] < mission.goalCnt)
                {
                    mission.UpdateMissionAccept();
                }
            }
        }
        #endregion
    }


    /// <summary>
    /// 攻击顿帧
    /// </summary>
    /// <param name="duration"></param>
    public void HitPause(int duration)
    {
        StartCoroutine(Pause(duration));
    }

    /// <summary>
    /// 向背包数据中添加物品，返回该物体是否可被拾取
    /// </summary>
    /// <param name="item">待添加的物品信息</param>
    /// <param name="cnt">添加数量</param>
    public bool AddItem(Items item, int cnt)
    {
        if (itemsDict.ContainsKey(item.itemName) && itemsDict[item.itemName] + cnt <= 99)
        { // 背包中已经存在该物品，且拾取后数量小于等于99，则数量+cnt
            itemsDict[item.itemName] += cnt;
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // 背包UI打开时，更新背包显示信息
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            return true;
        }
        else if (!itemsDict.ContainsKey(item.itemName) && slotDict.Count < 10 && itemsDict.Count < 10 && cnt <= 99)
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
                    itemsDict.Add(item.itemName, cnt);
                    return true;
                }
            }
            TipsBoxManager.Instance.ShowTipsBox("无法获取物品：<color=red>" + item.itemName + "</color>，已无空位", 3f);
            return false;
        }
        TipsBoxManager.Instance.ShowTipsBox("无法获取物品：<color=red>" + item.itemName + "</color>该物品已超出上限", 3f);
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

    /// <summary>
    /// 玩家数据本地化存储
    /// </summary>
    /// <param name="life">玩家生命值</param>
    /// <param name="coinCnt">玩家金币数</param>
    public void SaveUserData(float life, int coinCnt, int sceneID, int checkPointID)
    {
        userData.health = life;
        userData.coinCnt = coinCnt;
        userData.sceneID = sceneID;
        userData.checkPointID = checkPointID;
        LocalConfig.SaveUserData(userData);
        InitUserData(userData, false); // 保存成功后同步Gamemanager内的玩家数据
    }

    /// <summary>
    /// 加载玩家数据并根据bool跳转到游戏场景
    /// </summary>
    /// <param name="userData">玩家数据UserData</param>
    /// <param name="isLoad">是否跳转场景</param>
    public void InitUserData(UserData userData, bool isLoad = false)
    {
        this.userData = userData;
        username = userData.username;
        slotDict = userData.slotDict;
        itemsDict = userData.itemsDict;
        missionList = userData.missionList;
        if (isLoad)
        {
            SceneLoadManager.Instance.LoadLevelByIndexWithSlider(userData.sceneID);
        }
    }

    /// <summary>
    /// 攻击顿帧
    /// </summary>
    /// <param name="duration">停顿时间</param>
    /// <returns></returns>
    IEnumerator Pause(int duration)
    {
        float pauseTime = duration / 60f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }
}
