using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public CircleCollider2D circleCollider;
    public float damage = 15;
    [SerializeField] private float waitTime = 5f;
    private IEnumerator ie;
    private Coroutine cor;
    private AnimatorStateInfo info;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetTarget(Vector2 speed)
    {
        rb.velocity = speed;
        ie = WaitForExplosion(waitTime);
        cor = StartCoroutine(ie);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerControll player = collision.GetComponent<PlayerControll>();
            player.getHurt(damage, transform.position);
        }
    }

    IEnumerator WaitForExplosion(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.SetTrigger("canBoom");
        yield return new WaitForSeconds(1.5f);
        ObjectPool.Instance.Push(gameObject);
    }
}
