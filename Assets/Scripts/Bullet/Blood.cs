using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    [SerializeField] private float destroyTime;

    private void OnEnable()
    {
        StartCoroutine(PushToPool());
    }

    // ªÿ ’
    IEnumerator PushToPool()
    {
        yield return new WaitForSeconds(destroyTime);
        ObjectPool.Instance.Push(gameObject);
    }
}
