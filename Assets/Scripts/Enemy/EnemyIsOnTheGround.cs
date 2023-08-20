using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIsOnTheGround : MonoBehaviour
{
    // ���Ĳ�
    public LayerMask feetLayerMask;
    
    // �Ƿ�λ�ڵ���
    public bool isGround;

    // ���ҽ�λ��
    public Vector2 leftFoot;
    public Vector2 rightFoot;

    // ���ҽ���transform��λ��
    public Vector3 leftOffset;
    public Vector3 rightOffset;

    // ���߳���
    public float lenth = 0.08f;

    // Start is called before the first frame update
    void Start()
    {
        leftFoot = transform.position - leftOffset;
        rightFoot = transform.position - rightOffset;
    }

    // Update is called once per frame
    void Update()
    {
        IsOnTheGround();
        transform.parent.SendMessage("UpdateGroundStatus", isGround); // �㲥��������
    }

    private void IsOnTheGround()
    {
        leftFoot = transform.position - leftOffset;
        rightFoot = transform.position - rightOffset;

        var rayCastGroundLeft = Physics2D.RaycastAll(leftFoot, Vector2.down, lenth, feetLayerMask);
        var rayCastGroundRight = Physics2D.RaycastAll(rightFoot, Vector2.down, lenth, feetLayerMask);
        if ((rayCastGroundLeft.Length > 0) || (rayCastGroundRight.Length > 0))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(leftFoot, Vector3.down * lenth);
        Gizmos.DrawRay(rightFoot, Vector3.down * lenth);
    }
}
