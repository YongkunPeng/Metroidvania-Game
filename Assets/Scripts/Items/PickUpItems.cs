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
        { // ����������ʱͣ��
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        if (collision.CompareTag("Player"))
        {
            if (transform.CompareTag("Item"))
            { // ��Ʒ
                bool canPicked = GameManager.Instance.AddItem(item, 1);
                if (canPicked)
                { // �ɱ�ʰȡʱ��������Э��
                    TipsBoxManager.Instance.ShowTipsBox("ʰȡ��Ʒ��" + item.itemName, 1f);
                    StartCoroutine(DestroyItem());
                }
            }
            else if (transform.CompareTag("Coin"))
            { // ���
                GameObject.FindObjectOfType<PlayerControll>().ChangeCoinCnt(1);
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DestroyItem()
    { // ��Ʒ�Ӵ�����Һ�ʰȡ��������������ʧ
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
