using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : BasePanel
{
    [SerializeField] private GameObject playerGrid; // �����Ʒ�б�
    [SerializeField] private GameObject sellItemPrefab; // �б���Ԥ����
    [SerializeField] private GameObject shopPanel; // �Ҳ��̵����
    [SerializeField] private bool shouldUpdate = true; // ���±�����Ϣ��ʶ

    private void Awake()
    {
        OpenPanel(UIConst.Shop);
        playerGrid = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        shopPanel = transform.GetChild(1).gameObject;
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        GameManager.Instance.isPaused = true;
    }

    private void Update()
    {
        if (shouldUpdate)
        {
            UpdatePlayerBagData();
            UpdateShopData();
            shouldUpdate = false;
        }
    }

    /// <summary>
    /// ���������ұ�������
    /// </summary>
    private void UpdatePlayerBagData()
    {
        // �����ǰ����������
        for (int i = 0; i < playerGrid.transform.childCount; i++)
        {
            playerGrid.transform.GetChild(i).gameObject.SetActive(false);
            Destroy(playerGrid.transform.GetChild(i).gameObject);
        }

        playerGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 170 * GameManager.Instance.itemsDict.Count);

        // ������Ԥ�������
        foreach (var pair in GameManager.Instance.itemsDict)
        {
            GameObject sellItem = GameObject.Instantiate(sellItemPrefab, playerGrid.transform);
            sellItem.GetComponent<PlayerSellItemsControll>().UpdateItemData(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// �����Ҳ��̵�����
    /// </summary>
    private void UpdateShopData()
    {
        int coinCnt = 0;
        GameObject.FindObjectOfType<PlayerControll>().GetCoinData(ref coinCnt);
        shopPanel.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = coinCnt.ToString();
    }

    /// <summary>
    /// �ر��̵�UI
    /// </summary>
    public void CloseShopMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.isPaused = false;
        Destroy(gameObject);
    }

    /// <summary>
    /// �̵�������
    /// </summary>
    public void ChangeShouldUpdate()
    {
        shouldUpdate = true;
    }

    private void OnDestroy()
    {
        ClosePanel();
    }
}
