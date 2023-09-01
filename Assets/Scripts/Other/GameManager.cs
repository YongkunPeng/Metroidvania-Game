using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public bool isPaused; // ��¼��Ϸ�Ƿ���ͣ
    public Dictionary<int, string> slotDict; // ��Ʒ��������Ʒ
    public Dictionary<string, int> itemsDict; // ��ұ�������
    public Dictionary<string, Items> resourceDict = new Dictionary<string, Items>();
    private Items[] itemsResource;
    public List<Mission> missionList = new List<Mission>(); // �����������
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
        // ��������ScriptsObject��Ʒ����
        itemsResource = Resources.LoadAll<Items>("ItemData");
        foreach (Items item in itemsResource)
        {
            resourceDict.Add(item.itemName, item);
        }
    }

    private void Awake()
    {
        // ȷ��ֻ��һ��ʵ������
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ������ʼ���߼�
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        #region �����ռ�����״̬
        foreach ( Mission mission in missionList )
        {
            if (mission.missionStatus == Mission.MissionStatus.Accepted && mission.missionType == Mission.MissionType.Collect && itemsDict.ContainsKey(mission.collectItemName))
            { // ��Ʒ�����ֵ����и����壬������δ���
                if (itemsDict[mission.collectItemName] >= mission.goalCnt)
                {
                    mission.UpdateMissionComplete();
                }
            }
            else if (mission.missionStatus == Mission.MissionStatus.Completed && mission.missionType == Mission.MissionType.Collect)
            { // �����ռ����񣬽�����ɵ�δ�콱���ڼ������б仯����δ���ʱ
                if (!itemsDict.ContainsKey(mission.collectItemName) || itemsDict[mission.collectItemName] < mission.goalCnt)
                {
                    mission.UpdateMissionAccept();
                }
            }
        }
        #endregion
    }

    public void HitPause(int duration)
    {
        StartCoroutine(Pause(duration));
    }

    /// <summary>
    /// �򱳰������������Ʒ�����ظ������Ƿ�ɱ�ʰȡ
    /// </summary>
    /// <param name="item">����ӵ���Ʒ��Ϣ</param>
    public bool AddItem(Items item)
    {
        if (itemsDict.ContainsKey(item.itemName) && itemsDict[item.itemName] < 99)
        { // �������Ѿ����ڸ���Ʒ��������С��99��������+1
            itemsDict[item.itemName] += 1;
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // ����UI��ʱ�����±�����ʾ��Ϣ
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            return true;
        }
        else if (!itemsDict.ContainsKey(item.itemName) && slotDict.Count < 10 && itemsDict.Count < 10)
        { // �����в�������Ʒ���һ��пռ�
            for (int i = 0; i < 10; i++)
            {
                bool canUseSlot = true; // ��ʾ�ò��Ƿ�Ϊ��
                foreach(KeyValuePair<int, string> pair in slotDict)
                {
                    if (i == pair.Key)
                    { // �ò��ѱ�ռ�ã�������ѭ����������һ����ѭ��
                        canUseSlot = false;
                        break;
                    }
                }

                if (canUseSlot)
                { // ��ǰi��ָ��Ʒ��δ��ռ�ã�����ѭ��
                    slotDict.Add(i, item.itemName);
                    itemsDict.Add(item.itemName, 1);
                    return true;
                }
            }
            Debug.LogWarning("�޷�ʰȡ��Ʒ�����޿�λ��");
            return false;
        }
        Debug.LogWarning("�޷�ʰȡ��Ʒ������Ʒ�ѳ������ޣ�");
        return false;
    }

    /// <summary>
    /// �����������е�ָ����Ʒ������ʹ֮����-1
    /// </summary>
    /// <param name="item">����ӵ���Ʒ��Ϣ</param>
    public void RemoveItem(Items item)
    {
        if (itemsDict.ContainsKey(item.itemName))
        { // ���ڸ���Ʒ
            if (itemsDict[item.itemName] > 1)
            { // ��������1
                itemsDict[item.itemName] -= 1;
            }
            else
            { // ��������1
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
            { // ����UI��ʱ�����±�����ʾ��Ϣ
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            ResetItem();
        }
        else
        { // �����ڸ�����
            Debug.LogError("�޷����������ڵ�����");
        }
    }

    /// <summary>
    /// ������Ʒ�۽ű��е�slotItem
    /// </summary>
    public void ResetItem()
    {
        if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
        {
            #region ��ȡ����UI�еĸ�����Ʒ�۽ű�
            GameObject bagUI = GameObject.FindGameObjectWithTag("Bag");
            slots = new SlotController[bagUI.transform.GetChild(0).GetChild(1).childCount];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = bagUI.transform.GetChild(0).GetChild(1).GetChild(i).GetComponent<SlotController>();
            }
            #endregion

            #region ���¸�����Ʒ�۵�slotItem��ֵ
            for (int i = 0; i < slots.Length; i++)
            {
                if (slotDict.ContainsKey(i))
                {
                    slots[i].slotItem = resourceDict[slotDict[i]];
                }
                else
                { // ��������Ʒ
                    slots[i].slotItem = null;
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// ������ݱ��ػ��洢
    /// </summary>
    /// <param name="life">�������ֵ</param>
    /// <param name="coinCnt">��ҽ����</param>
    public void SaveUserData(float life, int coinCnt)
    {
        userData.health = life;
        userData.coinCnt = coinCnt;
        LocalConfig.SaveUserData(userData);
        Debug.Log("������ɣ�");
    }

    /// <summary>
    /// ����������ݲ���ת����Ϸ����
    /// </summary>
    /// <param name="userData">�������UserData</param>
    public void InitUserData(UserData userData)
    {
        this.userData = userData;
        username = userData.username;
        slotDict = userData.slotDict;
        itemsDict = userData.itemsDict;
        missionList = userData.missionList;
        SceneLoadManager.Instance.LoadLevelByIndex(1);
    }

    /// <summary>
    /// ������֡
    /// </summary>
    /// <param name="duration">ͣ��ʱ��</param>
    /// <returns></returns>
    IEnumerator Pause(int duration)
    {
        float pauseTime = duration / 60f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }
}
