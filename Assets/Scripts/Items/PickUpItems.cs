using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItems : MonoBehaviour
{
    private SpriteRenderer itemRender;
    private Color newColor;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    [SerializeField] private Items item;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        itemRender = GetComponent<SpriteRenderer>();
        newColor = itemRender.color;

        rb.AddForce(new Vector2(Random.Range(-2f, 2f), 5f), ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        { // 触碰到地面时停下
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        if (collision.CompareTag("Player"))
        {
            if (transform.CompareTag("Item"))
            { // 物品
                bool canPicked = GameManager.Instance.AddItem(item, 1);
                if (canPicked)
                { // 可被拾取时启动销毁协程
                    TipsBoxManager.Instance.ShowTipsBox("拾取物品：" + item.itemName, 1f);
                    StartCoroutine(DestroyItem());
                }
            }
            else if (transform.CompareTag("Coin"))
            { // 金币
                GameObject.FindObjectOfType<PlayerControll>().ChangeCoinCnt(1);
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DestroyItem()
    { // 物品接触到玩家后被拾取，缓慢上升并消失
        col.enabled = false;
        while (newColor.a >= 0.15f)
        {
            rb.velocity = new Vector2(0, 0.5f);
            newColor.a = Mathf.Lerp(newColor.a, 0f, 0.01f);
            itemRender.color = newColor;
            yield return null;
        }
        Destroy(gameObject);
    }
}
