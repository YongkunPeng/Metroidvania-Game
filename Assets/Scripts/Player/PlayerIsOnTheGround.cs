using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsOnTheGround : MonoBehaviour
{
    public LayerMask feetLayerMask;
    public bool isGround;
    [SerializeField] private float lenth = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IsOnTheGround();
        transform.parent.SendMessage("UpdateGroundStatus", isGround); // 广播给父对象
    }

    // 实时更新着地状态
    private void IsOnTheGround()
    {
        var rayCastGroundLeft = Physics2D.RaycastAll(transform.position - new Vector3(0.15f, 0.45f, 0), Vector2.down, lenth, feetLayerMask);
        var rayCastGroundRight = Physics2D.RaycastAll(transform.position - new Vector3(-0.14f, 0.45f, 0), Vector2.down, lenth, feetLayerMask);
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
        Gizmos.DrawRay(transform.position - new Vector3(0.15f, 0.45f, 0), Vector3.down * lenth);
        Gizmos.DrawRay(transform.position - new Vector3(-0.14f, 0.45f, 0), Vector3.down * lenth);
    }
}
