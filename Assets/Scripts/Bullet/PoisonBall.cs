using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PoisonBall : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public CircleCollider2D circleCollider;
    public float damage = 8;
    [SerializeField] private int attackPause = 3;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float destroyTime = 0.3f; // ��ײ��Ŀ�������ʱ��
    [SerializeField] private float destroyTimeNoTarget = 3f; // δ��ײ��Ŀ�������ʱ��
    private IEnumerator ie;
    private Coroutine cor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetSpeed(Vector2 dir)
    {
        rb.velocity = dir * speed;
        ie = DestroyBulletNoTarget(destroyTimeNoTarget, destroyTime);
        cor = StartCoroutine(ie);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ie = DestroyBullet(destroyTime);
            cor = StartCoroutine(ie);
            PlayerControll player = collision.GetComponent<PlayerControll>();
            player.getHurt(damage, transform.position);
            transform.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            GameManager.Instance.HitPause(attackPause);
        }
        else if (collision.CompareTag("Ground"))
        {
            ie = DestroyBullet(destroyTime);
            cor = StartCoroutine(ie);
        }
    }

    IEnumerator DestroyBullet(float desTime)
    { // ��ײ����������ʱ����ֹ����һ��ʱ�������
        animator.SetBool("isDestroy", true);
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        circleCollider.enabled = false;
        yield return new WaitForSeconds(desTime);
        ObjectPool.Instance.Push(gameObject);
    }

    IEnumerator DestroyBulletNoTarget(float waitTime, float desTime)
    { // δ���У�����һ��ʱ����Ȼ����
        yield return new WaitForSeconds(waitTime);
        animator.SetBool("isDestroy", true);
        circleCollider.enabled = false;
        yield return new WaitForSeconds(desTime);
        ObjectPool.Instance.Push(gameObject);
    }
}
