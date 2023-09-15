using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager _Instance;
    private Transform _uiRoot; // ���صĸ��ڵ�
    private Dictionary<string, string> pathDict; // ����·����
    private Dictionary<string, GameObject> prefabDict; // Ԥ�Ƽ������ֵ�
    public Dictionary<string, BasePanel> panelDict; // ���滺���ֵ�

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
                    _uiRoot = GameObject.Find("Canvas").transform; // �˴�ֱ����CanvasΪ���ڵ㣬���ӵ���Ŀ���������
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

        // ���ý����·��
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
        if (panelDict.TryGetValue(name, out panel)) // out�ؼ�����ref���ƣ���ͬ������out���εı�������Ҫ��ʼ�����൱�ڰ�name���ҵ�ֵ����������ֱ�Ӹ�ֵ��panel
        {
            Debug.LogError("�����Ѵ򿪣�" + name);
            return null;
        }

        // ����Ƿ������ñ���
        string path = "";
        if (!pathDict.TryGetValue(name, out path))
        {
            Debug.LogError("�������ƴ��󣬻�δ�����ñ��У�" + name);
            return null;
        }

        // ���ؽ���Ԥ�Ƽ�
        GameObject panelPrefab = null;
        if (!prefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = "Prefabs/Panel/" + path;
            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name, panelPrefab);
        }

        // �򿪽���
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
            Debug.LogError("����δ�򿪣�" + name);
            return false;
        }

        panel.ClosePanel();
        return true;
    }
}

public class UIConst // �洢����������
{
    public const string MainMenu = "MainMenu"; // ���˵�
    public const string RegistrationMenu = "RegistrationMenu"; // ����Ϸ�˵�
    public const string Archives = "Archives"; // �浵�˵�
    public const string PlayerInfo = "PlayerInfo"; // �����Ϣ�˵�
    public const string PlayerBag = "PlayerBag"; // ����
    public const string PlayerMission = "PlayerMission"; // ����
    public const string Settings = "Settings"; // ����
    public const string LoadScene = "LoadScene"; // ��������(��������)
    public const string LoadSceneWithDark = "LoadSceneWithDark"; // ��������(����ֱ�ӽ���)
    public const string Shop = "Shop"; // �̵�
    public const string RestartMenu = "RestartMenu"; // �����˵�
    public const string TipsBox = "TipsBox"; // ����
    public const string BossInfo = "BossInfo"; // BOSS��Ϣ
}
