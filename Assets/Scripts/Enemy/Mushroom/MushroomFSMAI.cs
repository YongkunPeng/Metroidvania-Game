using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Cinemachine;

[Serializable]
public class MushroomBlackboard : Blackboard
{
    // ��ȡ�������
    public Rigidbody2D rb;
    // ��ȡ���󶯻�������
    public Animator mushroomAnimator;
    // ʹ״̬���ܵ��ö����transform
    public Transform mushroomTransform;
    // ���Ŀ������transform
    public Transform targetTransform;

    [Header("���������١��������ʱ��")]
    public float idleTime; // ԭ�ش���ʱ��
    public float deadTime; // ��������ʱ��
    public float coolTime; // �������ʱ��

    [Header("�ƶ��ٶ�")]
    public float patrolSpeed;

    [Header("��������ⷶΧ")]
    public LayerMask attackLayerMask;
    public Transform attackPoint;
    public int chooseAttackSkill;
    public float radius1;
    public float radius2;
    public float radius3;

    [Header("״̬�ж�")]
    public bool isHit; // �Ƿ��ܻ�
    public bool isGround; // �Ƿ��ŵ�

    [Header("�������")]
    public float attack1Offset;
    public float attack2Offset;
    public float attack3Offset;
    public int attackPause;

    [Header("����λ��")]
    public float hurtOffset;

    [Header("����Ԥ����")]
    public GameObject bulletPre; // �䵯
    public GameObject blood; // ѪҺ����
    public GameObject dropItemPre; // ������
    public GameObject coin; // ���

    [Header("������(���ݹ������ͱ任)")]
    public float attack;

    [Header("��ҵ�������Χ")]
    public int min;
    public int max;

    [Header("�ƶ���Χ(ԭλ�üӼ�)")]
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

    // ����λ��֡�¼�
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

    // �ܻ��۳�Ѫ������(����ҽű��е���)
    public void getHurt(float damage)
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.EnemyHurt);
        mushroomBlackboard.isHit = true;
        mushroomBlackboard.health -= damage;
    }

    // ���ø���Ŀ�꣬����������ͨ��ִ��
    public void SetTarget(Transform target)
    {
        mushroomBlackboard.targetTransform = target;
    }

    // �ж��Ƿ��ڵ��棬����������ͨ��ִ��
    private void UpdateGroundStatus(bool isGround)
    {
        mushroomBlackboard.isGround = isGround;
    }
}

// ����״̬
public class MushroomIdleState : Istate
{
    // �ȴ�ʱ���ʱ��
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

// Ѳ��״̬
public class MushroomPatrolState : Istate
{
    // Ŀ��λ��
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

// ����״̬
public class MushroomHurtState : Istate
{
    // ��ȡ�������Ž���
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
            // ���˺�ֱ������
            mushroomBlackboard.targetTransform = GameObject.FindWithTag("Player").transform;
            mushroomFSM.SwitchState(StateType.Chase);
        }
        else if(info.normalizedTime <= 0.99f)
        {
            // ����ʱ���λ��
            mushroomBlackboard.rb.velocity = new Vector2(mushroomBlackboard.hurtOffset * mushroomBlackboard.targetTransform.localScale.x, mushroomBlackboard.rb.velocity.y);
            if (mushroomBlackboard.isHit == true)
            {
                mushroomFSM.SwitchState(StateType.Hurt);
            }
        }
    }
}

// ����״̬
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
            { // �л�ɱ���˻��ɱĢ���ˣ���δ��ɵ�����
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

// ׷��״̬
public class MushroomChaseState : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;
    private Transform targetPosition; // Ŀ��λ��

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
        // ���о���ȷ��
        mushroomBlackboard.chooseAttackSkill = Random.Range(0, 6);

        // ����Ŀ��λ��
        targetPosition = mushroomBlackboard.targetTransform;
        
        mushroomBlackboard.mushroomAnimator.Play("Walk");
    }

    public void OnExit()
    {
        // �˳�׷��ʱ���ù�������
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

        // ���λ���Ϸ�ʱ��ֹͣ�ƶ��ȴ�
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
        { // ��ʧĿ��
            mushroomBlackboard.targetTransform = mushroomBlackboard.mushroomTransform;
            mushroomFSM.SwitchState(StateType.Idle);
        }

        // �����߼��ж�
        if (mushroomBlackboard.chooseAttackSkill < 3)
        { // ʹ�ù���1
            mushroomBlackboard.mushroomTransform.GetChild(1).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 1;
            mushroomBlackboard.attackPause = 4;
            if (Physics2D.OverlapCircle(mushroomBlackboard.attackPoint.position, mushroomBlackboard.radius1, mushroomBlackboard.attackLayerMask))
            {
                mushroomFSM.SwitchState(StateType.Attack1);
            }
        }
        else if (mushroomBlackboard.chooseAttackSkill < 5)
        { // ʹ�ù���2
            mushroomBlackboard.mushroomTransform.GetChild(1).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 3;
            mushroomBlackboard.attackPause = 8;
            if (Physics2D.OverlapCircle(mushroomBlackboard.attackPoint.position, mushroomBlackboard.radius2, mushroomBlackboard.attackLayerMask))
            {
                mushroomFSM.SwitchState(StateType.Attack2);
            }
        }
        else if (mushroomBlackboard.chooseAttackSkill == 5)
        { // ʹ�ù���3
            mushroomBlackboard.mushroomTransform.GetChild(1).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 1;
            mushroomBlackboard.attackPause = 4;
            if (Physics2D.OverlapCircle(mushroomBlackboard.mushroomTransform.position, mushroomBlackboard.radius3, mushroomBlackboard.attackLayerMask))
            {
                mushroomFSM.SwitchState(StateType.Attack3);
            }
        }
    }
}

// ����1״̬
public class MushroomAttack1State : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info; // ��ȡ�������Ž���

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
        // ��ȡĿ��λ��
        targetPosition = mushroomBlackboard.targetTransform.position;
        // ���õ��˹�������
        mushroomFSM.FlipToPoint(mushroomBlackboard.mushroomTransform, targetPosition);

        mushroomBlackboard.mushroomAnimator.Play("Attack1");

        mushroomBlackboard.attack = 10f;
    }

    public void OnExit()
    {
        // targetPosition = new Vector2(0, 0);
        // �����򶯻�֡�¼���ɵĹ���λ���ٶ�
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

        // ʵʱ���¶������Ž���
        info = mushroomBlackboard.mushroomAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            mushroomFSM.SwitchState(StateType.Cool);
        }
    }
}

// ����2״̬
public class MushroomAttack2State : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info; // ��ȡ�������Ž���

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

        mushroomBlackboard.attack = 20f;
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

        // ʵʱ���¶������Ž���
        info = mushroomBlackboard.mushroomAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            mushroomFSM.SwitchState(StateType.Cool);
        }
    }
}

// ����3״̬
public class MushroomAttack3State : Istate
{
    private FSM mushroomFSM;
    private MushroomBlackboard mushroomBlackboard;

    private Vector2 targetPosition;
    private AnimatorStateInfo info; // ��ȡ�������Ž���

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

        // ʵʱ���¶������Ž���
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

// ������ȴ״̬
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
            { // �����ȴ�
                if (mushroomBlackboard.targetTransform.tag == "Player")
                { // ���й���Ŀ�꣬׷��
                    mushroomFSM.SwitchState(StateType.Chase);
                }
                else
                { // ���޹���Ŀ�꣬����
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
