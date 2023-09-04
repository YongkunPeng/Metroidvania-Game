using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : BasePanel
{
    [SerializeField] private GameObject playerGrid; // 玩家物品列表
    [SerializeField] private GameObject sellItemPrefab; // 列表子预制体
    [SerializeField] private GameObject shopPanel; // 右侧商店界面
    [SerializeField] private bool shouldUpdate = true; // 更新背包信息标识

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
    /// 更新左侧玩家背包数据
    /// </summary>
    private void UpdatePlayerBagData()
    {
        // 清空先前所有子物体
        for (int i = 0; i < playerGrid.transform.childCount; i++)
        {
            playerGrid.transform.GetChild(i).gameObject.SetActive(false);
            Destroy(playerGrid.transform.GetChild(i).gameObject);
        }

        playerGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 170 * GameManager.Instance.itemsDict.Count);

        // 生成新预制体填充
        foreach (var pair in GameManager.Instance.itemsDict)
        {
            GameObject sellItem = GameObject.Instantiate(sellItemPrefab, playerGrid.transform);
            sellItem.GetComponent<PlayerSellItemsControll>().UpdateItemData(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// 更新右侧商店数据
    /// </summary>
    private void UpdateShopData()
    {
        int coinCnt = 0;
        GameObject.FindObjectOfType<PlayerControll>().GetCoinData(ref coinCnt);
        shopPanel.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = coinCnt.ToString();
    }

    /// <summary>
    /// 关闭商店UI
    /// </summary>
    public void CloseShopMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.isPaused = false;
        Destroy(gameObject);
    }

    /// <summary>
    /// 商店界面更新
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
