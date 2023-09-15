using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Cinemachine;

[Serializable]
public class GoblinBlackboard : Blackboard
{
    // 获取对象刚体
    public Rigidbody2D rb;
    // 获取对象动画控制器
    public Animator goblinAnimator;
    // 使状态类能调用对象的transform
    public Transform goblinTransform;
    // 获得目标对象的transform
    public Transform targetTransform;

    [Header("待命、销毁、攻击间隔时间")]
    public float idleTime;
    public float deadTime;
    public float coolTime;

    [Header("移动速度")]
    public float patrolSpeed;

    [Header("各攻击检测范围")]
    public LayerMask attackLayerMask;
    public Transform attackPoint;
    public Transform evadePoint;
    public float radius;
    public int chooseAttackSkill;

    [Header("状态判断")]
    public bool isHit;
    public bool isGround;

    [Header("攻击相关")]
    public float attack1Offset;
    public float attack2Offset;
    public float attack3Offset;
    public int attackPause;

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

    [Header("移动范围(原位置加减)")]
    public Vector3 originPos;
    public float leftOffset;
    public float rightOffset;
}


public class GoblinFSMAI : MonoBehaviour
{
    private FSM goblinFSM;
    public GoblinBlackboard goblinBlackboard = new GoblinBlackboard();

    private void Awake()
    {
        goblinBlackboard.goblinAnimator = GetComponent<Animator>();
        goblinBlackboard.rb = GetComponent<Rigidbody2D>();
        goblinBlackboard.goblinTransform = this.transform;
        goblinBlackboard.targetTransform = this.transform;
    }

    void Start()
    {
        goblinFSM = new FSM(goblinBlackboard);

        goblinFSM.AddState(StateType.Idle, new GoblinIdleState(goblinFSM));
        goblinFSM.AddState(StateType.Patrol, new GoblinPatrolState(goblinFSM));
        goblinFSM.AddState(StateType.Chase, new GoblinChaseState(goblinFSM));
        goblinFSM.AddState(StateType.Attack1, new GoblinAttack1State(goblinFSM));
        goblinFSM.AddState(StateType.Attack2, new GoblinAttack2State(goblinFSM));
        goblinFSM.AddState(StateType.Attack3, new GoblinAttack3State(goblinFSM));
        goblinFSM.AddState(StateType.Evade, new GoblinEvadeState(goblinFSM));
        goblinFSM.AddState(StateType.Cool, new GoblinCoolState(goblinFSM));
        goblinFSM.AddState(StateType.Hurt, new GoblinHurtState(goblinFSM));
        goblinFSM.AddState(StateType.Dead, new GoblinDeadState(goblinFSM));

        goblinFSM.SwitchState(StateType.Idle);

        goblinBlackboard.originPos = transform.position; // 获取初始位置，便于指定范围内随机巡逻
    }

    void Update()
    {
        goblinFSM.OnCheck();
        goblinFSM.OnUpdate();
    }

    private void FixedUpdate()
    {
        goblinFSM.OnFixUpdate();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(goblinBlackboard.targetTransform.position, 0.1f);
        Gizmos.DrawWireSphere(goblinBlackboard.attackPoint.position, 0.35f);
        Gizmos.DrawWireSphere(goblinBlackboard.evadePoint.position, 0.35f);
        Gizmos.DrawRay(goblinBlackboard.goblinTransform.position + new Vector3(0, 0.1f, 0), Vector2.right * goblinBlackboard.goblinTransform.localScale.x * 2f);
        Gizmos.DrawRay(goblinBlackboard.goblinTransform.position - new Vector3(0, 0.2f, 0), Vector2.right * goblinBlackboard.goblinTransform.localScale.x * 3.2f);
    }

    // 攻击位移帧事件
    private void AttackOffset(int chooseSkill)
    {
        if (chooseSkill == 1)
        {
            goblinBlackboard.rb.velocity = new Vector2(goblinBlackboard.goblinTransform.localScale.x * goblinBlackboard.attack1Offset
                                                        , goblinBlackboard.rb.velocity.y);
        }
        else if (chooseSkill == 2)
        {
            goblinBlackboard.rb.velocity = new Vector2(goblinBlackboard.goblinTransform.localScale.x * goblinBlackboard.attack2Offset
                                                        , goblinBlackboard.rb.velocity.y);
        }
        else if (chooseSkill == 3)
        {
            goblinBlackboard.rb.velocity = new Vector2(goblinBlackboard.goblinTransform.localScale.x * goblinBlackboard.attack3Offset
                                                        , goblinBlackboard.rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerControll player = collision.GetComponent<PlayerControll>();
            player.getHurt(goblinBlackboard.attack, transform.position);
            GameManager.Instance.HitPause(goblinBlackboard.attackPause);
            transform.GetChild(2).GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
    }

    // 受击扣除血量方法(在玩家脚本中调用)
    public void getHurt(float damage)
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.EnemyHurt);
        goblinBlackboard.isHit = true;
        goblinBlackboard.health -= damage;
    }

    // 设置根据目标，用于子物体通信执行
    public void SetTarget(Transform target)
    {
        goblinBlackboard.targetTransform = target;
    }

    // 判断是否在地面，用于子物体通信执行
    private void UpdateGroundStatus(bool isGround)
    {
        goblinBlackboard.isGround = isGround;
    }

    // 播放劈砍音效
    public void GoblinAttackSound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.GoblinAttack);
    }
}

// 闲置
public class GoblinIdleState : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;
    private float idleTimer;

    public GoblinIdleState(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        idleTimer = 0;
        goblinBlackboard.goblinAnimator.Play("Idle");
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
        if (goblinBlackboard.isHit == true)
        {
            goblinFSM.SwitchState(StateType.Hurt);
        }

        if (goblinBlackboard.targetTransform.tag == "Player")
        {
            goblinFSM.SwitchState(StateType.Chase);
        }

        idleTimer += Time.fixedDeltaTime;
        if (idleTimer >= goblinBlackboard.idleTime)
        {
            goblinFSM.SwitchState(StateType.Patrol);
        }
    }
}

// 巡逻
public class GoblinPatrolState : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;
    private Vector2 targetPosition;

    public GoblinPatrolState(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        goblinBlackboard.goblinAnimator.Play("Walk");

        float offsetX = Random.Range(goblinBlackboard.originPos.x - goblinBlackboard.leftOffset, goblinBlackboard.originPos.x + goblinBlackboard.rightOffset);
        targetPosition = new Vector2(offsetX, goblinBlackboard.targetTransform.position.y);
    }

    public void OnExit()
    {
        targetPosition = new Vector2(0, 0);
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (goblinBlackboard.isHit == true)
        {
            goblinFSM.SwitchState(StateType.Hurt);
        }

        goblinFSM.FlipToPoint(goblinBlackboard.goblinTransform, targetPosition);

        if (goblinBlackboard.targetTransform.tag == "Player")
        { // 有目标时，进入追击状态
            goblinFSM.SwitchState(StateType.Chase);
        }

        if (Vector2.Distance(targetPosition, goblinBlackboard.goblinTransform.position) < 0.1f)
        { // 无目标，且到达目标点
            goblinFSM.SwitchState(StateType.Idle);
        }
        else
        { // 无目标，未到达目标点
            goblinBlackboard.goblinTransform.position = Vector2.MoveTowards(goblinBlackboard.goblinTransform.position,
                                                                    targetPosition,
                                                                    goblinBlackboard.patrolSpeed * Time.deltaTime);
        }
    }
}

// 追击
public class GoblinChaseState : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;
    private Transform targetPosition;

    public GoblinChaseState(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        goblinBlackboard.chooseAttackSkill = Random.Range(0, 4);

        targetPosition = goblinBlackboard.targetTransform;

        goblinBlackboard.goblinAnimator.Play("Walk");
    }

    public void OnExit()
    {
        goblinBlackboard.chooseAttackSkill = -1;
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        if (goblinBlackboard.isHit == true)
        {
            goblinFSM.SwitchState(StateType.Hurt);
        }

        goblinFSM.FlipToTransform(goblinBlackboard.goblinTransform, targetPosition);
        
        if (targetPosition.transform.position.y - goblinBlackboard.goblinTransform.position.y > 0.8f)
        { // 玩家位于上方时，停止移动等待
            goblinBlackboard.goblinAnimator.Play("Idle");
        }
        else
        { // 玩家位于地面，接近目标
            goblinBlackboard.goblinTransform.position = Vector2.MoveTowards(goblinBlackboard.goblinTransform.position,
                                                                targetPosition.position,
                                                                goblinBlackboard.patrolSpeed * Time.deltaTime);

            goblinBlackboard.goblinAnimator.Play("Walk");
        }

        if (Vector2.Distance(goblinBlackboard.goblinTransform.position, goblinBlackboard.targetTransform.position) > 5f)
        { // 丢失目标
            goblinBlackboard.targetTransform = goblinBlackboard.goblinTransform;
            goblinFSM.SwitchState(StateType.Idle);
        }

        
        // 攻击逻辑判断
        if (goblinBlackboard.chooseAttackSkill < 2)
        { // 使用攻击1
            goblinBlackboard.goblinTransform.GetChild(2).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 0.6f;
            goblinBlackboard.attackPause = 4;
            if (Physics2D.OverlapCircle(goblinBlackboard.attackPoint.position, goblinBlackboard.radius, goblinBlackboard.attackLayerMask))
            {
                goblinFSM.SwitchState(StateType.Attack1);
            }
        }
        else if (goblinBlackboard.chooseAttackSkill == 2)
        { // 执行回避
            if (Physics2D.OverlapCircle(goblinBlackboard.evadePoint.position, goblinBlackboard.radius, goblinBlackboard.attackLayerMask))
            {
                goblinFSM.SwitchState(StateType.Evade);
            }
        }
        else if (goblinBlackboard.chooseAttackSkill == 3)
        { // 使用攻击3
            RaycastHit2D target = Physics2D.Raycast(goblinBlackboard.goblinTransform.position, Vector2.right * goblinBlackboard.goblinTransform.localScale.x, 2f, goblinBlackboard.attackLayerMask);
            if (target.collider != null)
            {
                goblinFSM.SwitchState(StateType.Attack3);
            }
        }
    }
}

// 攻击1
public class GoblinAttack1State : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info;

    public GoblinAttack1State(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        targetPosition = goblinBlackboard.targetTransform.position;
        goblinFSM.FlipToPoint(goblinBlackboard.goblinTransform, targetPosition);
        goblinBlackboard.attack = 10f;

        goblinBlackboard.goblinAnimator.Play("Attack1");
    }

    public void OnExit()
    {
        goblinBlackboard.rb.velocity = new Vector2(0, goblinBlackboard.rb.velocity.y);
        goblinBlackboard.attack = 0f;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (goblinBlackboard.isHit == true)
        {
            goblinFSM.SwitchState(StateType.Hurt);
        }

        info = goblinBlackboard.goblinAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            goblinFSM.SwitchState(StateType.Cool);
        }
    }
}

// 攻击2
public class GoblinAttack2State : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info;

    public GoblinAttack2State(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        targetPosition = goblinBlackboard.targetTransform.position;
        goblinFSM.FlipToPoint(goblinBlackboard.goblinTransform, targetPosition);
        goblinBlackboard.attack = 20f;

        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerDash);
        goblinBlackboard.goblinAnimator.Play("Attack2");
    }

    public void OnExit()
    {
        // 重置x速度
        goblinBlackboard.rb.velocity = new Vector2(0, goblinBlackboard.rb.velocity.y);
        goblinBlackboard.attack = 0f;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (goblinBlackboard.isHit == true)
        {
            goblinFSM.SwitchState(StateType.Hurt);
        }

        info = goblinBlackboard.goblinAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            goblinFSM.SwitchState(StateType.Cool);
        }
    }
}

// 攻击3
public class GoblinAttack3State : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info;
    private Vector2 initialVelocity;

    public GoblinAttack3State(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        targetPosition = goblinBlackboard.goblinTransform.position;

        goblinFSM.FlipToPoint(goblinBlackboard.goblinTransform, goblinBlackboard.targetTransform.position);

        goblinBlackboard.goblinAnimator.Play("Attack3");

        // 计算抛掷炸弹的初速度
        Vector2 dir = (goblinBlackboard.targetTransform.position - goblinBlackboard.goblinTransform.position).normalized;
        float distance = Vector2.Distance(goblinBlackboard.targetTransform.position, targetPosition);
        float speedValue = (distance * Physics.gravity.magnitude) / (Mathf.Sin(2 * Mathf.Deg2Rad * 60));
        speedValue = Mathf.Sqrt(speedValue);
        initialVelocity = dir * speedValue;
        Debug.Log("发射速度：" + initialVelocity);
    }

    public void OnExit()
    {

    }

    public void OnFixUpdate()
    {
        targetPosition = goblinBlackboard.targetTransform.position;
    }

    public void OnUpdate()
    {
        if (goblinBlackboard.isHit == true)
        {
            goblinFSM.SwitchState(StateType.Hurt);
        }

        info = goblinBlackboard.goblinAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            GameObject bullet = ObjectPool.Instance.Get(goblinBlackboard.bulletPre);
            bullet.transform.position = goblinBlackboard.goblinTransform.position;
            bullet.GetComponent<Bomb>().SetTarget(initialVelocity);

            goblinFSM.SwitchState(StateType.Cool);
        }
    }
}

// 回避
public class GoblinEvadeState : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;
    RaycastHit2D target;

    public GoblinEvadeState(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        // 后跳
        goblinBlackboard.goblinTransform.gameObject.layer = 12;
        goblinBlackboard.rb.velocity = new Vector2(-2f * goblinBlackboard.goblinTransform.localScale.x, 4);
        goblinBlackboard.goblinAnimator.Play("Evade");
    }

    public void OnExit()
    {
        goblinBlackboard.goblinTransform.gameObject.layer = 6;
    }

    public void OnFixUpdate()
    {
        target = Physics2D.Raycast(goblinBlackboard.goblinTransform.position - new Vector3(0, 0.2f, 0), Vector2.right * goblinBlackboard.goblinTransform.localScale.x, 3.2f, goblinBlackboard.attackLayerMask);
    }

    public void OnUpdate()
    {
        if (goblinBlackboard.isHit == true)
        {
            goblinFSM.SwitchState(StateType.Hurt);
        }

        if (goblinBlackboard.isGround == true && MathF.Abs(goblinBlackboard.rb.velocity.y) <= 0.1f)
        {
            if (target.collider != null)
            {
                goblinBlackboard.goblinTransform.GetChild(2).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 1f;
                goblinBlackboard.attackPause = 8;
                goblinFSM.SwitchState(StateType.Attack2);
            }
            else
            {
                goblinFSM.SwitchState(StateType.Chase);
            }
        }
    }
}

// 攻击冷却
public class GoblinCoolState : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;
    private float coolTimer;

    public GoblinCoolState(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        coolTimer = 0;
        goblinBlackboard.goblinAnimator.Play("Idle");
    }

    public void OnExit()
    {
        coolTimer = 0;
    }

    public void OnFixUpdate()
    {
        coolTimer += Time.fixedDeltaTime;
    }

    public void OnUpdate()
    {
        if (goblinBlackboard.isHit == true)
        {
            goblinFSM.SwitchState(StateType.Hurt);
        }

        if (Vector2.Distance(goblinBlackboard.targetTransform.position, goblinBlackboard.goblinTransform.position) < 2f)
        {
            if (coolTimer >= goblinBlackboard.coolTime)
            { // 结束等待
                if (goblinBlackboard.targetTransform.tag == "Player")
                { // 若有攻击目标，追击
                    goblinFSM.SwitchState(StateType.Chase);
                }
                else
                { // 若无攻击目标，闲置
                    goblinFSM.SwitchState(StateType.Idle);
                }
            }
        }
        else
        {
            goblinFSM.SwitchState(StateType.Chase);
        }
    }
}

// 受伤
public class GoblinHurtState : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;

    private AnimatorStateInfo info;

    public GoblinHurtState(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        goblinBlackboard.goblinAnimator.Play("Hurt");
        GameObject blood = ObjectPool.Instance.Get(goblinBlackboard.blood);
        blood.transform.position = goblinBlackboard.goblinTransform.position;
    }

    public void OnExit()
    {
        goblinBlackboard.isHit = false;
        goblinBlackboard.rb.velocity = new Vector2(0, goblinBlackboard.rb.velocity.y);
    }

    public void OnFixUpdate()
    {

    }

    public void OnUpdate()
    {
        info = goblinBlackboard.goblinAnimator.GetCurrentAnimatorStateInfo(0);

        if (goblinBlackboard.health <= 0f)
        {
            goblinFSM.SwitchState(StateType.Dead);
        }
        else if (info.normalizedTime > 0.99f)
        {
            goblinBlackboard.targetTransform = GameObject.FindWithTag("Player").transform;
            goblinFSM.SwitchState(StateType.Chase);
        }
        else if (info.normalizedTime <= 0.99f)
        {
            goblinBlackboard.rb.velocity = new Vector2(-goblinBlackboard.hurtOffset * goblinBlackboard.goblinTransform.localScale.x, goblinBlackboard.rb.velocity.y);
            if (goblinBlackboard.isHit == true)
            { // 受击时再次受到攻击
                goblinFSM.SwitchState(StateType.Hurt);
            }
        }
    }
}

// 死亡
public class GoblinDeadState : Istate
{
    private FSM goblinFSM;
    private GoblinBlackboard goblinBlackboard;

    public float deadTimer;

    public GoblinDeadState(FSM fsm)
    {
        goblinFSM = fsm;
        goblinBlackboard = fsm.blackboard as GoblinBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        deadTimer = 0;
        goblinBlackboard.goblinAnimator.Play("Dead");
        goblinBlackboard.rb.velocity = new Vector2(-2f * goblinBlackboard.goblinTransform.localScale.x, 4);

        GameObject dropItem = GameObject.Instantiate(goblinBlackboard.dropItemPre, goblinBlackboard.goblinTransform.position, Quaternion.identity); // 掉落战利品
        for (int i = 0; i < Random.Range(goblinBlackboard.min, goblinBlackboard.max); i++)
        { // 掉落金币
            GameObject.Instantiate(goblinBlackboard.coin, goblinBlackboard.goblinTransform.position, Quaternion.identity);
        }

        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.GoblinDead);

        foreach (Mission mission in GameManager.Instance.missionList)
        {
            if (((mission.missionType == Mission.MissionType.KillEnemy) || (mission.missionType == Mission.MissionType.KillGoblin))
                && mission.missionStatus == Mission.MissionStatus.Accepted)
            { // 有击杀敌人或击杀哥布林，且未完成的任务
                mission.UpdateMissionData(1);
            }
        }
    }

    public void OnExit()
    {
        deadTimer = 0;
    }

    public void OnFixUpdate()
    {
        deadTimer += Time.fixedDeltaTime;
        if (deadTimer >= goblinBlackboard.deadTime)
        {
            GameObject.Destroy(goblinBlackboard.goblinTransform.gameObject);
        }
    }

    public void OnUpdate()
    {

    }
}