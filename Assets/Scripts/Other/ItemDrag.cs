using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("��¼�ϵõ�����������ڵ�ԭ��Ʒ��")]
    [SerializeField] public static GameObject itemBeginDragged;
    [SerializeField] public static Transform startParent;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private LayoutElement element;
    [SerializeField] private Transform outSlot; // ��¼���в۵ĸ�����

    private void Awake()
    {
        element = GetComponent<LayoutElement>();
        outSlot = transform.parent.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startParent = transform.parent;
        startPosition = transform.position;
        itemBeginDragged = gameObject;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        startParent.GetComponent<GridLayoutGroup>().enabled = false; // ȡ�����񲼾ַ�ֹ�϶�ʱ��˸
        element.ignoreLayout = true;
        transform.SetParent(outSlot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + new Vector3(0, 0, 1);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        element.ignoreLayout = false;
        startParent.GetComponent<GridLayoutGroup>().enabled = true; // �ָ����񲼾�����Ʒ����������
        itemBeginDragged = null;
        if (transform.parent == startParent || transform.parent == outSlot)
        {
            Debug.Log("����");
            transform.SetParent(startParent);
            transform.position = startPosition;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
