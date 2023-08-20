using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private static ObjectPool _Instance; // ����
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>(); // �ֵ䣬�洢����Ԥ�������
    private GameObject pool = null; // ��������ؽ�㣬���ڴ洢����Ԥ����Ķ���أ����𵽷�������

    public static ObjectPool Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new ObjectPool();
            }
            return _Instance;
        }
    }

    public GameObject Get(GameObject prefab) // �˴�prefab.name��(Clone)
    {
        GameObject obj;

        if (objectPool.ContainsKey(prefab.name) && objectPool[prefab.name].Count != 0)
        { // �л�����ʱ�����������ɾ�������ֵ��������ڣ��ж϶���Ϊ�յ��ֵ��ֵ�ԣ�ɾ���ü�ֵ��
            if (objectPool[prefab.name].Peek() == null)
            {
                // objectPool.Remove(prefab.name);
                objectPool[prefab.name].Clear();
            }
        }

        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        { // �����ڸö���Ķ���ػ������ڶ������ù�
            obj = GameObject.Instantiate(prefab);
            Push(obj); // ��������ӵ���Ӧ�Ķ�����
            if (pool == null)
            { // ���ܵĶ���ؽ��Ϊ����������Unity��Ϊ�䴴������Ϊ���ж���صĸ��ڵ�
                pool = new GameObject("ObjectPool");
            }
            GameObject childPool = GameObject.Find("ObjectPool/" + prefab.name + "Pool"); //�Ӷ���أ�������Ӧ���Ӷ���
            if (!childPool)
            { // �����ڵ�ǰԤ������Ӷ���أ���Unity��Ϊ�䴴����Ӧ���Ӷ����
                childPool = new GameObject(prefab.name + "Pool"); // �����Ӷ����
                childPool.transform.SetParent(pool.transform); // ���Ӷ������Ϊ������ص��ӽڵ�
            }
            obj.transform.SetParent(childPool.transform);
        }
        obj = objectPool[prefab.name].Dequeue(); // ʹһ������Ӷ�Ӧ�Ķ����г���
        obj.SetActive(true); // ����
        return obj;
    }

    public void Push(GameObject prefab) // �˴�prefab.name��(Clone)
    {
        Rigidbody2D rb = prefab.GetComponent<Rigidbody2D>();
        CircleCollider2D cirCol = prefab.GetComponent<CircleCollider2D>();
        BoxCollider2D boxCol = prefab.GetComponent<BoxCollider2D>();
        SpriteRenderer sprite = prefab.GetComponent <SpriteRenderer>();
        string name = prefab.name.Replace("(Clone)", string.Empty);

        if (!objectPool.ContainsKey(name))
        { // �����ڸ��������½������Ͷ���
            objectPool.Add(name, new Queue<GameObject>());
        }
        objectPool[name].Enqueue(prefab); // ���

        if (rb != null)
        { // ���������ײ���ָ���Ĭ��
            rb.gravityScale = 1;
            rb.simulated = true;
            if (cirCol != null)
            {
                cirCol.enabled = true;
            }
            else if (boxCol != null)
            {
                boxCol.enabled = true;
            }
        }
        if (sprite != null)
        {
            sprite.color = Color.white;
        }

        prefab.SetActive(false); // ʧ��
    }
}
