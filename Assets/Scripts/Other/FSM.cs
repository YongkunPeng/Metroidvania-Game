using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//状态枚举
public enum StateType
{
    Idle, //待机
    Run, //玩家移动
    Dead, //死亡
    Hurt, //受击

    Jump, //跳跃
    Fall, //下落
    Falling, //下落中
    Dash, //冲刺
    Land, //落地
    LightAttack1, //玩家轻攻击1
    LightAttack2, //玩家轻攻击2
    LightAttack3, //玩家轻攻击3
    HeavyAttack1, //玩家重攻击1
    HeavyAttack2, //玩家重攻击2
    HeavyAttack3, //玩家重攻击3
    Shoot, //玩家射击
    Climb, //玩家攀爬
    ClimbJump, //蹬墙跳

    Cool, //攻击冷却
    Evade, //回避
    Patrol, //巡逻
    Chase, //追击
    Attack1, //敌人攻击1
    Attack2, //敌人攻击2
    Attack3, //敌人攻击3
    Attack4, //敌人攻击4
}

// 状态类接口，规范实现
public interface Istate
{
    // 进入状态
    void OnEnter();
    // 处于状态
    void OnUpdate();
    // 退出状态
    void OnExit();
    // 每秒定量帧
    void OnFixUpdate();
    // 
    void OnCheck();
}

[Serializable]
public class Blackboard
{
    [Header("基本数据")]
    // 黑板类，用于存储基本参数
    public float health; // 生命值
    public float baseLightAttack; // 基础轻攻击伤害
    public float baseHeavyAttack; // 基础重攻击伤害
    public float speed; // 移动速度
    public float jumpSpeed; // 跳跃高度
}

public class FSM
{
    // 当前状态
    public Istate currState;
    // 状态字典
    public Dictionary<StateType, Istate> states;
    public Blackboard blackboard;

    public FSM(Blackboard blackboard)
    {
        this.states = new Dictionary<StateType, Istate>();
        this.blackboard = blackboard;
    }

    // 添加状态
    public void AddState(StateType stateType, Istate istate)
    {   
        // 检查是否重复添加
        if (states.ContainsKey(stateType))
        {
            Debug.LogWarning("状态已被添加，无法重复！");
            return;
        }
        states.Add(stateType, istate);
    }

    // 切换状态
    public void SwitchState(StateType stateType)
    {   
        // 检查是否存在状态
        if (!states.ContainsKey(stateType))
        {
            Debug.LogWarning("所要切换的状态不存在！");
            return;
        }
        // 退出当前状态
        if (currState != null)
        {
            currState.OnExit();
        }
        // 进入到新状态
        currState = states[stateType];
        currState.OnEnter();
    }

    public void OnUpdate()
    {
        currState.OnUpdate();
    }

    public void OnFixUpdate()
    {
        currState.OnFixUpdate();
    }

    public void OnCheck()
    {
        currState.OnCheck();
    }

    // 朝向目标对象
    public void FlipToTransform(Transform current, Transform target)
    {
        if (target != null)
        {
            if (current.position.x > target.position.x)
            {
                current.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                current.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    // 朝向目标点
    public void FlipToPoint(Transform current, Vector2 target)
    {
        if (target != null)
        {
            if (current.position.x > target.x)
            {
                current.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                current.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
