using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer mySprite;
    private SpriteRenderer playerSprite;

    private Color color;

    [Header("���䲽��")]
    public float step; // ��ֵ����

    private void OnEnable()
    {
        // ��ȡ�������ҵ�������
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mySprite = GetComponent<SpriteRenderer>();
        playerSprite = playerTransform.GetComponent<SpriteRenderer>();

        // ������һ�ȡ�����Ϣ
        transform.position = playerTransform.position;
        transform.localScale = playerTransform.localScale;
        transform.rotation = playerTransform.rotation;

        color = playerSprite.color;
        mySprite.color = playerSprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        color.a = Mathf.Lerp(color.a, 0f, step);
        mySprite.color = color;
        if (mySprite.color.a <= 0.001f)
        {
            ObjectPool.Instance.Push(gameObject);
        }
    }
}
