using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSellItemsControll : MonoBehaviour
{
    [SerializeField] private Image itemImg; // 物品图片
    [SerializeField] private Text itemName; // 物品名
    [SerializeField] private Text itemCnt; // 物品数
    [SerializeField] private Text itemSell; // 卖价
    [SerializeField] private Items item; //物品
    [SerializeField] private Button sellBtn; // 售卖按钮

    private void Awake()
    {
        itemImg = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        itemName = transform.GetChild(1).GetComponent<Text>();
        itemCnt = transform.GetChild(2).GetComponent<Text>();
        itemSell = transform.GetChild(3).GetComponent<Text>();
        sellBtn = transform.GetChild(4).GetComponent<Button>();
    }

    private void OnEnable()
    {
        sellBtn.onClick.AddListener(SellItem);
    }

    /// <summary>
    /// 更新该可售卖物品相关信息
    /// </summary>
    /// <param name="itemName">物品名</param>
    /// <param name="itemCnt">物品数量</param>
    public void UpdateItemData(string itemName, int itemCnt)
    {
        this.item = GameManager.Instance.resourceDict[itemName];
        this.itemImg.sprite = item.itemSprite;
        this.itemName.text = itemName;
        this.itemCnt.text = itemCnt.ToString();
        this.itemSell.text = "售价：" + item.sellPrice;
    }

    /// <summary>
    /// 售卖监听
    /// </summary>
    private void SellItem()
    {
        bool isSell = GameObject.FindObjectOfType<PlayerControll>().ChangeCoinCnt(this.item.sellPrice);
        if (isSell)
        {
            Debug.Log("卖出：" + this.item.sellPrice);
            TipsBoxManager.Instance.ShowTipsBox("卖出物品：" + this.item.itemName, 1f);
            AudioSourceManager.Instance.PlaySound(GlobalAudioClips.ShopSound);
            GameManager.Instance.RemoveItem(this.item);
            transform.parent.parent.parent.parent.GetComponent<ShopMenu>().ChangeShouldUpdate();
        }
    }
}
