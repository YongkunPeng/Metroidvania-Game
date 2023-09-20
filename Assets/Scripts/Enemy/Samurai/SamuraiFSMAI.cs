using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SamuraiBlackboard : Blackboard
{
    // ��ȡ�������
    public Rigidbody2D rb;
    // ��ȡ���󶯻�������
    public Animator samuraiAnimator;
    // ʹ״̬���ܵ��ö����transform
    public Transform samuraiTransform;
    // ���Ŀ������transform
    public Transform targetTransform;
    // ��ö����col
    public BoxCollider2D col;

    [Header("��������")]
    public float maxHealth;
    public float toughness;

    [Header("�ٶ�")]
    public float fallMultiplier;

    [Header("��������ⷶΧ")]
    public int chooseAttackSkill;
    public LayerMask targetLayer;
    public LayerMask airWall;
    public float attack1Length;
    public float attack2Length;
    public float attack3Length;

    [Header("״̬�ж�")]
    public bool isHit;
    public bool isGround;

    [Header("����λ��")]
    public float hurtOffset;

    [Header("����Ԥ����")]
    public GameObject bulletPre; // �䵯
    public GameObject blood; // ѪҺ����
    public GameObject dropItemPre; // ������
    public GameObject coin; // ���

    [Header("���������")]
    public float attack;
    public int attackPause;

    [Header("��ҵ�������Χ")]
    public int min;
    public int max;
}

public class SamuraiFSMAI : MonoBehaviour
{
    private FSM samuraiFSM;
    public SamuraiBlackboard samuraiBlackboard = new SamuraiBlackboard();
    [SerializeField] private bool healToughness; // �ָ�����Э���Ƿ��ѱ�����
    [SerializeField] private bool isStatusChange; // �Ƿ��ѽ�����׶�

    [Header("��Ч��Ϸ����")]
    [SerializeField] private GameObject shinningEffect; // ������Ч
    [SerializeField] private GameObject redEyeEffect; // ������Ч
    [SerializeField] private GameObject bloodBurst; // ������Ч

    [Header("����")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material originMaterial;
    [SerializeField] private Material HitMaterial;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        samuraiBlackboard.rb = GetComponent<Rigidbody2D>();
        samuraiBlackboard.samuraiAnimator = GetComponent<Animator>();
        samuraiBlackboard.col = GetComponent<BoxCollider2D>();
        samuraiBlackboard.samuraiTransform = transform;
        samuraiBlackboard.targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    private void OnEnable()
    {
        samuraiBlackboard.maxHealth = samuraiBlackboard.health; // ��¼��������
    }

    private void Start()
    {
        samuraiFSM = new FSM(samuraiBlackboard);

        samuraiFSM.AddState(StateType.Idle, new SamuraiIdleState(samuraiFSM));
        samuraiFSM.AddState(StateType.Chase, new SamuraiChaseState(samuraiFSM));
        samuraiFSM.AddState(StateType.Attack1, new SamuraiAttack1State(samuraiFSM));
        samuraiFSM.AddState(StateType.Attack2, new SamuraiAttack2State(samuraiFSM));
        samuraiFSM.AddState(StateType.Attack3, new SamuraiAttack3State(samuraiFSM));
        samuraiFSM.AddState(StateType.Attack4, new SamuraiAttack4State(samuraiFSM));
        samuraiFSM.AddState(StateType.Evade, new SamuraiEvadeState(samuraiFSM));
        samuraiFSM.AddState(StateType.Jump, new SamuraiJumpState(samuraiFSM));
        samuraiFSM.AddState(StateType.Fall, new SamuraiFallState(samuraiFSM));
        samuraiFSM.AddState(StateType.Hurt, new SamuraiHurtState(samuraiFSM));
        samuraiFSM.AddState(StateType.Dead, new SamuraiDeadState(samuraiFSM));

        samuraiFSM.SwitchState(StateType.Idle);
    }

    private void FixedUpdate()
    {
        samuraiFSM.OnFixUpdate();
    }

    private void Update()
    {
        samuraiFSM.OnCheck();
        samuraiFSM.OnUpdate();
        RedEyeEffectActive();
    }
    
    // ���׶���Ч
    private void RedEyeEffectActive()
    {
        if (!isStatusChange)
        {
            if (samuraiBlackboard.health <= samuraiBlackboard.maxHealth / 2 && !redEyeEffect.activeSelf)
            {
                redEyeEffect.SetActive(true);
                bloodBurst.SetActive(true);
                samuraiBlackboard.speed = 6;
                isStatusChange = true;
            }
        }
    }

    // ��Ѫ����
    public void getHurt(float damage)
    {
        StartCoroutine(RenderChange());
        HealSamuraiToughness();
        GameObject blood = ObjectPool.Instance.Get(samuraiBlackboard.blood);
        blood.transform.position = samuraiBlackboard.samuraiTransform.position;
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.EnemyHurt);
        samuraiBlackboard.isHit = true;
        samuraiBlackboard.health -= damage;
        samuraiBlackboard.toughness -= damage;
    }

    // �ָ�����
    private void HealSamuraiToughness()
    {
        if (samuraiBlackboard.toughness <= 0 && healToughness == false)
        { // ����С�ڵ���0��δ�����ָ�Э��
            healToughness = true;
            StartCoroutine(HealToughness());
        }
    }

    // �ж��Ƿ��ڵ��棬����������ͨ��ִ��
    private void UpdateGroundStatus(bool isGround)
    {
        samuraiBlackboard.isGround = isGround;
    }

    // �ε���Ч����
    public void Shinning()
    {
        shinningEffect.SetActive(true);
    }

    // ����λ��
    public void AttackOffset(float offset)
    {
        samuraiBlackboard.rb.velocity = new Vector2(transform.localScale.x * offset, samuraiBlackboard.rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerControll player = collision.GetComponent<PlayerControll>();
            player.getHurt(samuraiBlackboard.attack, transform.position);
            GameManager.Instance.HitPause(samuraiBlackboard.attackPause);
            transform.GetChild(0).GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.right * transform.localScale.x * samuraiBlackboard.attack1Length);
        Gizmos.DrawRay(transform.position - new Vector3(0, 0.1f, 0), Vector3.right * transform.localScale.x * samuraiBlackboard.attack2Length);
        Gizmos.DrawRay(transform.position - new Vector3(0, 0.2f, 0), Vector3.right * transform.localScale.x * samuraiBlackboard.attack3Length);
        Gizmos.color = Color.blue;
    }

    // �ָ�����
    IEnumerator HealToughness()
    {
        yield return new WaitForSeconds(2f);
        samuraiBlackboard.toughness = 50;
        healToughness = false;
    }

    // �ܻ������л�
    IEnumerator RenderChange()
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        float startTime = Time.time;
        float endTime = startTime;
        while (endTime - startTime <= 0.2f)
        {
            endTime = Time.time;
            yield return waitTime;
            spriteRenderer.material = HitMaterial;
            yield return waitTime; // �ظ�ʹ��ͬһwaitforsecond����ֹGC����
            spriteRenderer.material = originMaterial;
        }
        spriteRenderer.material = originMaterial;
    }

    #region ��Ч

    // ����1��Ч
    public void Attack1Sound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.SamuraiAttack1);
    }

    // ����1�ε���Ч
    public void Attack2DrawNormalSound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.SamuraiDrawNormal);
    }

    // ����2��Ч
    public void Attack2Sound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.SamuraiAttack2);
    }

    // ����2�ε���Ч
    public void Attack2DrawSound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.SamuraiDraw);
    }

    // ����3��Ч
    public void Attack3Sound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.SamuraiAttack3);
    }
    #endregion
}

// ����
public class SamuraiIdleState : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;
    private float idleTimer;

    public SamuraiIdleState(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        samuraiBlackboard.samuraiAnimator.Play("Idle");
    }

    public void OnExit()
    {
        idleTimer = 0;
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        idleTimer += Time.deltaTime;
        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        if (idleTimer >= 0.5f)
        {
            samuraiFSM.SwitchState(StateType.Chase);
        }

        if (samuraiBlackboard.rb.velocity.y < 0f)
        { // ����
            samuraiFSM.SwitchState(StateType.Fall);
        }
    }
}

// ׷��
public class SamuraiChaseState : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;
    

    public SamuraiChaseState(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        samuraiBlackboard.chooseAttackSkill = 0;
        if (samuraiBlackboard.health <= samuraiBlackboard.maxHealth / 2)
        {
            samuraiBlackboard.chooseAttackSkill = Random.Range(0, 3);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        samuraiFSM.FlipToTransform(samuraiBlackboard.samuraiTransform, samuraiBlackboard.targetTransform); // ʼ���������

        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        if (Vector2.Distance(samuraiBlackboard.samuraiTransform.position, samuraiBlackboard.targetTransform.position) < 1 
            || samuraiBlackboard.targetTransform.position.y - samuraiBlackboard.samuraiTransform.position.y > 0.5)
        { // ����ÿջ��ѽӽ�
            samuraiBlackboard.samuraiAnimator.Play("Idle");
        }
        else
        { // ׷������
            samuraiBlackboard.samuraiAnimator.Play("Run");
            samuraiBlackboard.samuraiTransform.position = Vector2.MoveTowards(samuraiBlackboard.samuraiTransform.position,
                                                                    samuraiBlackboard.targetTransform.position,
                                                                    samuraiBlackboard.speed * Time.deltaTime);
        }

        // ���ݳ��о��߳���
        if (samuraiBlackboard.chooseAttackSkill == 0)
        {
            RaycastHit2D ray = Physics2D.Raycast(samuraiBlackboard.samuraiTransform.position, Vector3.right * samuraiBlackboard.samuraiTransform.localScale.x, samuraiBlackboard.attack1Length, samuraiBlackboard.targetLayer);
            if (ray.collider != null)
            {
                samuraiFSM.SwitchState(StateType.Attack1);
            }
        }
        else if (samuraiBlackboard.chooseAttackSkill == 1)
        {
            RaycastHit2D ray = Physics2D.Raycast(samuraiBlackboard.samuraiTransform.position, Vector3.right * samuraiBlackboard.samuraiTransform.localScale.x, samuraiBlackboard.attack2Length, samuraiBlackboard.targetLayer);
            if (ray.collider != null)
            {
                samuraiFSM.SwitchState(StateType.Attack2);
            }
        }
        else if (samuraiBlackboard.chooseAttackSkill == 2)
        {
            RaycastHit2D ray = Physics2D.Raycast(samuraiBlackboard.samuraiTransform.position, Vector3.right * samuraiBlackboard.samuraiTransform.localScale.x, samuraiBlackboard.attack3Length, samuraiBlackboard.targetLayer);
            if (ray.collider != null)
            {
                samuraiFSM.SwitchState(StateType.Attack1);
            }
        }
    }
}

// ���˻ر�
public class SamuraiEvadeState : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

    private float evadeTimer;

    public SamuraiEvadeState(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        evadeTimer = 0;
        samuraiFSM.FlipToTransform(samuraiBlackboard.samuraiTransform, samuraiBlackboard.targetTransform);
        samuraiBlackboard.samuraiAnimator.Play("Run");
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        evadeTimer += Time.deltaTime;
        if (evadeTimer <= 0.3f)
        { // ����
            samuraiBlackboard.rb.velocity = new Vector2(-samuraiBlackboard.samuraiTransform.localScale.x * 4 * Time.deltaTime * 250, 0);
            RaycastHit2D hit = Physics2D.Raycast(samuraiBlackboard.samuraiTransform.position, samuraiBlackboard.samuraiTransform.localScale.x * Vector2.left, 2f, samuraiBlackboard.airWall);
            Debug.DrawRay(samuraiBlackboard.samuraiTransform.position, samuraiBlackboard.samuraiTransform.localScale.x * Vector2.left, Color.yellow);
            if (hit.collider != null)
            { // ��ֹ��ǽ
                samuraiBlackboard.rb.velocity = Vector2.zero;
                samuraiFSM.SwitchState(StateType.Idle);
            }
        }
        else if (evadeTimer > 0.5f)
        { // ������ʱ
            samuraiFSM.SwitchState(StateType.Idle);
        }
    }
}

// ����1
public class SamuraiAttack1State : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

    private AnimatorStateInfo info;

    public SamuraiAttack1State(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        samuraiFSM.FlipToTransform(samuraiBlackboard.samuraiTransform, samuraiBlackboard.targetTransform);
        samuraiBlackboard.attack = 15;
        samuraiBlackboard.attackPause = 4;
        samuraiBlackboard.samuraiTransform.GetChild(0).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 1;
        samuraiBlackboard.samuraiAnimator.Play("Attack1");
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            if (samuraiBlackboard.chooseAttackSkill < 2)
            {
                samuraiFSM.SwitchState(StateType.Attack3);
            }
            else if (samuraiBlackboard.chooseAttackSkill == 2)
            {
                samuraiFSM.SwitchState(StateType.Attack4);
            }
        }
    }
}

// ����2
public class SamuraiAttack2State : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

    private AnimatorStateInfo info;

    public SamuraiAttack2State(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        samuraiFSM.FlipToTransform(samuraiBlackboard.samuraiTransform, samuraiBlackboard.targetTransform);
        samuraiBlackboard.attack = 25;
        samuraiBlackboard.attackPause = 8;
        samuraiBlackboard.samuraiTransform.GetChild(0).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 3;
        samuraiBlackboard.samuraiAnimator.Play("Attack2");
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            samuraiFSM.SwitchState(StateType.Attack3);
        }
    }
}

// ����3
public class SamuraiAttack3State : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

    private AnimatorStateInfo info;
    private int choose;

    public SamuraiAttack3State(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        choose = Random.Range(0, 3);
        samuraiBlackboard.attack = 15;
        samuraiBlackboard.attackPause = 6;
        samuraiBlackboard.samuraiTransform.GetChild(0).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 2;
        samuraiBlackboard.samuraiAnimator.Play("Attack3");
        if (samuraiBlackboard.chooseAttackSkill == 1)
        { // ����2ת������3ʱ����������
            samuraiFSM.FlipToTransform(samuraiBlackboard.samuraiTransform, samuraiBlackboard.targetTransform);
        }
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);

        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        if (info.normalizedTime >= 0.95f && choose < 2)
        {
            samuraiFSM.SwitchState(StateType.Evade);
        }
        else if (info.normalizedTime >= 0.95f && choose == 2)
        {
            samuraiFSM.SwitchState(StateType.Jump);
        }
    }
}

// ����4
public class SamuraiAttack4State : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

    private AnimatorStateInfo info;
    private int choose; // �ر��жϣ�����or����
    private Vector2 dir;
    private bool slash; // �Ƿ��ѷ�������
    private Vector3 front; // bossǰ��

    public SamuraiAttack4State(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        slash = false;
        choose = Random.Range(0, 3);
        samuraiBlackboard.attack = 10;
        samuraiBlackboard.attackPause = 6;
        samuraiBlackboard.samuraiTransform.GetChild(0).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 2;
        samuraiBlackboard.samuraiAnimator.Play("Attack3");
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);

        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        if (info.normalizedTime >= 0.6f && !slash)
        {
            front.Set(0.5f * samuraiBlackboard.samuraiTransform.localScale.x, 0, 0);
            slash = true;
            if (samuraiBlackboard.health <= samuraiBlackboard.maxHealth / 2)
            {
                GameObject gameObject = ObjectPool.Instance.Get(samuraiBlackboard.bulletPre);
                gameObject.transform.position = samuraiBlackboard.samuraiTransform.position + front;
                dir.Set(samuraiBlackboard.samuraiTransform.localScale.x, 0);
                gameObject.GetComponent<SlashFX>().SetDirection(dir);
            }
            AudioSourceManager.Instance.PlaySound(GlobalAudioClips.SamuraiAttack4);
        }

        if (info.normalizedTime >= 0.95f && choose < 2)
        {
            samuraiFSM.SwitchState(StateType.Evade);
        }
        else if (info.normalizedTime >= 0.95f && choose == 2)
        {
            samuraiFSM.SwitchState(StateType.Jump);
        }
    }
}

// ��Ծ
public class SamuraiJumpState : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

    public SamuraiJumpState(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
        samuraiFSM.FlipToTransform(samuraiBlackboard.samuraiTransform, samuraiBlackboard.targetTransform);
        samuraiBlackboard.samuraiAnimator.Play("Jump");
        samuraiBlackboard.rb.AddForce(new Vector2(-samuraiBlackboard.samuraiTransform.localScale.x * 4f, 6f), ForceMode2D.Impulse);
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        if (samuraiBlackboard.rb.velocity.y < 0f)
        { // ����
            samuraiFSM.SwitchState(StateType.Fall);
        }
    }
}

// ����
public class SamuraiFallState : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

    public SamuraiFallState(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        samuraiBlackboard.samuraiAnimator.Play("Fall");
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // �ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        if (samuraiBlackboard.isGround == true)
        { // ���
            samuraiFSM.SwitchState(StateType.Idle);
        }
        else
        { // ��������
            samuraiBlackboard.rb.velocity += Vector2.up * Physics2D.gravity.y * (samuraiBlackboard.fallMultiplier - 1) * Time.deltaTime;
        }
    }
}

// ����
public class SamuraiHurtState : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

    private AnimatorStateInfo info;

    public SamuraiHurtState(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        samuraiBlackboard.isHit = false;

        samuraiBlackboard.samuraiAnimator.Play("Hit");
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);
        
        if (samuraiBlackboard.health <= 0f)
        { // ����Ϊ0������
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (info.normalizedTime < 0.99f && samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0f)
        { // �ܻ�ʱ�����ܻ�������Ϊ0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
        else if (info.normalizedTime >= 0.95f && samuraiBlackboard.isGround == true)
        { // ��������ܻ�����
            samuraiFSM.SwitchState(StateType.Jump);
        }
        else if (info.normalizedTime >= 0.95f)
        {
            samuraiFSM.SwitchState(StateType.Idle);
        }
    }
}

// ����
public class SamuraiDeadState : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;
    private bool isEnter = false;

    public SamuraiDeadState(FSM fsm)
    {
        samuraiFSM = fsm;
        samuraiBlackboard = fsm.blackboard as SamuraiBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        if (!isEnter)
        {
            isEnter = true;
            GameObject dropItem = GameObject.Instantiate(samuraiBlackboard.dropItemPre, samuraiBlackboard.samuraiTransform.position, Quaternion.identity); // ����ս��Ʒ
            for (int i = 0; i < Random.Range(samuraiBlackboard.min, samuraiBlackboard.max); i++)
            { // ������
                GameObject.Instantiate(samuraiBlackboard.coin, samuraiBlackboard.samuraiTransform.position, Quaternion.identity);
            }
        }
        samuraiBlackboard.rb.velocity = Vector2.zero;
        samuraiBlackboard.rb.gravityScale = 0;
        samuraiBlackboard.col.enabled = false;
        GameManager.Instance.userData.killBoss = true;

        samuraiBlackboard.samuraiAnimator.Play("Dead");
    }

    public void OnExit()
    {
        samuraiBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {

    }
}