using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public LayerMask sightLayerMask;
    public float sightLenth;
    private RaycastHit2D target;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        FindTarget();
    }

    private void FindTarget()
    {
        if (transform.parent.localScale.x == -1)
        { // ³¯Ïò×ó±ß
            target = Physics2D.Raycast(transform.position, Vector2.left, sightLenth, sightLayerMask);
        }
        else if (transform.parent.localScale.x == 1)
        { // ³¯Ïò×ó±ß
            target = Physics2D.Raycast(transform.position, Vector2.right, sightLenth, sightLayerMask);
        }

        if (target.collider != null)
        {
            transform.parent.SendMessage("SetTarget", target.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (transform.parent.localScale.x == -1)
        {
            Gizmos.DrawRay(transform.position, Vector2.left * sightLenth);
        }
        else if (transform.parent.localScale.x == 1)
        {
            Gizmos.DrawRay(transform.position, Vector2.right * sightLenth);
        }
    }
}
