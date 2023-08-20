using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsClimb : MonoBehaviour
{
    public LayerMask WallLayerMask;
    public bool isClimb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IsClimbTheWall();
        transform.parent.SendMessage("UpdateClimbStatus", isClimb);
    }

    void IsClimbTheWall()
    {
        var rayCastWallUp = Physics2D.RaycastAll(transform.position + new Vector3(0f, 0.3f, 0f), Vector2.right * transform.parent.localScale.x, 0.25f, WallLayerMask);
        var rayCastWallDown = Physics2D.RaycastAll(transform.position - new Vector3(0f, 0.3f, 0f), Vector2.right * transform.parent.localScale.x, 0.25f, WallLayerMask);
        if ((rayCastWallUp.Length > 0) || (rayCastWallDown.Length > 0))
        {
            isClimb = true;
        }
        else
        {
            isClimb = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + new Vector3(0f, 0.3f, 0f), Vector2.right * transform.parent.localScale.x * 0.25f);
        Gizmos.DrawRay(transform.position - new Vector3(0f, 0.3f, 0f), Vector2.right * transform.parent.localScale.x * 0.25f);
    }
}
