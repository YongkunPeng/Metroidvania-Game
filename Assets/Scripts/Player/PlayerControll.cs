using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

[Serializable]
public class PlayerBlackboard : Blackboard
{
    [Header("组件")]
    // 获取对象刚体
    public Rigidbody2D rb;
    // 获取对象动画控制器
    public Animator playerAnimator;
    // 使状态类能调用对象的transform
    public Transform playerTransform;

    [Header("条件判断")]
    public bool isHit = false; // 是否受击
    public bool isGround; // 是否在地面
    public bool isClimb; // 是否爬墙
    public bool isLightAttack; // 是否追加轻攻击
    public bool isHeavyAttack; // 是否追加重攻击
    public bool light; // 当前处于轻攻击状态
    public bool heavy; // 当前处于重攻击状态
    public int lightAttackPause;
    public int heavyAttackPause;
    public bool isRun; // 是否奔跑
    public bool isDash; // 是否可以冲刺

    [Header("位移")]
    // 跳跃参数
    public float fallMultiplier;
    public float lowJumpMultiplier;
    // 攻击位移
    public float lightAttackOffset;
    public float heavtAttackOffset;
    // 受伤位移
    public float hurtOffset;
    // 冲刺
    public float dashTime; // 冲刺时长
    public float dashTimeLeft; // 剩余时间
    public float lastDash; // 上次冲刺的时间
    public float dashCool; // 冲刺冷却
    public float dashSpeed; // 冲刺速度
    public float dashCoolCurr; // 冷却剩余时间，用于UI通信;
    // 爬墙
    public float climbSpeed; // 下滑速度

    [Header("其它")]
    public float intervalTime = 0.5f; // 追加攻击的允许间隔时间
    public float attack = 0f; // 将要给予敌人的攻击
    public PhysicsMaterial2D noFriction; // 无摩擦材质，用于爬墙时使用
    public int goldCoinCnt; // 金币数量
    public int arrowCnt; // 箭矢数量
    public string playerID; // 玩家名，用于存储文件命名

    [Header("生成预制体")]
    public GameObject shadow;
    public GameObject arrow;
}

public class PlayerControll : MonoBehaviour
{
    private FSM playerFSM;
    public PlayerBlackboard playerBlackboard = new PlayerBlackboard();
    public SpriteRenderer sprite;

    public LayerMask feetLayerMask; // 接触地面判断射线

    private void Awake()
    {
        sprite = transform.GetComponent<SpriteRenderer>();
        // 为玩家黑板赋予刚体和动画控制器
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

        // 设置玩家其他属性
        playerBlackboard.baseLightAttack = 5;
        playerBlackboard.baseHeavyAttack = 10;
        playerBlackboard.speed = 3;
        playerBlackboard.jumpSpeed = 6;
        playerBlackboard.lightAttackOffset = 0.3f;
        playerBlackboard.heavtAttackOffset = 0.5f;
        playerBlackboard.lastDash = -3f;

        playerFSM = new FSM(playerBlackboard);
        
        // 添加状态
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

        // 进入初始状态
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
    /// 根据GameManager内userdata给予玩家相应箭矢数、金币数
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

    // 玩家是否接地(通过广播的方式获取子物体isGround信息)
    private void UpdateGroundStatus(bool isGround)
    {
        playerBlackboard.isGround = isGround;
    }

    // 玩家是否接触墙壁(通过广播的方式获取子物体isClimb信息)
    private void UpdateClimbStatus(bool isClimb)
    {
        playerBlackboard.isClimb = isClimb;
    }

    // 绘制图层方便观察
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

    // UI获取生命值等信息
    public void GetPlayerInfo(ref float life, ref float coolTimeCurr, ref int goldCoinCnt, ref int arrowCnt)
    {
        life = playerBlackboard.health;
        coolTimeCurr = playerBlackboard.dashCoolCurr;
        goldCoinCnt = playerBlackboard.goldCoinCnt;
        arrowCnt = playerBlackboard.arrowCnt;
    }

    // 获取玩家生命、金币
    public void GetCoinData(ref int goldCoinCnt)
    {
        goldCoinCnt = playerBlackboard.goldCoinCnt;
    }

    // 获取玩家金币
    public void GetLifeCoinData(ref float life, ref int goldCoinCnt)
    {
        life = playerBlackboard.health;
        goldCoinCnt = playerBlackboard.goldCoinCnt;
    }

    // 重攻击播放，帧事件
    public void PlayHeavyAttackSound()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.PlayerHeavyAttack);
    }

    // 增加/减少金币，并判断能否买卖出
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
        { // 减少金币数大于现有金币
            return false;
        }
    }

    // 回复生命
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

// 闲置状态
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
        // 当有移动时切换奔跑状态
        horizontal_x = Input.GetAxisRaw("Horizontal");
        if (horizontal_x != 0)
        {
            playerFSM.SwitchState(StateType.Run);
        }

        // 着地且速度竖直方向为负时，进入下落状态
        if ((playerBlackboard.isGround == false) && (playerBlackboard.rb.velocity.y < 0))
        {
            playerFSM.SwitchState(StateType.Fall);
        }
    }

    public void OnUpdate()
    {
        // 受伤时切换到受伤状态
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // 需要间隔检测，放入Update中
        if ((playerBlackboard.isGround == true) && (Input.GetButtonDown("Jump")))
        {
            playerFSM.SwitchState(StateType.Jump);
        }

        // 按下攻击键进入轻攻击状态
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // 按下攻击键进入重攻击状态
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // 按下冲刺键进入冲刺状态
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        // 按下射击键射出弓箭
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// 移动状态
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
        playerBlackboard.rb.velocity = new Vector2(horizontal_x * playerBlackboard.speed, playerBlackboard.rb.velocity.y); // 玩家移动

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        if (Mathf.Abs(horizontal_x) < 0.1f) // 按下移动键后切换为跑动状态
        {
            playerFSM.SwitchState(StateType.Idle);
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 受伤时切换到受伤状态
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // 切换为跳跃状态
        if ((playerBlackboard.isGround == true) && (Input.GetButtonDown("Jump")))
        {
            playerFSM.SwitchState(StateType.Jump);
        }

        // 按下攻击键进入轻攻击状态
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // 按下攻击键进入重攻击状态
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // 按下冲刺键进入冲刺状态
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        if (playerBlackboard.isClimb && horizontal_x != 0f && !playerBlackboard.isGround)
        {
            playerFSM.SwitchState(StateType.Climb);
        }

        // 按下射击键射出弓箭
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }

        // 着地且速度竖直方向为负时，进入下落状态
        if ((playerBlackboard.isGround == false) && (playerBlackboard.rb.velocity.y < 0))
        {
            playerFSM.SwitchState(StateType.Fall);
        }
    }
}

// 跳跃状态
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
        // 跳跃
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

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // 着地且速度竖直方向为负时，进入下落状态
        if ((playerBlackboard.isGround == false) && (playerBlackboard.rb.velocity.y < 0))
        {
            playerFSM.SwitchState(StateType.Fall);
        }
    }

    public void OnUpdate()
    {
        // 玩家跳跃过程的水平移动
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 按下攻击键进入轻攻击状态
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // 按下攻击键进入重攻击状态
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // 按下冲刺键进入冲刺状态
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }
        
        if (playerBlackboard.rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        { // 跳跃期间松开跳跃，施加项下的速度
            playerBlackboard.rb.velocity += Vector2.up * Physics2D.gravity.y * (playerBlackboard.lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (playerBlackboard.isClimb == true && horizontal_x != 0f)
        { // 有水平输入且紧贴墙
            playerFSM.SwitchState(StateType.Climb);
        }

        // 受伤时切换到受伤状态
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // 按下射击键射出弓箭
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// 下落状态
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

        // 获取当前动画的播放进度
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.99f)
        {
            playerFSM.SwitchState(StateType.Falling);
        }

        // 落地时切换为落地状态
        if (playerBlackboard.isGround == true)
        {
            playerFSM.SwitchState(StateType.Land);
        }
    }

    public void OnUpdate()
    {
        // 玩家下落过程的水平移动
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 按下攻击键进入轻攻击状态
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // 按下攻击键进入重攻击状态
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // 按下冲刺键进入冲刺状态
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        // 有水平输入且紧贴墙
        if (playerBlackboard.isClimb == true && horizontal_x != 0f)
        {
            playerFSM.SwitchState(StateType.Climb);
        }

        // 受伤时切换到受伤状态
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // 按下射击键射出弓箭
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// 下落过程状态
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

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // 落地时切换为落地状态
        if (playerBlackboard.isGround == true)
        {
            playerFSM.SwitchState(StateType.Land);
        }
    }

    public void OnUpdate()
    {
        // 玩家下落过程的水平移动
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 按下攻击键进入轻攻击状态
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // 按下攻击键进入重攻击状态
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // 按下冲刺键进入冲刺状态
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        // 下落时，逐渐加大下落速度
        if (playerBlackboard.rb.velocity.y < 0)
        {
            playerBlackboard.rb.velocity += Vector2.up * Physics2D.gravity.y * (playerBlackboard.fallMultiplier - 1) * Time.deltaTime;
        }

        // 有水平输入且紧贴墙
        if (playerBlackboard.isClimb == true && horizontal_x != 0f)
        {
            playerFSM.SwitchState(StateType.Climb);
        }

        // 受伤时切换到受伤状态
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }

        // 按下射击键射出弓箭
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// 着地状态
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
        // 落地时播放落地动画，并重置玩家速度使之为0
        playerBlackboard.rb.velocity = Vector2.zero;
        playerBlackboard.playerAnimator.Play("Land");
    }

    public void OnExit()
    {
        
    }

    public void OnFixUpdate()
    {
        // 动画播放完毕进入闲置状态
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.99f)
        {
            playerFSM.SwitchState(StateType.Idle);
        }
    }

    public void OnUpdate()
    {
        // 受伤时切换到受伤状态
        if (playerBlackboard.isHit)
        {
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 爬墙状态
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
        playerBlackboard.rb.sharedMaterial = playerBlackboard.noFriction; // 切换为无摩擦材质，防止卡墙
    }

    public void OnExit()
    {
        playerBlackboard.rb.sharedMaterial = null;
    }

    public void OnFixUpdate()
    {
        // 玩家跳跃过程的水平移动
        horizontal_x = Input.GetAxisRaw("Horizontal");
        playerBlackboard.rb.velocity = new Vector2(horizontal_x * playerBlackboard.speed, playerBlackboard.rb.velocity.y);

        // 根据输入方向设置朝向
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
        { // 着地
            playerFSM.SwitchState(StateType.Idle);
        }

        if (!playerBlackboard.isClimb && !playerBlackboard.isGround)
        { // 未着地且未攀爬
            playerFSM.SwitchState(StateType.Falling);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        { // 爬墙时按下跳跃，进行蹬墙跳
            playerFSM.SwitchState(StateType.ClimbJump);
        }
        else
        { // 仍在爬墙，缓慢下滑
            playerBlackboard.rb.velocity = new Vector2(0, -playerBlackboard.climbSpeed);
        }
        
        if (playerBlackboard.isHit)
        { // 受伤时切换到受伤状态
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 爬墙跳状态
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
        // 跳跃
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

            // 根据输入方向设置朝向
            if (horizontal_x > 0)
            {
                playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
            }
            else if (horizontal_x < 0)
            {
                playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
            }

        }

        // 着地且速度竖直方向为负时，进入下落状态
        if ((playerBlackboard.isGround == false) && (playerBlackboard.rb.velocity.y < 0))
        {
            playerFSM.SwitchState(StateType.Fall);
        }
    }

    public void OnUpdate()
    {
        horizontalInputTimer += Time.deltaTime;

        // 玩家跳跃过程的水平移动
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 按下攻击键进入轻攻击状态
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerFSM.SwitchState(StateType.LightAttack1);
        }

        // 按下攻击键进入重攻击状态
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerFSM.SwitchState(StateType.HeavyAttack1);
        }

        // 按下冲刺键进入冲刺状态
        if (Input.GetKeyDown(KeyCode.L) && playerBlackboard.isDash)
        {
            playerFSM.SwitchState(StateType.Dash);
        }

        if (playerBlackboard.isClimb == true && horizontal_x != 0f)
        { // 有水平输入且紧贴墙
            playerFSM.SwitchState(StateType.Climb);
        }

        if (playerBlackboard.isHit)
        { // 受伤时切换到受伤状态
            playerFSM.SwitchState(StateType.Hurt);
        }

        // 按下射击键射出弓箭
        if (Input.GetKeyDown(KeyCode.U) && playerBlackboard.arrowCnt > 0)
        {
            playerFSM.SwitchState(StateType.Shoot);
        }
    }
}

// 冲刺状态
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
        playerBlackboard.lastDash = Time.time; // 记录最后冲刺的时间
        playerBlackboard.playerAnimator.Play("Dash");
    }

    public void OnExit()
    {
        // 重置速度
        playerBlackboard.rb.velocity = Vector2.zero;
    }

    public void OnFixUpdate()
    {
        if (playerBlackboard.dashTimeLeft > 0)
        { // 仍能冲刺
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

// 轻攻击状态1
public class PlayerLightAttack1State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // 记录刚进入该状态的时间
    private float currTime; // 记录按下轻攻击键的当前时间

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
        playerBlackboard.attack = playerBlackboard.baseLightAttack; // 此时攻击力为基础轻攻击力
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
        { // 只在地面时，增加攻击位移
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.lightAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            if (playerBlackboard.isLightAttack == false)
            { // 未按下轻攻击键
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isLightAttack == true)
            { // 按下攻击键，追加攻击
                playerFSM.SwitchState(StateType.LightAttack2);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // 轻攻击1过程中再次按下轻攻击键，判定为追加攻击
        if ((Input.GetKeyDown(KeyCode.J)) && (!playerBlackboard.isLightAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isLightAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // 受伤时切换到受伤状态
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 轻攻击状态2
public class PlayerLightAttack2State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // 记录刚进入该状态的时间
    private float currTime; // 记录按下轻攻击键的当前时间

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
        playerBlackboard.attack = playerBlackboard.baseLightAttack; // 此时攻击力为基础轻攻击力
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
        { // 只在地面时，增加攻击位移
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.lightAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            
            if (playerBlackboard.isLightAttack == false)
            { // 未按下轻攻击键
                playerFSM.SwitchState(StateType.Idle);
            } 
            else if (playerBlackboard.isLightAttack == true)
            { // 按下攻击键，追加攻击
                playerFSM.SwitchState(StateType.LightAttack3);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // 轻攻击2过程中再次按下轻攻击键，判定为追加攻击
        if ((Input.GetKeyDown(KeyCode.J)) && (!playerBlackboard.isLightAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isLightAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // 受伤时切换到受伤状态
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 轻攻击状态3
public class PlayerLightAttack3State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // 记录刚进入该状态的时间
    private float currTime; // 记录按下轻攻击键的当前时间

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
        playerBlackboard.attack = playerBlackboard.baseLightAttack; // 此时攻击力为基础轻攻击力
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
        { // 只在地面时，增加攻击位移
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.lightAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            
            if (playerBlackboard.isLightAttack == false)
            { // 未按下轻攻击键
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isLightAttack == true)
            { // 按下攻击键，追加攻击
                playerFSM.SwitchState(StateType.LightAttack1);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // 轻攻击3过程中再次按下轻攻击键，判定为追加攻击
        if ((Input.GetKeyDown(KeyCode.J)) && (!playerBlackboard.isLightAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isLightAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // 受伤时切换到受伤状态
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 重攻击状态1
public class PlayerHeavyAttack1State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // 记录刚进入该状态的时间
    private float currTime; // 记录按下轻攻击键的当前时间

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
        playerBlackboard.attack = playerBlackboard.baseHeavyAttack; // 此时攻击力为基础重攻击力
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
        { // 只在地面时，增加攻击位移
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.heavtAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            if (playerBlackboard.isHeavyAttack == false)
            { // 未按下轻攻击键
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isHeavyAttack == true)
            { // 按下攻击键，追加攻击
                playerFSM.SwitchState(StateType.HeavyAttack2);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // 轻攻击3过程中再次按下轻攻击键，判定为追加攻击
        if ((Input.GetKeyDown(KeyCode.K)) && (!playerBlackboard.isHeavyAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isHeavyAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // 受伤时切换到受伤状态
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 重攻击状态2
public class PlayerHeavyAttack2State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // 记录刚进入该状态的时间
    private float currTime; // 记录按下轻攻击键的当前时间

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
        playerBlackboard.attack = playerBlackboard.baseHeavyAttack; // 此时攻击力为基础重攻击力
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
        { // 只在地面时，增加攻击位移
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.heavtAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            if (playerBlackboard.isHeavyAttack == false)
            { // 未按下轻攻击键
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isHeavyAttack == true)
            { // 按下攻击键，追加攻击
                playerFSM.SwitchState(StateType.HeavyAttack3);
            }
        }
    }

    public void OnUpdate()
    {
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // 轻攻击3过程中再次按下轻攻击键，判定为追加攻击
        if ((Input.GetKeyDown(KeyCode.K)) && (!playerBlackboard.isHeavyAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isHeavyAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // 受伤时切换到受伤状态
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 重攻击状态3
public class PlayerHeavyAttack3State : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;
    private AnimatorStateInfo info;
    private float lastTime; // 记录刚进入该状态的时间
    private float currTime; // 记录按下轻攻击键的当前时间

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
        playerBlackboard.attack = playerBlackboard.baseHeavyAttack; // 此时攻击力为基础重攻击力
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
        { // 只在地面时，增加攻击位移
            playerBlackboard.rb.velocity = new Vector2(playerBlackboard.playerTransform.localScale.x * playerBlackboard.heavtAttackOffset, playerBlackboard.rb.velocity.y);
        }

        if (info.normalizedTime >= 0.99f)
        {
            if (playerBlackboard.isHeavyAttack == false)
            { // 未按下轻攻击键
                playerFSM.SwitchState(StateType.Idle);
            }
            else if (playerBlackboard.isHeavyAttack == true)
            { // 按下攻击键，追加攻击
                playerFSM.SwitchState(StateType.HeavyAttack1);
            }
        }
    }

    public void OnUpdate()
    {
        info = playerBlackboard.playerAnimator.GetCurrentAnimatorStateInfo(0);
        horizontal_x = Input.GetAxisRaw("Horizontal");

        // 根据输入方向设置朝向
        if (horizontal_x > 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal_x < 0)
        {
            playerBlackboard.playerTransform.localScale = new Vector3(-1, 1, 1);
        }

        // 轻攻击3过程中再次按下轻攻击键，判定为追加攻击
        if ((Input.GetKeyDown(KeyCode.K)) && (!playerBlackboard.isHeavyAttack))
        {
            currTime = Time.time;
            if (currTime - lastTime < playerBlackboard.intervalTime)
            {
                playerBlackboard.isHeavyAttack = true;
            }
        }

        if (playerBlackboard.isHit)
        { // 受伤时切换到受伤状态
            playerFSM.SwitchState(StateType.Hurt);
        }
    }
}

// 射箭状态
public class PlayerShootState : Istate
{
    private FSM playerFSM;
    private PlayerBlackboard playerBlackboard;

    private AnimatorStateInfo info;
    private bool isShoot; // 是否已射击
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
        { // 受伤时切换到受伤状态
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

// 受伤状态
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
