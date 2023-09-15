using System.Collections.Generic;
using System;
using System.IO; // 文件的读写
using Newtonsoft.Json; // 数据序列化和反序列化
using UnityEngine;

public class LocalConfig
{
    // 用字典存储读入的用户数据，防止反复读文件，降低性能
    public static Dictionary<string, UserData> userDataDict = new Dictionary<string, UserData>();
    // 用于异或加密的数组
    public static char[] keyChars = {'$', 'd', 'd', 'v', '@'};

    // 加密方法
    public static string Encrypt(string data)
    {
        char[] dataChars = data.ToCharArray();
        for (int i = 0; i < dataChars.Length; i++)
        {
            char dataChar = dataChars[i];
            char keyChar = keyChars[i % keyChars.Length];
            char newChar = (char)(dataChar ^ keyChar);
            dataChars[i] = newChar;
        }
        return new string(dataChars);
    }

    // 解密方法
    public static string Decrypt(string data)
    {
        return Encrypt(data);
    }

    public static void SaveUserData(UserData data)
    {
        // 保存用户数据，Application.persistentDataPath为Unity提供的一个目录路径，该目录可用于存储用户数据
        if (!File.Exists(Application.persistentDataPath + "/users"))
        { // 不存在则创建该文件
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/users");
        }
        userDataDict[data.username] = data;

        // 转换为json字符串
        string jsonData = JsonConvert.SerializeObject(data);
        // 加密
        jsonData = Encrypt(jsonData);
        // 写入json文件，按用户名分类
        File.WriteAllText(Application.persistentDataPath + string.Format("/users/{0}.json", data.username), jsonData);
    }

    public static UserData LoadUserData(string username)
    {
        if (userDataDict.ContainsKey(username))
        { // 缓存中存在该数据
            return userDataDict[username];
        }

        // 加载用户数据
        string path = Application.persistentDataPath + string.Format("/users/{0}.json", username);
        if (File.Exists(path))
        {
            // 读取所有内容
            string jsonData = File.ReadAllText(path);
            // 解密
            jsonData = Decrypt(jsonData);
            UserData data = JsonConvert.DeserializeObject<UserData>(jsonData);
            return data;
        }
        else
        {
            Debug.LogError("存档不存在!");
            return null;
        }
    }
}

[Serializable]
public class UserData
{ // 用户数据类
    public string username; // 用户存档名
    public float health; // 生命值
    public int coinCnt; // 金币数量
    public Dictionary<int, string> slotDict; // 背包中物品所在槽
    public Dictionary<string, int> itemsDict; // 背包中物品的数量
    public List<Mission> missionList;
    public int sceneID; // 所在场景
    public int checkPointID; // 检查点ID
    public bool killBoss = false; // 是否击杀BOSS

    public UserData(string username, float health, int arrowCnt, int coinCnt)
    {
        this.username = username;
        this.health = health;
        this.coinCnt = coinCnt;
    }

    public UserData()
    {

    }
}
