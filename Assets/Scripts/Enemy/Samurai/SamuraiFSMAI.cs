using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SamuraiBlackboard : Blackboard
{
    // 获取对象刚体
    public Rigidbody2D rb;
    // 获取对象动画控制器
    public Animator samuraiAnimator;
    // 使状态类能调用对象的transform
    public Transform samuraiTransform;
    // 获得目标对象的transform
    public Transform targetTransform;

    [Header("基础参数")]
    public float maxHealth;
    public float toughness;

    [Header("速度")]
    public float fallMultiplier;

    [Header("各攻击检测范围")]
    public int chooseAttackSkill;
    public LayerMask targetLayer;
    public float attack1Length;
    public float attack2Length;

    [Header("状态判断")]
    public bool isHit;
    public bool isGround;

    [Header("受伤位移")]
    public float hurtOffset;

    [Header("生成预制体")]
    public GameObject bulletPre; // 射弹
    public GameObject blood; // 血液粒子
    public GameObject dropItemPre; // 掉落物
    public GameObject coin; // 金币

    [Header("攻击力(根据攻击类型变换)")]
    public float attack;

    [Header("金币掉落数范围")]
    public int min;
    public int max;
}

public class SamuraiFSMAI : MonoBehaviour
{
    private FSM samuraiFSM;
    public SamuraiBlackboard samuraiBlackboard = new SamuraiBlackboard();
    [SerializeField] private GameObject shinningEffect;
    [SerializeField] private bool healToughness; // 恢复韧性协程是否已被调用
    
    [Header("材质")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material originMaterial;
    [SerializeField] private Material HitMaterial;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        samuraiBlackboard.rb = GetComponent<Rigidbody2D>();
        samuraiBlackboard.samuraiAnimator = GetComponent<Animator>();
        samuraiBlackboard.samuraiTransform = transform;
        samuraiBlackboard.targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    private void OnEnable()
    {
        samuraiBlackboard.maxHealth = samuraiBlackboard.health; // 记录生命上限
    }

    private void Start()
    {
        samuraiFSM = new FSM(samuraiBlackboard);

        samuraiFSM.AddState(StateType.Idle, new SamuraiIdleState(samuraiFSM));
        samuraiFSM.AddState(StateType.Chase, new SamuraiChaseState(samuraiFSM));
        samuraiFSM.AddState(StateType.Attack1, new SamuraiAttack1State(samuraiFSM));
        samuraiFSM.AddState(StateType.Attack2, new SamuraiAttack2State(samuraiFSM));
        samuraiFSM.AddState(StateType.Attack3, new SamuraiAttack3State(samuraiFSM));
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
    }

    // 扣血方法
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

    // 恢复韧性
    private void HealSamuraiToughness()
    {
        if (samuraiBlackboard.toughness <= 0 && healToughness == false)
        { // 韧性小于等于0且未启动恢复协程
            healToughness = true;
            StartCoroutine(HealToughness());
        }
    }

    // 判断是否在地面，用于子物体通信执行
    private void UpdateGroundStatus(bool isGround)
    {
        samuraiBlackboard.isGround = isGround;
    }

    // 拔刀特效激活
    public void Shinning()
    {
        shinningEffect.SetActive(true);
    }

    // 攻击位移
    public void AttackOffset(float offset)
    {
        samuraiBlackboard.rb.velocity = new Vector2(transform.localScale.x * offset, samuraiBlackboard.rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("命中玩家");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.right * transform.localScale.x * samuraiBlackboard.attack1Length);
        Gizmos.DrawRay(transform.position - new Vector3(0, 0.1f, 0), Vector3.right * transform.localScale.x * samuraiBlackboard.attack2Length);
        Gizmos.color = Color.blue;
    }

    // 恢复韧性
    IEnumerator HealToughness()
    {
        yield return new WaitForSeconds(4f);
        samuraiBlackboard.toughness = 100;
        healToughness = false;
    }

    // 受击材质切换
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
            yield return waitTime; // 重复使用同一waitforsecond，防止GC产生
            spriteRenderer.material = originMaterial;
        }
        spriteRenderer.material = originMaterial;
    }
}

// 闲置
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
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        idleTimer += Time.deltaTime;
        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // 受击且韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        if (idleTimer >= 0.5f)
        {
            samuraiFSM.SwitchState(StateType.Chase);
        }

        if (samuraiBlackboard.rb.velocity.y < 0f)
        { // 下落
            samuraiFSM.SwitchState(StateType.Fall);
        }
    }
}

// 追击
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
            Debug.Log("攻击2");
            samuraiBlackboard.chooseAttackSkill = Random.Range(0, 2);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // 受击且韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
    }

    public void OnExit()
    {

    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        samuraiFSM.FlipToTransform(samuraiBlackboard.samuraiTransform, samuraiBlackboard.targetTransform); // 始终面向敌人

        if (Vector2.Distance(samuraiBlackboard.samuraiTransform.position, samuraiBlackboard.targetTransform.position) < 1 
            || samuraiBlackboard.targetTransform.position.y - samuraiBlackboard.samuraiTransform.position.y > 0.5)
        { // 玩家置空或已接近
            samuraiBlackboard.samuraiAnimator.Play("Idle");
        }
        else
        { // 追击敌人
            samuraiBlackboard.samuraiAnimator.Play("Run");
            samuraiBlackboard.samuraiTransform.position = Vector2.MoveTowards(samuraiBlackboard.samuraiTransform.position,
                                                                    samuraiBlackboard.targetTransform.position,
                                                                    samuraiBlackboard.speed * Time.deltaTime);
        }

        // 根据出招决策出招
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
    }
}

// 后退回避
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
        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // 受击且韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }

        evadeTimer += Time.deltaTime;
        if (evadeTimer <= 0.5f)
        { // 后退
            samuraiBlackboard.rb.velocity = new Vector2(-samuraiBlackboard.samuraiTransform.localScale.x * samuraiBlackboard.speed * Time.deltaTime * 600, 0);
        }
        else if (evadeTimer > 0.5f)
        { // 超过计时
            samuraiFSM.SwitchState(StateType.Idle);
        }
    }
}

// 攻击1
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
        samuraiBlackboard.samuraiAnimator.Play("Attack1");
    }

    public void OnExit()
    {
        
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            samuraiFSM.SwitchState(StateType.Attack3);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // 受击且韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 攻击2
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
        samuraiBlackboard.samuraiAnimator.Play("Attack2");
    }

    public void OnExit()
    {

    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            samuraiFSM.SwitchState(StateType.Attack3);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // 受击且韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 攻击3
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
        samuraiBlackboard.samuraiAnimator.Play("Attack3");
        if (samuraiBlackboard.chooseAttackSkill == 1)
        { // 攻击2转来攻击3时，调整朝向
            samuraiFSM.FlipToTransform(samuraiBlackboard.samuraiTransform, samuraiBlackboard.targetTransform);
        }
    }

    public void OnExit()
    {

    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f && choose < 2)
        {
            samuraiFSM.SwitchState(StateType.Evade);
        }
        else if (info.normalizedTime >= 0.95f && choose == 2)
        {
            samuraiFSM.SwitchState(StateType.Jump);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // 受击且韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 跳跃
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

    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        if (samuraiBlackboard.rb.velocity.y < 0f)
        { // 下落
            samuraiFSM.SwitchState(StateType.Fall);
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // 受击且韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 下落
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

    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        if (samuraiBlackboard.isGround == true)
        { // 落地
            samuraiFSM.SwitchState(StateType.Idle);
        }
        else
        { // 加速下落
            samuraiBlackboard.rb.velocity += Vector2.up * Physics2D.gravity.y * (samuraiBlackboard.fallMultiplier - 1) * Time.deltaTime;
        }

        if (samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0)
        { // 受击且韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 受伤
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
        
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        info = samuraiBlackboard.samuraiAnimator.GetCurrentAnimatorStateInfo(0);
        
        if (samuraiBlackboard.health <= 0f)
        { // 生命为0，死亡
            samuraiFSM.SwitchState(StateType.Dead);
        }

        if (info.normalizedTime < 0.99f && samuraiBlackboard.isHit == true && samuraiBlackboard.toughness <= 0f)
        { // 受击时继续受击，韧性为0
            samuraiFSM.SwitchState(StateType.Hurt);
        }
        else if (info.normalizedTime >= 0.95f && samuraiBlackboard.isGround == true)
        { // 播放完毕受击动画
            samuraiFSM.SwitchState(StateType.Jump);
        }
        else if (info.normalizedTime >= 0.95f)
        {
            samuraiFSM.SwitchState(StateType.Idle);
        }
    }
}

// 死亡
public class SamuraiDeadState : Istate
{
    private FSM samuraiFSM;
    private SamuraiBlackboard samuraiBlackboard;

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
        samuraiBlackboard.samuraiAnimator.Play("Dead");

        GameObject dropItem = GameObject.Instantiate(samuraiBlackboard.dropItemPre, samuraiBlackboard.samuraiTransform.position, Quaternion.identity); // 掉落战利品
        for (int i = 0; i < Random.Range(samuraiBlackboard.min, samuraiBlackboard.max); i++)
        { // 掉落金币
            GameObject.Instantiate(samuraiBlackboard.coin, samuraiBlackboard.samuraiTransform.position, Quaternion.identity);
        }
    }

    public void OnExit()
    {

    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {

    }
}