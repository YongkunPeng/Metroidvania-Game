using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("记录拖得的物体和其所在的原物品槽")]
    [SerializeField] public static GameObject itemBeginDragged;
    [SerializeField] public static Transform startParent;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private LayoutElement element;
    [SerializeField] private Transform outSlot; // 记录所有槽的父对象

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
        startParent.GetComponent<GridLayoutGroup>().enabled = false; // 取消网格布局防止拖动时闪烁
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
        startParent.GetComponent<GridLayoutGroup>().enabled = true; // 恢复网格布局是物品处于正中心
        itemBeginDragged = null;
        if (transform.parent == startParent || transform.parent == outSlot)
        {
            Debug.Log("不在");
            transform.SetParent(startParent);
            transform.position = startPosition;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
