using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;

public class SlashFX : MonoBehaviour
{
    [SerializeField] private float speed = 2500f; // 弹幕速度
    [SerializeField] private int attackPause = 3; // 停顿时间
    [SerializeField] private float damage = 10f; // 伤害
    private BoxCollider2D col;
    private Rigidbody2D rb;
    private CinemachineImpulseSource impulseSource;
    private Light2D lightColor;
    private SpriteRenderer sprite;
    private float startTime;
    private float endTime;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        sprite = GetComponent<SpriteRenderer>();
        lightColor = GetComponent<Light2D>();
    }

    private void OnEnable()
    {
        startTime = Time.time;
        endTime = Time.time;
        rb.gravityScale = 0;
        sprite.color = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1);
        lightColor.color = new Color(1, 80f / 255f, 80f / 255f, 1);
    }

    private void Update()
    {
        endTime += Time.deltaTime;
        if (endTime - startTime >= 1f)
        {
            StartCoroutine(HideSlashFX());
        }
    }

    /// <summary>
    /// 根据方向设置气刃速度和方向
    /// </summary>
    /// <param name="dir">方向</param>
    public void SetDirection(Vector2 dir)
    {
        rb.velocity = new Vector2(dir.x * speed * Time.deltaTime, 0);
        if (dir.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerControll>().getHurt(damage, transform.position);
            impulseSource.GenerateImpulse();
            GameManager.Instance.HitPause(attackPause);
            StartCoroutine(HideSlashFX());
        }
        if (collision.CompareTag("Ground"))
        {
            StartCoroutine(HideSlashFX());
        }
    }

    /// <summary>
    /// 对象池回收
    /// </summary>
    /// <returns></returns>
    IEnumerator HideSlashFX()
    {
        col.enabled = false;
        while (sprite.color.a >= 0.2f)
        {
            sprite.color -= new Color(0, 0, 0, 0.001f);
            lightColor.color -= new Color(0, 0, 0, 0.001f);
            yield return null;
        }
        ObjectPool.Instance.Push(gameObject);
    }
}
