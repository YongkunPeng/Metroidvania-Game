using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private static ObjectPool _Instance; // 单例
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>(); // 字典，存储各类预制体对象
    private GameObject pool = null; // 整个对象池结点，用于存储各个预制体的对象池，并起到分类作用

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

    public GameObject Get(GameObject prefab) // 此处prefab.name无(Clone)
    {
        GameObject obj;

        if (objectPool.ContainsKey(prefab.name) && objectPool[prefab.name].Count != 0)
        { // 切换场景时，对象池物体删除，但字典数据仍在，判断队列为空的字典键值对，删除该键值对
            if (objectPool[prefab.name].Peek() == null)
            {
                // objectPool.Remove(prefab.name);
                objectPool[prefab.name].Clear();
            }
        }

        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        { // 不存在该对象的对象池或对象池内对象已用光
            obj = GameObject.Instantiate(prefab);
            Push(obj); // 将对象添加到相应的队列中
            if (pool == null)
            { // 若总的对象池结点为创建，则在Unity中为其创建，作为所有对象池的父节点
                pool = new GameObject("ObjectPool");
            }
            GameObject childPool = GameObject.Find("ObjectPool/" + prefab.name + "Pool"); //子对象池，存贮对应的子对象
            if (!childPool)
            { // 不存在当前预制体的子对象池，在Unity中为其创建对应的子对象池
                childPool = new GameObject(prefab.name + "Pool"); // 创建子对象池
                childPool.transform.SetParent(pool.transform); // 将子对象池作为父对象池的子节点
            }
            obj.transform.SetParent(childPool.transform);
        }
        obj = objectPool[prefab.name].Dequeue(); // 使一个对象从对应的队列中出队
        obj.SetActive(true); // 激活
        return obj;
    }

    public void Push(GameObject prefab) // 此处prefab.name有(Clone)
    {
        Rigidbody2D rb = prefab.GetComponent<Rigidbody2D>();
        CircleCollider2D cirCol = prefab.GetComponent<CircleCollider2D>();
        BoxCollider2D boxCol = prefab.GetComponent<BoxCollider2D>();
        SpriteRenderer sprite = prefab.GetComponent <SpriteRenderer>();
        string name = prefab.name.Replace("(Clone)", string.Empty);

        if (!objectPool.ContainsKey(name))
        { // 不存在该索引，新建索引和队列
            objectPool.Add(name, new Queue<GameObject>());
        }
        objectPool[name].Enqueue(prefab); // 入队

        if (rb != null)
        { // 将刚体和碰撞器恢复到默认
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

        prefab.SetActive(false); // 失活
    }
}
