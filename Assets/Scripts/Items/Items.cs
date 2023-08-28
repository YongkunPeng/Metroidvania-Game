using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>ScriptableObject��Ϊ���������࣬�̳���Ľű����ɲ��������κζ����ϣ���ʵ�����ݹ�������洢ͬһ���ݵĽű���δ����������ڴ�</para>
/// <para>�̳�ScriptableObject��Ľű��д洢�������ڱ༭��ģʽ�־ã�����󲻳־ã�һ��洢��Ϸ�����������в���ı�ı���</para>
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/ʰȡ��", order = 0)]
public class Items : ScriptableObject
{
    public string itemName; // ��Ʒ��
    public string itemDes; // ��Ʒ����
    public Sprite itemSprite; // ��ƷͼƬ
    public int buyPrice; // ����۸�
    public int sellPrice; // �����۸�
}

public class ItemsConst // ��Ʒ��
{
    public const string Arrow = "��ʸ";
    public const string MushroomChunks = "Ģ�����";
}