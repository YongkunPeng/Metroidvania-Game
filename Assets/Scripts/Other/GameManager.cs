using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public bool isPaused; // ��¼��Ϸ�Ƿ���ͣ
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
        if (itemsDict.ContainsKey(item) && itemsDict[item] < 99)
        { // �������Ѿ����ڸ���Ʒ��������С��99��������+1
            itemsDict[item] += 1;
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // ����UI��ʱ�����±�����ʾ��Ϣ
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            return true;
        }
        else
        {
            if (itemsDict.Count <= 10 && !itemsDict.ContainsKey(item))
            { // �����ڸ������ҿռ�����
                itemsDict.Add(item, 1);
                if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
                { // ����UI��ʱ�����±�����ʾ��Ϣ
                    GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
                }
                return true;
            }
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
        if (itemsDict.ContainsKey(item))
        { // ���ڸ���Ʒ
            if (itemsDict[item] > 1)
            { // ��������1
                itemsDict[item] -= 1;
            }
            else
            { // ��������1
                itemsDict.Remove(item);
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
                if (i < itemsDict.Count)
                { // ��������Ʒ
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
                { // ��������Ʒ
                    slots[i].slotItem = null;
                }
            }
            #endregion
        }
    }
}
