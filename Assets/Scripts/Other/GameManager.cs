using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public bool isPaused; // ��¼��Ϸ�Ƿ���ͣ
    public Dictionary<int, string> slotDict = new Dictionary<int, string>(); // ��Ʒ��������Ʒ
    public Dictionary<string, int> itemsDict = new Dictionary<string, int>(); // ��ұ�������
    public Dictionary<string, Items> resourceDict = new Dictionary<string, Items>();
    private Items[] itemsResource;
    public List<Mission> missionList = new List<Mission>(); // �����������
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
        // userData = LocalConfig.LoadUserData(username); // �����û�����

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
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
}
