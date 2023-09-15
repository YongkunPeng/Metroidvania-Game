using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Cinemachine;

[Serializable]
public class MushroomBlackboard : Blackboard
{
    // 获取对象刚体
    public Rigidbody2D rb;
    // 获取对象动画控制器
    public Animator mushroomAnimator;
    // 使状态类能调用对象的transform
    public Transform mushroomTransform;
    // 获得目标对象的transform
    public Transform targetTransform;

    [Header("待命、销毁、攻击间隔时间")]
    public float idleTime; // 原地待命时间
    public float deadTime; // 对象销毁时间
    public float coolTime; // 攻击间隔时间

    [Header("移动速度")]
    public float patrolSpeed;

    [Header("各攻击检测范围")]
    public LayerMask attackLayerMask;
    public Transform attackPoint;
    public int chooseAttackSkill;
    public float radius1;
    public float radius2;
    public float radius3;

    [Header("状态判断")]
    public bool isHit; // 是否受击
    public bool isGround; // 是否着地

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

public class MushroomFSMAI : MonoBehaviour
{
    private FSM mushroomFSM;
    public MushroomBlackboard mushroomBlackboard = new MushroomBlackboard();

    private void Awake()
    {
        mushroomBlackboard.mushroomAnimator = transform.GetComponent<Animator>();
        mushroomBlackboard.rb = transform.GetComponent<Rigidbody2D>();
        mushroomBlackboard.mushroomTransform = this.transform;
        mushroomBlackboard.targetTransform = this.transform;
    }

    // Start is called before the first frame update
    void Start()
    {   
        mushroomFSM = new FSM(mushroomBlackboard);

        mushroomFSM.AddState(StateType.Idle, new MushroomIdleState(mushroomFSM));
        mushroomFSM.AddState(StateType.Patrol, new MushroomPatrolState(mushroomFSM));
        mushroomFSM.AddState(StateType.Chase, new MushroomChaseState(mushroomFSM));
        mushroomFSM.AddState(StateType.Hurt, new MushroomHurtState(mushroomFSM));
        mushroomFSM.AddState(StateType.Dead, new MushroomDeadState(mushroomFSM));
        mushroomFSM.AddState(StateType.Attack1, new MushroomAttack1State(mushroomFSM));
        mushroomFSM.AddState(StateType.Attack2, new MushroomAttack2State(mushroomFSM));
        mushroomFSM.AddState(StateType.Attack3, new MushroomAttack3State(mushroomFSM));
        mushroomFSM.AddState(StateType.Cool, new MushroomCoolState(mushroomFSM));

        mushroomFSM.SwitchState(StateType.Idle);

        mushroomBlackboard.originPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        mushroomFSM.OnCheck();
        mushroomFSM.OnUpdate();
    }

    void FixedUpdate()
    {
        mushroomFSM.OnFixUpdate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(mushroomBlackboard.attackPoint.position, mushroomBlackboard.radius1);
        Gizmos.DrawWireSphere(mushroomBlackboard.attackPoint.position, mushroomBlackboard.radius2);
        Gizmos.DrawWireSphere(mushroomBlackboard.mushroomTransform.position, mushroomBlackboard.radius3);
        Gizmos.DrawWireSphere(mushroomBlackboard.targetTransform.position, 0.1f);
    }

    // 攻击位移帧事件
    private void AttackOffset(int chooseSkill)
    {
        if(chooseSkill == 1)
        {
            mushroomBlackboard.rb.velocity = new Vector2(mushroomBlackboard.mushroomTransform.localScale.x * mushroomBlackboard.attack1Offset
                                                        , mushroomBlackboard.rb.velocity.y);
        }
        else if(chooseSkill == 2)
        {
            mushroomBlackboard.rb.velocity = new Vector2(mushroomBlackboard.mushroomTransform.localScale.x * mushroomBlackboard.attack2Offset
                                                        , mushroomBlackboard.rb.velocity.y);
        }
        else if(chooseSkill == 3)
        {
            mushroomBlackboard.rb.velocity = new Vector2(mushroomBlackboard.mushroomTransform.localScale.x * mushroomBlackboard.attack3Offset
                                                        , mushroomBlackboard.rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerControll player = collision.GetComponent<PlayerControll>();
            player.getHurt(mushroomBlackboard.attack, transform.position);
            GameManager.Instance.HitPause(mushroomBlackboard.attackPause);
            transform.GetChild(1).GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
    }

    public void PlayAttack1Sound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.MushroomAttack1);
    }

    public void PlayAttack2Sound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.MushroomAttack2);
    }

    public void PlayAttack3Sound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.MushroomShoot);
    }

    // 受击扣除血量方法(在玩家脚本中调用)
    public void getHurt(float damage)
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.EnemyHurt);
        mushroomBlackboard.isHit = true;
        mushroomBlackboard.health -= damage;
    }

    // 设置根据目标，用于子物体通信执行
    public void SetTarget(Transform target)
    {
        mushroomBlackboard.targetTransform = target;
    }

    // 判断是否在地面，用于子物体通信执行
    private void UpdateGroundStatus(bool isGround)
    {
        mushroomBlackboard.isGround = isGround;
    }
}

// 闲置状态
public class MushroomIdleState : Istate
{
    // 等待时间计时器
    private float idleTimer;

    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    public MushroomIdleState(FSM fsm)
    {
        mushroomFSM = fsm;
        mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        mushroomBlackboard.mushroomAnimator.Play("Idle");
        idleTimer = 0;
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
        if (mushroomBlackboard.isHit == true)
        {
            mushroomFSM.SwitchState(StateType.Hurt);
        }

        if (mushroomBlackboard.targetTransform.tag == "Player")
        {
            mushroomFSM.SwitchState(StateType.Chase);
        }

        idleTimer += Time.fixedDeltaTime;
        if (idleTimer >= mushroomBlackboard.idleTime)
        {
            mushroomFSM.SwitchState(StateType.Patrol);
        }
    }
}

// 巡逻状态
public class MushroomPatrolState : Istate
{
    // 目标位置
    private Vector2 targetPosition;

    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;
    
    public MushroomPatrolState(FSM fsm)
    {
        this.mushroomFSM = fsm;
        this.mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        mushroomBlackboard.mushroomAnimator.Play("Walk");

        float offsetX = Random.Range(mushroomBlackboard.originPos.x - mushroomBlackboard.leftOffset, mushroomBlackboard.originPos.x + mushroomBlackboard.rightOffset);
        targetPosition = new Vector2(offsetX, mushroomBlackboard.targetTransform.position.y);
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
        if (mushroomBlackboard.isHit == true)
        {
            mushroomFSM.SwitchState(StateType.Hurt);
        }

        mushroomFSM.FlipToPoint(mushroomBlackboard.mushroomTransform, targetPosition);

        if (mushroomBlackboard.targetTransform.tag == "Player")
        {
            mushroomFSM.SwitchState(StateType.Chase);
        }

        if (Vector2.Distance(targetPosition, mushroomBlackboard.mushroomTransform.position) < 0.1f)
        {
            mushroomFSM.SwitchState(StateType.Idle);
        }
        else
        {
            mushroomBlackboard.mushroomTransform.position = Vector2.MoveTowards(mushroomBlackboard.mushroomTransform.position,
                                                                                targetPosition,
                                                                                mushroomBlackboard.patrolSpeed * Time.deltaTime);
        }
    }
}

// 受伤状态
public class MushroomHurtState : Istate
{
    // 获取动画播放进度
    private AnimatorStateInfo info;

    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    public MushroomHurtState(FSM fsm)
    {
        this.mushroomFSM = fsm;
        this.mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        mushroomBlackboard.mushroomAnimator.Play("Hurt");
        GameObject blood = ObjectPool.Instance.Get(mushroomBlackboard.blood);
        blood.transform.position = mushroomBlackboard.mushroomTransform.position;
    }

    public void OnExit()
    {
        mushroomBlackboard.isHit = false;
        mushroomBlackboard.rb.velocity = new Vector2(0, mushroomBlackboard.rb.velocity.y);
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        info = mushroomBlackboard.mushroomAnimator.GetCurrentAnimatorStateInfo(0);
        if (mushroomBlackboard.health <= 0f)
        {
            mushroomFSM.SwitchState(StateType.Dead);
        }
        else if(info.normalizedTime > 0.99f)
        {
            // 受伤后直接索敌
            mushroomBlackboard.targetTransform = GameObject.FindWithTag("Player").transform;
            mushroomFSM.SwitchState(StateType.Chase);
        }
        else if(info.normalizedTime <= 0.99f)
        {
            // 受伤时造成位移
            mushroomBlackboard.rb.velocity = new Vector2(mushroomBlackboard.hurtOffset * mushroomBlackboard.targetTransform.localScale.x, mushroomBlackboard.rb.velocity.y);
            if (mushroomBlackboard.isHit == true)
            {
                mushroomFSM.SwitchState(StateType.Hurt);
            }
        }
    }
}

// 死亡状态
public class MushroomDeadState : Istate
{
    public float deadTimer;

    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    public MushroomDeadState(FSM fsm)
    {
        this.mushroomFSM = fsm;
        this.mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        deadTimer = 0;
        mushroomBlackboard.mushroomAnimator.Play("Dead");

        GameObject dropItem = GameObject.Instantiate(mushroomBlackboard.dropItemPre, mushroomBlackboard.mushroomTransform.position, Quaternion.identity);
        for (int i = 0; i < Random.Range(mushroomBlackboard.min, mushroomBlackboard.max); i++)
        {
            GameObject.Instantiate(mushroomBlackboard.coin, mushroomBlackboard.mushroomTransform.position, Quaternion.identity);
        }

        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.MushroomDead);

        foreach (Mission mission in GameManager.Instance.missionList)
        {
            if (((mission.missionType == Mission.MissionType.KillEnemy) || (mission.missionType == Mission.MissionType.KillMushroom))
                && mission.missionStatus == Mission.MissionStatus.Accepted)
            { // 有击杀敌人或击杀蘑菇人，且未完成的任务
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
        
    }

    public void OnUpdate()
    {
        deadTimer += Time.deltaTime;
        if (deadTimer >= mushroomBlackboard.deadTime)
        {
            GameObject.Destroy(mushroomBlackboard.mushroomTransform.gameObject);
        }
    }
}

// 追击状态
public class MushroomChaseState : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;
    private Transform targetPosition; // 目标位置

    public MushroomChaseState(FSM fsm)
    {
        this.mushroomFSM = fsm;
        this.mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        // 出招决策确认
        mushroomBlackboard.chooseAttackSkill = Random.Range(0, 6);

        // 攻击目标位置
        targetPosition = mushroomBlackboard.targetTransform;
        
        mushroomBlackboard.mushroomAnimator.Play("Walk");
    }

    public void OnExit()
    {
        // 退出追击时重置攻击决策
        mushroomBlackboard.chooseAttackSkill = -1;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (mushroomBlackboard.isHit == true)
        {
            mushroomFSM.SwitchState(StateType.Hurt);
        }

        mushroomFSM.FlipToTransform(mushroomBlackboard.mushroomTransform, targetPosition);

        // 玩家位于上方时，停止移动等待
        if (targetPosition.transform.position.y - mushroomBlackboard.mushroomTransform.position.y > 0.8f)
        {
            mushroomBlackboard.mushroomAnimator.Play("Idle");
        }
        else
        {
            mushroomBlackboard.mushroomTransform.position = Vector2.MoveTowards(mushroomBlackboard.mushroomTransform.position,
                                                                targetPosition.position,
                                                                mushroomBlackboard.patrolSpeed * Time.deltaTime);

            mushroomBlackboard.mushroomAnimator.Play("Walk");
        }

        if (Vector2.Distance(mushroomBlackboard.mushroomTransform.position, mushroomBlackboard.targetTransform.position) > 5f)
        { // 丢失目标
            mushroomBlackboard.targetTransform = mushroomBlackboard.mushroomTransform;
            mushroomFSM.SwitchState(StateType.Idle);
        }

        // 攻击逻辑判断
        if (mushroomBlackboard.chooseAttackSkill < 3)
        { // 使用攻击1
            mushroomBlackboard.mushroomTransform.GetChild(1).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 1;
            mushroomBlackboard.attackPause = 4;
            if (Physics2D.OverlapCircle(mushroomBlackboard.attackPoint.position, mushroomBlackboard.radius1, mushroomBlackboard.attackLayerMask))
            {
                mushroomFSM.SwitchState(StateType.Attack1);
            }
        }
        else if (mushroomBlackboard.chooseAttackSkill < 5)
        { // 使用攻击2
            mushroomBlackboard.mushroomTransform.GetChild(1).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 3;
            mushroomBlackboard.attackPause = 8;
            if (Physics2D.OverlapCircle(mushroomBlackboard.attackPoint.position, mushroomBlackboard.radius2, mushroomBlackboard.attackLayerMask))
            {
                mushroomFSM.SwitchState(StateType.Attack2);
            }
        }
        else if (mushroomBlackboard.chooseAttackSkill == 5)
        { // 使用攻击3
            mushroomBlackboard.mushroomTransform.GetChild(1).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 1;
            mushroomBlackboard.attackPause = 4;
            if (Physics2D.OverlapCircle(mushroomBlackboard.mushroomTransform.position, mushroomBlackboard.radius3, mushroomBlackboard.attackLayerMask))
            {
                mushroomFSM.SwitchState(StateType.Attack3);
            }
        }
    }
}

// 攻击1状态
public class MushroomAttack1State : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info; // 获取动画播放进度

    public MushroomAttack1State(FSM fsm)
    {
        this.mushroomFSM = fsm;
        this.mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        // 获取目标位置
        targetPosition = mushroomBlackboard.targetTransform.position;
        // 设置敌人攻击方向
        mushroomFSM.FlipToPoint(mushroomBlackboard.mushroomTransform, targetPosition);

        mushroomBlackboard.mushroomAnimator.Play("Attack1");

        mushroomBlackboard.attack = 15f;
    }

    public void OnExit()
    {
        // targetPosition = new Vector2(0, 0);
        // 重置因动画帧事件造成的攻击位移速度
        mushroomBlackboard.rb.velocity = new Vector2(0, mushroomBlackboard.rb.velocity.y);

        mushroomBlackboard.attack = 0f;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (mushroomBlackboard.isHit == true)
        {
            mushroomFSM.SwitchState(StateType.Hurt);
        }

        // 实时更新动画播放进度
        info = mushroomBlackboard.mushroomAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            mushroomFSM.SwitchState(StateType.Cool);
        }
    }
}

// 攻击2状态
public class MushroomAttack2State : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info; // 获取动画播放进度

    public MushroomAttack2State(FSM fsm)
    {
        this.mushroomFSM = fsm;
        this.mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        targetPosition = mushroomBlackboard.targetTransform.position;

        mushroomFSM.FlipToPoint(mushroomBlackboard.mushroomTransform, targetPosition);

        mushroomBlackboard.mushroomAnimator.Play("Attack2");

        mushroomBlackboard.attack = 25f;
    }

    public void OnExit()
    {
        mushroomBlackboard.rb.velocity = new Vector2(0, mushroomBlackboard.rb.velocity.y);

        mushroomBlackboard.attack = 0f;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {

        if (mushroomBlackboard.isHit == true)
        {
            mushroomFSM.SwitchState(StateType.Hurt);
        }

        // 实时更新动画播放进度
        info = mushroomBlackboard.mushroomAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            mushroomFSM.SwitchState(StateType.Cool);
        }
    }
}

// 攻击3状态
public class MushroomAttack3State : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info; // 获取动画播放进度

    private GameObject bullet1;
    private GameObject bullet2;
    private GameObject bullet3;

    private bool canShoot;

    public MushroomAttack3State(FSM fsm)
    {
        this.mushroomFSM = fsm;
        this.mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        targetPosition = mushroomBlackboard.targetTransform.position;
        mushroomBlackboard.mushroomAnimator.Play("Attack3");
        canShoot = true;
    }

    public void OnExit()
    {
        mushroomBlackboard.rb.velocity = new Vector2(0, mushroomBlackboard.rb.velocity.y);
        bullet1 = null;
        bullet2 = null;
        bullet3 = null;
        canShoot = true;
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (mushroomBlackboard.isHit == true)
        {
            mushroomFSM.SwitchState(StateType.Hurt);
        }

        mushroomFSM.FlipToPoint(mushroomBlackboard.mushroomTransform, targetPosition);

        // 实时更新动画播放进度
        info = mushroomBlackboard.mushroomAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.6f && canShoot)
        {
            bullet1 = ObjectPool.Instance.Get(mushroomBlackboard.bulletPre);
            bullet2 = ObjectPool.Instance.Get(mushroomBlackboard.bulletPre);
            bullet3 = ObjectPool.Instance.Get(mushroomBlackboard.bulletPre);

            bullet1.transform.position = mushroomBlackboard.mushroomTransform.position;
            bullet2.transform.position = mushroomBlackboard.mushroomTransform.position;
            bullet3.transform.position = mushroomBlackboard.mushroomTransform.position;

            bullet1.GetComponent<PoisonBall>().SetSpeed(new Vector2(1, 1));
            bullet2.GetComponent<PoisonBall>().SetSpeed(new Vector2(0, 1));
            bullet3.GetComponent<PoisonBall>().SetSpeed(new Vector2(-1, 1));

            canShoot = false;
        }

        if (info.normalizedTime >= 0.95f)
        {
            mushroomFSM.SwitchState(StateType.Chase);
        }
    }
}

// 攻击冷却状态
public class MushroomCoolState : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;
    private float coolTimer;

    public MushroomCoolState(FSM fsm)
    {
        mushroomFSM = fsm;
        mushroomBlackboard = fsm.blackboard as MushroomBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        coolTimer = 0;
        mushroomBlackboard.mushroomAnimator.Play("Idle");
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
        if (mushroomBlackboard.isHit == true)
        {
            mushroomFSM.SwitchState(StateType.Hurt);
        }

        if (Vector2.Distance(mushroomBlackboard.targetTransform.position, mushroomBlackboard.mushroomTransform.position) < 1f)
        {
            if (coolTimer >= mushroomBlackboard.coolTime)
            { // 结束等待
                if (mushroomBlackboard.targetTransform.tag == "Player")
                { // 若有攻击目标，追击
                    mushroomFSM.SwitchState(StateType.Chase);
                }
                else
                { // 若无攻击目标，闲置
                    mushroomFSM.SwitchState(StateType.Idle);
                }
            }
        }
        else
        {
            mushroomFSM.SwitchState(StateType.Chase);
        }
    }
}
