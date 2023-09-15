using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager _Instance;
    private Transform _uiRoot; // 挂载的根节点
    private Dictionary<string, string> pathDict; // 配置路径表
    private Dictionary<string, GameObject> prefabDict; // 预制件缓存字典
    public Dictionary<string, BasePanel> panelDict; // 界面缓存字典

    public static UIManager Instance
    {
        get 
        {
            if (_Instance == null)
            {
                _Instance = new UIManager();
            }
            return _Instance;
        }
    }

    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                if (GameObject.Find("Canvas"))
                {
                    _uiRoot = GameObject.Find("Canvas").transform; // 此处直接以Canvas为根节点，复杂的项目视情况而定
                }
                else
                {
                    _uiRoot = new GameObject("Canvas").transform;
                }
                return _uiRoot;
            }
            return _uiRoot;
        }
    }

    private UIManager()
    {
        InitDicts();
    }

    private void InitDicts()
    {
        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();

        // 配置界面和路径
        pathDict = new Dictionary<string, string>()
        {
            {UIConst.MainMenu, "MainMenu"},
            {UIConst.RegistrationMenu, "RegistrationMenu"},
            {UIConst.Archives, "Archives"},
            {UIConst.Settings, "Settings"},
            {UIConst.PlayerInfo, "Player Info"},
            {UIConst.PlayerBag, "Bag"},
            {UIConst.PlayerMission, "Missions"},
            {UIConst.LoadScene, "LoadScene"},
            {UIConst.Shop, "ShopMenu"},
            {UIConst.TipsBox, "TipsBox"},
            {UIConst.RestartMenu, "RestartMenu"},
            {UIConst.LoadSceneWithDark, "LoadSceneWithDark"},
            {UIConst.BossInfo, "Boss Info"}
        };
    }

    public BasePanel OpenPanel(string name)
    {
        BasePanel panel = null;
        if (panelDict.TryGetValue(name, out panel)) // out关键字与ref类似，不同点在与out修饰的变量不需要初始化，相当于按name键找到值，若存在则直接赋值给panel
        {
            Debug.LogError("界面已打开：" + name);
            return null;
        }

        // 检测是否在配置表中
        string path = "";
        if (!pathDict.TryGetValue(name, out path))
        {
            Debug.LogError("界面名称错误，或未在配置表中：" + name);
            return null;
        }

        // 加载界面预制件
        GameObject panelPrefab = null;
        if (!prefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = "Prefabs/Panel/" + path;
            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name, panelPrefab);
        }

        // 打开界面
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        panel = panelObject.GetComponent<BasePanel>();
        panelDict.Add(name, panel);
        return panel;
    }

    public bool ClosePanel(string name)
    {
        BasePanel panel = null;
        if (!panelDict.TryGetValue(name, out panel))
        {
            Debug.LogError("界面未打开：" + name);
            return false;
        }

        panel.ClosePanel();
        return true;
    }
}

public class UIConst // 存储界面名常量
{
    public const string MainMenu = "MainMenu"; // 主菜单
    public const string RegistrationMenu = "RegistrationMenu"; // 新游戏菜单
    public const string Archives = "Archives"; // 存档菜单
    public const string PlayerInfo = "PlayerInfo"; // 玩家信息菜单
    public const string PlayerBag = "PlayerBag"; // 背包
    public const string PlayerMission = "PlayerMission"; // 任务
    public const string Settings = "Settings"; // 设置
    public const string LoadScene = "LoadScene"; // 场景过渡(需点击进入)
    public const string LoadSceneWithDark = "LoadSceneWithDark"; // 场景过渡(渐变直接进入)
    public const string Shop = "Shop"; // 商店
    public const string RestartMenu = "RestartMenu"; // 死亡菜单
    public const string TipsBox = "TipsBox"; // 弹窗
    public const string BossInfo = "BossInfo"; // BOSS信息
}
