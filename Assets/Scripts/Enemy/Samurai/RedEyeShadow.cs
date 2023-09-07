using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RedEyeShadow : MonoBehaviour
{
    [SerializeField] private GameObject redEyeShadow;

    private void Update()
    {
        GameObject gameObject = ObjectPool.Instance.Get(redEyeShadow);
        gameObject.transform.position = transform.position;
    }
}
