using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>ScriptableObject类为数据容器类，继承其的脚本即可不挂载在任何对象上，且实现数据共享，避免存储同一数据的脚本多次创建，消耗内存</para>
/// <para>继承ScriptableObject类的脚本中存储的数据在编辑器模式持久，打包后不持久，一般存储游戏发布后运行中不会改变的变量</para>
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/拾取物", order = 0)]
public class Items : ScriptableObject
{
    public string itemName; // 物品名
    public string itemDes; // 物品描述
    public Sprite itemSprite; // 物品图片
    public int buyPrice; // 购买价格
    public int sellPrice; // 贩卖价格
    public bool canUse; // 是否可被使用
}

public class ItemsConst // 物品名
{
    public const string Arrow = "箭矢";
    public const string MushroomChunks = "蘑菇碎块";
    public const string LifePotion = "生命药剂";
}