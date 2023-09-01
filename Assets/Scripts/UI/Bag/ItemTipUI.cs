using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTipUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text soldPriceText;

    private void Awake()
    {
        nameText = transform.GetChild(0).GetComponent<Text>();
        descriptionText = transform.GetChild(1).GetComponent<Text>();
        soldPriceText = transform.GetChild(2).GetComponent<Text>();
    }

    /// <summary>
    /// 显示物品详细信息UI
    /// </summary>
    public void ShowItemTip()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏物品详细信息UI，同时清空信息
    /// </summary>
    public void HideItemTip()
    {
        gameObject.SetActive(false);
        if (nameText != null || descriptionText != null || soldPriceText != null)
        {
            nameText.text = null;
            descriptionText.text = null;
            soldPriceText.text = null;
        }
    }

    /// <summary>
    /// 更新物品详细信息UI中的内容
    /// </summary>
    /// <param name="name">物品名</param>
    /// <param name="desc">物品描述</param>
    /// <param name="coin">玩家向商店卖出价格</param>
    public void UpdateItemTip(string name, string desc, int coin)
    {
        nameText.text = name;
        descriptionText.text = desc;
        soldPriceText.text = coin.ToString();
    }

    /// <summary>
    /// 修改物品详细信息UI的位置到光标处
    /// </summary>
    /// <param name="pos">显示的位置</param>
    public void SetTipPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }
}
