using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

[Serializable]
public class PlayerBlackboard : Blackboard
{
    [Header("���")]
    // ��ȡ�������
    public Rigidbody2D rb;
    // ��ȡ���󶯻�������
    public Animator playerAnimator;
    // ʹ״̬���ܵ��ö����transform
    public Transform playerTransform;

    [Header("�����ж�")]
    public bool isHit = false; // �Ƿ��ܻ�
    public bool isGround; // �Ƿ��ڵ���
    public bool isClimb; // �Ƿ���ǽ
    public bool isLightAttack; // �Ƿ�׷���ṥ��
    public bool isHeavyAttack; // �Ƿ�׷���ع���
    public bool light; // ��ǰ�����ṥ��״̬
    public bool heavy; // ��ǰ�����ع���״̬
    public int lightAttackPause;
    public int heavyAttackPause;
    public bool isRun; // �Ƿ���
    public bool isDash; // �Ƿ���Գ��

    [Header("λ��")]
    // ��Ծ����
    public float fallMultiplier;
    public float lowJumpMultiplier;
    // ����λ��
    public float lightAttackOffset;
    public float heavtAttackOffset;
    // ����λ��
    public float hurtOffset;
    // ���
    public float dashTime; // ���ʱ��
    public float dashTimeLeft; // ʣ��ʱ��
    public float lastDash; // �ϴγ�̵�ʱ��
    public float dashCool; // �����ȴ
    public float dashSpeed; // ����ٶ�
    public float dashCoolCurr; // ��ȴʣ��ʱ�䣬����UIͨ��;
    // ��ǽ
    public float climbSpeed; // �»��ٶ�

    [Header("����")]
    public float intervalTime = 0.5f; // ׷�ӹ�����������ʱ��
    public float attack = 0f; // ��Ҫ������˵Ĺ���
    public PhysicsMaterial2D noFriction; // ��Ħ�����ʣ�������ǽʱʹ��
    public int goldCoinCnt; // �������
    public int arrowCnt; // ��ʸ����
    public string playerID; // ����������ڴ洢�ļ�����

    [Header("����Ԥ����")]
    public GameObject shadow;
    public GameObject arrow;
}

public class PlayerControll : MonoBehaviour
{
    private FSM playerFSM;
    public PlayerBlackboard playerBlackboard = new PlayerBlackboard();
    public SpriteRenderer sprite;

    public LayerMask feetLayerMask; // �Ӵ������ж�����

    private void Awake()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
        // Ϊ��Һڰ帳�����Ͷ���������
        playerBlackboard.rb = GetComponent<Rigidbody2D>();
        playerBlackboard.playerAnimator = GetComponent<Animator>();
        playerBlackboard.playerTransform = this.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerBlackboard.playerID = GameManager.Instance.userData.username;
        playerBlackboard.health = GameManager.Instance.userData.health;
        playerBlackboard.goldCoinCnt = GameManager.Instance.userData.coinCnt;

        // ���������������
        playerBlackboard.baseLightAttack = 5;
        playerBlackboard.baseHeavyAttack = 10;
        playerBlackboard.speed = 3;
        playerBlackboard.jumpSpeed = 6;
        playerBlackboard.lightAttackOffset = 0.3f;
        playerBlackboard.heavtAttackOffset = 0.5f;
        playerBlackboard.lastDash = -3f;

        playerFSM = new FSM(playerBlackboard);
        
        // ���״̬
        playerFSM.AddState(StateType.Idle, new PlayerIdleState(playerFSM));
        playerFSM.AddState(StateType.Run, new PlayerRunState(playerFSM));
        playerFSM.AddState(StateType.Jump, new PlayerJumpState(playerFSM));
        playerFSM.AddState(StateType.Fall, new PlayerFallState(playerFSM));
        playerFSM.AddState(StateType.Falling, new PlayerFallingState(playerFSM));
        playerFSM.AddState(StateType.Land, new PlayerLandState(playerFSM));
        playerFSM.AddState(StateType.Dash, new PlayerDashState(playerFSM));
        playerFSM.AddState(StateType.Climb, new PlayerClimbState(playerFSM));
        playerFSM.AddState(StateType.ClimbJump, new PlayerClimbJumpState(playerFSM));
        
        playerFSM.AddState(StateType.LightAttack1, new PlayerLightAttack1State(playerFSM));
        playerFSM.AddState(StateType.LightAttack2, new PlayerLightAttack2State(playerFSM));
        playerFSM.AddState(StateType.LightAttack3, new PlayerLightAttack3State(playerFSM));
        playerFSM.AddState(StateType.HeavyAttack1, new PlayerHeavyAttack1State(playerFSM));
        playerFSM.AddState(StateType.HeavyAttack2, new PlayerHeavyAttack2State(playerFSM));
        playerFSM.AddState(StateType.HeavyAttack3, new PlayerHeavyAttack3State(playerFSM));
        playerFSM.AddState(StateType.Shoot, new PlayerShootState(playerFSM));

        playerFSM.AddState(StateType.Hurt, new PlayerHurtState(playerFSM));

        // �����ʼ״̬
        playerFSM.SwitchState(StateType.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateChangeablePlayerData();

        CanDash();
        playerFSM.OnCheck();
        playerFSM.OnUpdate();
    }

    private void FixedUpdate()
    {
        playerFSM.OnFixUpdate();
    }

    /// <summary>
    /// ����GameManager��userdata���������Ӧ��ʸ���������
    /// </summary>
    public void UpdateChangeablePlayerData()
    {
        if (GameManager.Instance.userData.itemsDict.ContainsKey(ItemsConst.Arrow))
        {
            playerBlackboard.arrowCnt = GameManager.Instance.userData.itemsDict[ItemsConst.Arrow];
        }
        else if (!GameManager.Instance.userData.itemsDict.ContainsKey(ItemsConst.Arrow))
        {
            playerBlackboard.arrowCnt = 0;
        }
    }

    // ����Ƿ�ӵ�(ͨ���㲥�ķ�ʽ��ȡ������isGround��Ϣ)
    private void UpdateGroundStatus(bool isGround)
    {
        playerBlackboard.isGround = isGround;
    }

    // ����Ƿ�Ӵ�ǽ��(ͨ���㲥�ķ�ʽ��ȡ������isClimb��Ϣ)
    private void UpdateClimbStatus(bool isClimb)
    {
        playerBlackboard.isClimb = isClimb;
    }

    // ����ͼ�㷽��۲�
    private void OnDrawGizmosSelected()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int currAttackPause = 0;
        MushroomFSMAI mushroom = collision.gameObject.GetComponent<MushroomFSMAI>();
        GoblinFSMAI goblin = collision.gameObject.GetComponent<GoblinFSMAI>();

        if (playerBlackboard.light)
        {
            currAttackPause = playerBlackboard.lightAttackPause;
            transform.GetChild(1).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 0.2f;
        }
        else if(playerBlackboard.heavy)
        {
            currAttackPause = playerBlackboard.heavyAttackPause;
            transform.GetChild(1).GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 0.5f;
        }

        if (mushroom != null)
        {
            transform.GetChild(1).GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            GameManager.Instance.HitPause(currAttackPause);
            mushroom.getHurt(playerBlackboard.attack);
        }
        if (goblin != null)
        {
            transform.GetChild(1).GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            GameManager.Instance.HitPause(currAttackPause);
            goblin.getHurt(playerBlackboard.attack);
        }
    }

    public void getHurt(float damage, Vector3 damagePos)
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerHurt);
        playerBlackboard.isHit = true;
        StartCoroutine(CanBeHurt());

        if (playerBlackboard.health - damage >= 0f)
        {
            playerBlackboard.health -= damage;
        }
        else
        {
            playerBlackboard.health = 0f;
        }
        
        if (damagePos.x > transform.position.x)
        {
            playerBlackboard.rb.velocity = new Vector2(-playerBlackboard.hurtOffset, 2f);
        }
        else
        {
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.hurtOffset, 2f);
        }
    }

    public void CanDash()
    {
        playerBlackboard.dashCoolCurr = playerBlackboard.lastDash + playerBlackboard.dashCool - Time.time;
        if (Time.time >= (playerBlackboard.lastDash + playerBlackboard.dashCool))
        {
            playerBlackboard.isDash = true;
            playerBlackboard.dashTimeLeft = playerBlackboard.dashTime;
        }
    }

    // UI��ȡ����ֵ����Ϣ
    public void GetPlayerInfo(ref float life, ref float coolTimeCurr, ref int goldCoinCnt, ref int arrowCnt)
    {
        life = playerBlackboard.health;
        coolTimeCurr = playerBlackboard.dashCoolCurr;
        goldCoinCnt = playerBlackboard.goldCoinCnt;
        arrowCnt = playerBlackboard.arrowCnt;
    }

    // ��ȡ������������
    public void GetCoinData(ref int goldCoinCnt)
    {
        goldCoinCnt = playerBlackboard.goldCoinCnt;
    }

    // ��ȡ��ҽ��
    public void GetLifeCoinData(ref float life, ref int goldCoinCnt)
    {
        life = playerBlackboard.health;
        goldCoinCnt = playerBlackboard.goldCoinCnt;
    }

    // �ع������ţ�֡�¼�
    public void PlayHeavyAttackSound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerHeavyAttack);
    }

    // ����/���ٽ�ң����ж��ܷ�������
    public bool ChangeCoinCnt(int num)
    {
        if (playerBlackboard.goldCoinCnt + num < 9999 && playerBlackboard.goldCoinCnt + num >= 0)
        {
            playerBlackboard.goldCoinCnt += num;
            return true;
        }
        else if (playerBlackboard.goldCoinCnt + num >= 9999)
        {
            playerBlackboard.goldCoinCnt = 9999;
            return true;
        }
        else
        { // ���ٽ�����������н��
            return false;
        }
    }

    // �ظ�����
    public void HealLife(float num)
    {
        if (playerBlackboard.health + num < 100f)
        {
            playerBlackboard.health += num;
        }
        else
        {
            playerBlackboard.health = 100f;
        }
    }

    IEnumerator CanBeHurt()
    {
        float startTime = Time.time;
        float endTime = startTime;
        playerBlackboard.playerTransform.gameObject.layer = 12;
        while (endTime - startTime <= 1f)
        {
            endTime = Time.time;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = !sprite.enabled;
        }
        sprite.enabled = true;
        playerBlackboard.playerTransform.gameObject.layer = 7;
    }
}

// ����״̬
public class PlayerIdleState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private float horizontal_x;

    public PlayerIdleState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        playerBlackboard.playerAnimator.Play("Idle");
    }

    public void OnExit()
    {
        
    }

    public void OnFixUpdate()
    {
        // �����ƶ�ʱ�л�����״̬
        horizontal_x = Input.GetAxisRaw("Horizontal");
        if (horizontal_x != 0)
        {
            playerFSM.SwitchState(StateType.Run);
        }

        // �ŵ����ٶ���ֱ����Ϊ��ʱ����������״̬
        if ((playerBlackboard.isGround == false) && (playerBlackboard.rb.velocity.y < 0))
        {
            playerFSM.SwitchState(StateType.Fall);
        }
    }

    public void OnUpdate()
    {
        // ����ʱ�л�������״̬
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // ��Ҫ�����⣬����Update��
        if ((playerBlackboard.isGround == true) && (Input.GetButtonDown("Jump")))
        {
            playerFSM.SwitchState(StateType.Jump);
        }

        // ���¹����������ṥ��״̬
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // ���¹����������ع���״̬
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // ���³�̼�������״̬
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        // ����������������
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// �ƶ�״̬
public class PlayerRunState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private float horizontal_x;

    public PlayerRunState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        playerBlackboard.playerAnimator.Play("Run");
        AudioSourceManager.Instance.PlayPlayerWalkSound();
    }

    public void OnExit()
    {
        AudioSourceManager.Instance.StopPlayerWalkSound();
    }

    public void OnFixUpdate()
    {
        playerBlackboard.rb.velocity = new Vector2(horizontal_x * playerBlackboard.speed, playerBlackboard.rb.velocity.y); // ����ƶ�

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        if (Mathf.Abs(horizontal_x) < 0.1f) // �����ƶ������л�Ϊ�ܶ�״̬
        {
            playerFSM.SwitchState(StateType.Idle);
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // ����ʱ�л�������״̬
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // �л�Ϊ��Ծ״̬
        if ((playerBlackboard.isGround == true) && (Input.GetButtonDown("Jump")))
        {
            playerFSM.SwitchState(StateType.Jump);
        }

        // ���¹����������ṥ��״̬
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // ���¹����������ع���״̬
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // ���³�̼�������״̬
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        if (playerBlackboard.isClimb && horizontal_x != 0f && !playerBlackboard.isGround)
        {
            playerFSM.SwitchState(StateType.Climb);
        }

        // ����������������
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }

        // �ŵ����ٶ���ֱ����Ϊ��ʱ����������״̬
        if ((playerBlackboard.isGround == false) && (playerBlackboard.rb.velocity.y < 0))
        {
            playerFSM.SwitchState(StateType.Fall);
        }
    }
}

// ��Ծ״̬
public class PlayerJumpState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private float horizontal_x;

    public PlayerJumpState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        // ��Ծ
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerJump);
        playerBlackboard.playerAnimator.Play("Jump");
        playerBlackboard.rb.velocity = new Vector2(0, playerBlackboard.jumpSpeed);   
    }

    public void OnExit()
    {
        
    }

    public void OnFixUpdate()
    {
        playerBlackboard.rb.velocity = new Vector2(horizontal_x * playerBlackboard.speed, playerBlackboard.rb.velocity.y);

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // �ŵ����ٶ���ֱ����Ϊ��ʱ����������״̬
        if ((playerBlackboard.isGround == false) && (playerBlackboard.rb.velocity.y < 0))
        {
            playerFSM.SwitchState(StateType.Fall);
        }
    }

    public void OnUpdate()
    {
        // �����Ծ���̵�ˮƽ�ƶ�
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // ���¹����������ṥ��״̬
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // ���¹����������ع���״̬
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // ���³�̼�������״̬
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }
        
        if (playerBlackboard.rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        { // ��Ծ�ڼ��ɿ���Ծ��ʩ�����µ��ٶ�
            playerBlackboard.rb.velocity += Vector2.up * Physics2D.gravity.y * (playerBlackboard.lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (playerBlackboard.isClimb == true && horizontal_x != 0f)
        { // ��ˮƽ�����ҽ���ǽ
            playerFSM.SwitchState(StateType.Climb);
        }

        // ����ʱ�л�������״̬
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // ����������������
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// ����״̬
public class PlayerFallState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private float horizontal_x;
    private AnimatorStateInfo info;

    public PlayerFallState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        playerBlackboard.playerAnimator.Play("Fall");
    }

    public void OnExit()
    {
        
    }

    public void OnFixUpdate()
    {
        playerBlackboard.rb.velocity = new Vector2(horizontal_x * playerBlackboard.speed, playerBlackboard.rb.velocity.y);

        // ��ȡ��ǰ�����Ĳ��Ž���
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.99f)
        {
            playerFSM.SwitchState(StateType.Falling);
        }

        // ���ʱ�л�Ϊ���״̬
        if (playerBlackboard.isGround == true)
        {
            playerFSM.SwitchState(StateType.Land);
        }
    }

    public void OnUpdate()
    {
        // ���������̵�ˮƽ�ƶ�
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // ���¹����������ṥ��״̬
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // ���¹����������ع���״̬
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // ���³�̼�������״̬
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        // ��ˮƽ�����ҽ���ǽ
        if (playerBlackboard.isClimb == true && horizontal_x != 0f)
        {
            playerFSM.SwitchState(StateType.Climb);
        }

        // ����ʱ�л�������״̬
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // ����������������
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// �������״̬
public class PlayerFallingState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private float horizontal_x;

    public PlayerFallingState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        playerBlackboard.playerAnimator.Play("Falling");
    }

    public void OnExit()
    {
        
    }

    public void OnFixUpdate()
    {
        playerBlackboard.rb.velocity = new Vector2(horizontal_x * playerBlackboard.speed, playerBlackboard.rb.velocity.y);

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // ���ʱ�л�Ϊ���״̬
        if (playerBlackboard.isGround == true)
        {
            playerFSM.SwitchState(StateType.Land);
        }
    }

    public void OnUpdate()
    {
        // ���������̵�ˮƽ�ƶ�
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // ���¹����������ṥ��״̬
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // ���¹����������ع���״̬
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // ���³�̼�������״̬
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        // ����ʱ���𽥼Ӵ������ٶ�
        if (playerBlackboard.rb.velocity.y < 0)
        {
            playerBlackboard.rb.velocity += Vector2.up * Physics2D.gravity.y * (playerBlackboard.fallMultiplier - 1) * Time.deltaTime;
        }

        // ��ˮƽ�����ҽ���ǽ
        if (playerBlackboard.isClimb == true && horizontal_x != 0f)
        {
            playerFSM.SwitchState(StateType.Climb);
        }

        // ����ʱ�л�������״̬
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // ����������������
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// �ŵ�״̬
public class PlayerLandState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;

    public PlayerLandState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerLand);
        // ���ʱ������ض���������������ٶ�ʹ֮Ϊ0
        playerBlackboard.rb.velocity = Vector2.zero;
        playerBlackboard.playerAnimator.Play("Land");
    }

    public void OnExit()
    {
        
    }

    public void OnFixUpdate()
    {
        // ����������Ͻ�������״̬
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.99f)
        {
            playerFSM.SwitchState(StateType.Idle);
        }
    }

    public void OnUpdate()
    {
        // ����ʱ�л�������״̬
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// ��ǽ״̬
public class PlayerClimbState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private float horizontal_x;

    public PlayerClimbState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        playerBlackboard.playerAnimator.Play("Climb");
        playerBlackboard.rb.sharedMaterial = playerBlackboard.noFriction; // �л�Ϊ��Ħ�����ʣ���ֹ��ǽ
    }

    public void OnExit()
    {
        playerBlackboard.rb.sharedMaterial = null;
    }

    public void OnFixUpdate()
    {
        // �����Ծ���̵�ˮƽ�ƶ�
        horizontal_x = Input.GetAxisRaw("Horizontal");
        playerBlackboard.rb.velocity = new Vector2(horizontal_x * playerBlackboard.speed, playerBlackboard.rb.velocity.y);

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void OnUpdate()
    {
        if (playerBlackboard.isGround)
        { // �ŵ�
            playerFSM.SwitchState(StateType.Idle);
        }

        if (!playerBlackboard.isClimb && !playerBlackboard.isGround)
        { // δ�ŵ���δ����
            playerFSM.SwitchState(StateType.Falling);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        { // ��ǽʱ������Ծ�����е�ǽ��
            playerFSM.SwitchState(StateType.ClimbJump);
        }
        else
        { // ������ǽ�������»�
            playerBlackboard.rb.velocity = new Vector2(0, -playerBlackboard.climbSpeed);
        }
        
        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// ��ǽ��״̬
public class PlayerClimbJumpState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private float horizontal_x;
    private const float horizontalInputTime = 0.2f;
    private float horizontalInputTimer; 

    public PlayerClimbJumpState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        // ��Ծ
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerJump);
        playerBlackboard.playerAnimator.Play("Jump");
        playerBlackboard.rb.velocity = new Vector2(-playerBlackboard.playerTransform.localScale.x * 4f, playerBlackboard.jumpSpeed);
        playerBlackboard.playerTransform.localScale = new Vector3(-playerBlackboard.playerTransform.localScale.x, 1, 1);

        horizontalInputTimer = 0f;
    }

    public void OnExit()
    {
        horizontalInputTimer = 0f;
    }

    public void OnFixUpdate()
    {
        if (horizontal_x != 0f && horizontalInputTime <= horizontalInputTimer)
        {
            playerBlackboard.rb.velocity = new Vector2(horizontal_x * playerBlackboard.speed, playerBlackboard.rb.velocity.y);

            // �������뷽�����ó���
            if (horizontal_x > 0)
            {
                playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
            }
            else if (horizontal_x < 0)
            {
                playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
            }

        }

        // �ŵ����ٶ���ֱ����Ϊ��ʱ����������״̬
        if ((playerBlackboard.isGround == false) && (playerBlackboard.rb.velocity.y < 0))
        {
            playerFSM.SwitchState(StateType.Fall);
        }
    }

    public void OnUpdate()
    {
        horizontalInputTimer += Time.deltaTime;

        // �����Ծ���̵�ˮƽ�ƶ�
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // ���¹����������ṥ��״̬
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // ���¹����������ع���״̬
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // ���³�̼�������״̬
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        if (playerBlackboard.isClimb == true && horizontal_x != 0f)
        { // ��ˮƽ�����ҽ���ǽ
            playerFSM.SwitchState(StateType.Climb);
        }

        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }

        // ����������������
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// ���״̬
public class PlayerDashState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;

    public PlayerDashState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerDash);
        playerBlackboard.lastDash = Time.time; // ��¼����̵�ʱ��
        playerBlackboard.playerAnimator.Play("Dash");
    }

    public void OnExit()
    {
        // �����ٶ�
        playerBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {
        if (playerBlackboard.dashTimeLeft > 0)
        { // ���ܳ��
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.dashSpeed * playerBlackboard.playerTransform.localScale.x, playerBlackboard.rb.velocity.y);
            playerBlackboard.dashTimeLeft -= Time.deltaTime;
            ObjectPool.Instance.Get(playerBlackboard.shadow);
        }
        else if (playerBlackboard.dashTimeLeft <= 0)
        {
            playerBlackboard.isDash = false;
            playerFSM.SwitchState(StateType.Idle);
        }
    }

    public void OnUpdate()
    {

    }
}

// �ṥ��״̬1
public class PlayerLightAttack1State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // ��¼�ս����״̬��ʱ��
    private float currTime; // ��¼�����ṥ�����ĵ�ǰʱ��

    private float horizontal_x;

    public PlayerLightAttack1State(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerLightAttack);
        playerBlackboard.light = true;
        playerBlackboard.attack = playerBlackboard.baseLightAttack; // ��ʱ������Ϊ�����ṥ����
        playerBlackboard.playerAnimator.Play("LightAttack1");
        lastTime = Time.time;
    }

    public void OnExit()
    {
        playerBlackboard.attack = 0f;
        playerBlackboard.light = false;
        playerBlackboard.isLightAttack = false;
    }

    public void OnFixUpdate()
    {
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);
        
        if (playerBlackboard.isGround == true)
        { // ֻ�ڵ���ʱ�����ӹ���λ��
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.lightAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            if (playerBlackboard.isLightAttack == false)
            { // δ�����ṥ����
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isLightAttack == true)
            { // ���¹�������׷�ӹ���
                playerFSM.SwitchState(StateType.LightAttack2);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // �ṥ��1�������ٴΰ����ṥ�������ж�Ϊ׷�ӹ���
        if ((Input.GetKeyDown(KeyCode.J)) && (!playerBlackboard.isLightAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isLightAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// �ṥ��״̬2
public class PlayerLightAttack2State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // ��¼�ս����״̬��ʱ��
    private float currTime; // ��¼�����ṥ�����ĵ�ǰʱ��

    private float horizontal_x;

    public PlayerLightAttack2State(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerLightAttack);
        playerBlackboard.light = true;
        playerBlackboard.attack = playerBlackboard.baseLightAttack; // ��ʱ������Ϊ�����ṥ����
        playerBlackboard.playerAnimator.Play("LightAttack2");
        lastTime = Time.time;
    }

    public void OnExit()
    {
        playerBlackboard.attack = 0f;
        playerBlackboard.light = false;
        playerBlackboard.isLightAttack = false;
    }

    public void OnFixUpdate()
    {
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerBlackboard.isGround == true)
        { // ֻ�ڵ���ʱ�����ӹ���λ��
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.lightAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            
            if (playerBlackboard.isLightAttack == false)
            { // δ�����ṥ����
                playerFSM.SwitchState(StateType.Idle);
            } 
            else if (playerBlackboard.isLightAttack == true)
            { // ���¹�������׷�ӹ���
                playerFSM.SwitchState(StateType.LightAttack3);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // �ṥ��2�������ٴΰ����ṥ�������ж�Ϊ׷�ӹ���
        if ((Input.GetKeyDown(KeyCode.J)) && (!playerBlackboard.isLightAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isLightAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// �ṥ��״̬3
public class PlayerLightAttack3State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // ��¼�ս����״̬��ʱ��
    private float currTime; // ��¼�����ṥ�����ĵ�ǰʱ��

    private float horizontal_x;

    public PlayerLightAttack3State(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerLightAttack);
        playerBlackboard.light = true;
        playerBlackboard.attack = playerBlackboard.baseLightAttack; // ��ʱ������Ϊ�����ṥ����
        playerBlackboard.playerAnimator.Play("LightAttack3");
        lastTime = Time.time;
    }

    public void OnExit()
    {
        playerBlackboard.attack = 0f;
        playerBlackboard.light = false;
        playerBlackboard.isLightAttack = false;
    }

    public void OnFixUpdate()
    {
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerBlackboard.isGround == true)
        { // ֻ�ڵ���ʱ�����ӹ���λ��
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.lightAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            
            if (playerBlackboard.isLightAttack == false)
            { // δ�����ṥ����
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isLightAttack == true)
            { // ���¹�������׷�ӹ���
                playerFSM.SwitchState(StateType.LightAttack1);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // �ṥ��3�������ٴΰ����ṥ�������ж�Ϊ׷�ӹ���
        if ((Input.GetKeyDown(KeyCode.J)) && (!playerBlackboard.isLightAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isLightAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// �ع���״̬1
public class PlayerHeavyAttack1State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // ��¼�ս����״̬��ʱ��
    private float currTime; // ��¼�����ṥ�����ĵ�ǰʱ��

    private float horizontal_x;

    public PlayerHeavyAttack1State(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        playerBlackboard.heavy = true;
        playerBlackboard.attack = playerBlackboard.baseHeavyAttack; // ��ʱ������Ϊ�����ع�����
        playerBlackboard.playerAnimator.Play("HeavyAttack1");
        lastTime = Time.time;
    }

    public void OnExit()
    {
        playerBlackboard.attack = 0f;
        playerBlackboard.heavy = false;
        playerBlackboard.isHeavyAttack = false;
    }

    public void OnFixUpdate()
    {
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerBlackboard.isGround == true)
        { // ֻ�ڵ���ʱ�����ӹ���λ��
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.heavtAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            if (playerBlackboard.isHeavyAttack == false)
            { // δ�����ṥ����
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isHeavyAttack == true)
            { // ���¹�������׷�ӹ���
                playerFSM.SwitchState(StateType.HeavyAttack2);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // �ṥ��3�������ٴΰ����ṥ�������ж�Ϊ׷�ӹ���
        if ((Input.GetKeyDown(KeyCode.K)) && (!playerBlackboard.isHeavyAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isHeavyAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// �ع���״̬2
public class PlayerHeavyAttack2State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // ��¼�ս����״̬��ʱ��
    private float currTime; // ��¼�����ṥ�����ĵ�ǰʱ��

    private float horizontal_x;

    public PlayerHeavyAttack2State(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        playerBlackboard.heavy = true;
        playerBlackboard.attack = playerBlackboard.baseHeavyAttack; // ��ʱ������Ϊ�����ع�����
        playerBlackboard.playerAnimator.Play("HeavyAttack2");
        lastTime = Time.time;
    }

    public void OnExit()
    {
        playerBlackboard.attack = 0f;
        playerBlackboard.heavy = false;
        playerBlackboard.isHeavyAttack = false;
    }

    public void OnFixUpdate()
    {
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerBlackboard.isGround == true)
        { // ֻ�ڵ���ʱ�����ӹ���λ��
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.heavtAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            if (playerBlackboard.isHeavyAttack == false)
            { // δ�����ṥ����
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isHeavyAttack == true)
            { // ���¹�������׷�ӹ���
                playerFSM.SwitchState(StateType.HeavyAttack3);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // �ṥ��3�������ٴΰ����ṥ�������ж�Ϊ׷�ӹ���
        if ((Input.GetKeyDown(KeyCode.K)) && (!playerBlackboard.isHeavyAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isHeavyAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// �ع���״̬3
public class PlayerHeavyAttack3State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // ��¼�ս����״̬��ʱ��
    private float currTime; // ��¼�����ṥ�����ĵ�ǰʱ��

    private float horizontal_x;

    public PlayerHeavyAttack3State(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {

    }

    public void OnEnter()
    {
        playerBlackboard.heavy = true;
        playerBlackboard.attack = playerBlackboard.baseHeavyAttack; // ��ʱ������Ϊ�����ع�����
        playerBlackboard.playerAnimator.Play("HeavyAttack3");
        lastTime = Time.time;
    }

    public void OnExit()
    {
        playerBlackboard.attack = 0f;
        playerBlackboard.heavy = false;
        playerBlackboard.isHeavyAttack = false;
    }

    public void OnFixUpdate()
    {
        if (playerBlackboard.isGround == true)
        { // ֻ�ڵ���ʱ�����ӹ���λ��
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.heavtAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            if (playerBlackboard.isHeavyAttack == false)
            { // δ�����ṥ����
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isHeavyAttack == true)
            { // ���¹�������׷�ӹ���
                playerFSM.SwitchState(StateType.HeavyAttack1);
            }
        }
    }

    public void OnUpdate()
    {
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // �������뷽�����ó���
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // �ṥ��3�������ٴΰ����ṥ�������ж�Ϊ׷�ӹ���
        if ((Input.GetKeyDown(KeyCode.K)) && (!playerBlackboard.isHeavyAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isHeavyAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// ���״̬
public class PlayerShootState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;

    private AnimatorStateInfo info;
    private bool isShoot; // �Ƿ������
    private GameObject arrow;

    public PlayerShootState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerShoot);
        playerBlackboard.playerAnimator.Play("Shoot");
        isShoot = true;
        playerBlackboard.rb.velocity = new Vector2(0, playerBlackboard.rb.velocity.y);
    }

    public void OnExit()
    {
        isShoot = true;
        arrow = null;
        GameManager.Instance.RemoveItem(GameManager.Instance.resourceDict[ItemsConst.Arrow]);
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        if (playerBlackboard.isHit)
        { // ����ʱ�л�������״̬
            playerFSM.SwitchState(StateType.Hurt);
        }

        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.90f && isShoot)
        {
            arrow = ObjectPool.Instance.Get(playerBlackboard.arrow);
            arrow.transform.position = playerBlackboard.playerTransform.position;
            arrow.GetComponent<Arrow>().SetSpeed(new Vector2(playerBlackboard.playerTransform.localScale.x, 0));
            isShoot = false;
        }

        if (info.normalizedTime >= 0.95f)
        {
            playerFSM.SwitchState(StateType.Idle);
        }
    }
}

// ����״̬
public class PlayerHurtState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;

    private AnimatorStateInfo info;

    public PlayerHurtState(FSM fsm)
    {
        playerFSM = fsm;
        playerBlackboard = fsm.blackboard as PlayerBlackboard;
    }

    public void OnCheck()
    {
        
    }

    public void OnEnter()
    {
        playerBlackboard.playerAnimator.Play("Hurt");
    }

    public void OnExit()
    {
        playerBlackboard.isHit = false;
        playerBlackboard.rb.velocity = new Vector2(0, playerBlackboard.rb.velocity.y);
    }

    public void OnFixUpdate()
    {
        
    }

    public void OnUpdate()
    {
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime > 0.99f)
        {
            playerFSM.SwitchState(StateType.Idle);
        }
        //else if (info.normalizedTime <= 0.99f)
        //{
        //    playerBlackboard.rb.velocity = new Vector2(-playerBlackboard.hurtOffset * playerBlackboard.playerTransform.localScale.x, 1f);
        //}
    }
}
