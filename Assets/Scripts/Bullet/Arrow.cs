using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D col;
    public float damage = 10f;
    [SerializeField] private float speed = 13f;
    [SerializeField] private int attackPause = 3;
    private IEnumerator ie;
    private Coroutine cor;
    private Vector3 offset;
    public Transform enemyTransform;
    private Color setColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        enemyTransform = null;
        setColor = GetComponent<SpriteRenderer>().color;
        ie = PushArrow();
    }

    public void SetSpeed(Vector2 dir)
    {
        rb.velocity = new Vector2(dir.x * speed, 0);
    }

    private void Update()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        if (enemyTransform != null)
        { // 攻击到敌人时，箭矢粘附在敌人身上
            transform.position = enemyTransform.position + offset;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            cor = StartCoroutine(ie);
            rb.simulated = false;
            enemyTransform = collision.transform;
            offset = transform.position - enemyTransform.position;
        }

        if (collision.CompareTag("Ground"))
        {
            rb.simulated = false;
            cor = StartCoroutine(ie);
        }
        
        MushroomFSMAI mushroom = collision.gameObject.GetComponent<MushroomFSMAI>();
        GoblinFSMAI goblin = collision.gameObject.GetComponent<GoblinFSMAI>();
        SamuraiFSMAI samurai = collision.gameObject.GetComponent<SamuraiFSMAI>();
        if (mushroom != null)
        {
            rb.simulated = false;
            mushroom.getHurt(damage);
            GameManager.Instance.HitPause(attackPause);
            GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
        if (goblin != null)
        {
            rb.simulated = false;
            goblin.getHurt(damage);
            GameManager.Instance.HitPause(attackPause);
            GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
        if (samurai != null)
        {
            rb.simulated = false;
            samurai.getHurt(damage);
            GameManager.Instance.HitPause(attackPause);
            GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
    }

    IEnumerator PushArrow()
    {
        while (setColor.a >= 0.2f)
        {
            GetComponent<SpriteRenderer>().color = setColor;
            setColor.a = Mathf.Lerp(setColor.a, 0, 0.05f);
            yield return null;
        }
        ObjectPool.Instance.Push(gameObject);
    }
}
