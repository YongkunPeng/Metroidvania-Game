using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//״̬ö��
public enum StateType
{
    Idle, //����
    Run, //����ƶ�
    Dead, //����
    Hurt, //�ܻ�

    Jump, //��Ծ
    Fall, //����
    Falling, //������
    Dash, //���
    Land, //���
    LightAttack1, //����ṥ��1
    LightAttack2, //����ṥ��2
    LightAttack3, //����ṥ��3
    HeavyAttack1, //����ع���1
    HeavyAttack2, //����ع���2
    HeavyAttack3, //����ع���3
    Shoot, //������
    Climb, //�������
    ClimbJump, //��ǽ��

    Cool, //������ȴ
    Evade, //�ر�
    Patrol, //Ѳ��
    Chase, //׷��
    Attack1, //���˹���1
    Attack2, //���˹���2
    Attack3, //���˹���3

}

// ״̬��ӿڣ��淶ʵ��
public interface Istate
{
    // ����״̬
    void OnEnter();
    // ����״̬
    void OnUpdate();
    // �˳�״̬
    void OnExit();
    // ÿ�붨��֡
    void OnFixUpdate();
    // 
    void OnCheck();
}

[Serializable]
public class Blackboard
{
    [Header("��������")]
    // �ڰ��࣬���ڴ洢��������
    public float health; // ����ֵ
    public float baseLightAttack; // �����ṥ���˺�
    public float baseHeavyAttack; // �����ع����˺�
    public float speed; // �ƶ��ٶ�
    public float jumpSpeed; // ��Ծ�߶�
}

public class FSM
{
    // ��ǰ״̬
    public Istate currState;
    // ״̬�ֵ�
    public Dictionary<StateType, Istate> states;
    public Blackboard blackboard;

    public FSM(Blackboard blackboard)
    {
        this.states = new Dictionary<StateType, Istate>();
        this.blackboard = blackboard;
    }

    // ���״̬
    public void AddState(StateType stateType, Istate istate)
    {   
        // ����Ƿ��ظ����
        if (states.ContainsKey(stateType))
        {
            Debug.LogWarning("״̬�ѱ���ӣ��޷��ظ���");
            return;
        }
        states.Add(stateType, istate);
    }

    // �л�״̬
    public void SwitchState(StateType stateType)
    {   
        // ����Ƿ����״̬
        if (!states.ContainsKey(stateType))
        {
            Debug.LogWarning("��Ҫ�л���״̬�����ڣ�");
            return;
        }
        // �˳���ǰ״̬
        if (currState != null)
        {
            currState.OnExit();
        }
        // ���뵽��״̬
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

    // ����Ŀ�����
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

    // ����Ŀ���
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
