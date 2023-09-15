using System.Collections.Generic;
using System;
using System.IO; // �ļ��Ķ�д
using Newtonsoft.Json; // �������л��ͷ����л�
using UnityEngine;

public class LocalConfig
{
    // ���ֵ�洢������û����ݣ���ֹ�������ļ�����������
    public static Dictionary<string, UserData> userDataDict = new Dictionary<string, UserData>();
    // ���������ܵ�����
    public static char[] keyChars = {'$', 'd', 'd', 'v', '@'};

    // ���ܷ���
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

    // ���ܷ���
    public static string Decrypt(string data)
    {
        return Encrypt(data);
    }

    public static void SaveUserData(UserData data)
    {
        // �����û����ݣ�Application.persistentDataPathΪUnity�ṩ��һ��Ŀ¼·������Ŀ¼�����ڴ洢�û�����
        if (!File.Exists(Application.persistentDataPath + "/users"))
        { // �������򴴽����ļ�
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/users");
        }
        userDataDict[data.username] = data;

        // ת��Ϊjson�ַ���
        string jsonData = JsonConvert.SerializeObject(data);
        // ����
        jsonData = Encrypt(jsonData);
        // д��json�ļ������û�������
        File.WriteAllText(Application.persistentDataPath + string.Format("/users/{0}.json", data.username), jsonData);
    }

    public static UserData LoadUserData(string username)
    {
        if (userDataDict.ContainsKey(username))
        { // �����д��ڸ�����
            return userDataDict[username];
        }

        // �����û�����
        string path = Application.persistentDataPath + string.Format("/users/{0}.json", username);
        if (File.Exists(path))
        {
            // ��ȡ��������
            string jsonData = File.ReadAllText(path);
            // ����
            jsonData = Decrypt(jsonData);
            UserData data = JsonConvert.DeserializeObject<UserData>(jsonData);
            return data;
        }
        else
        {
            Debug.LogError("�浵������!");
            return null;
        }
    }
}

[Serializable]
public class UserData
{ // �û�������
    public string username; // �û��浵��
    public float health; // ����ֵ
    public int coinCnt; // �������
    public Dictionary<int, string> slotDict; // ��������Ʒ���ڲ�
    public Dictionary<string, int> itemsDict; // ��������Ʒ������
    public List<Mission> missionList;
    public int sceneID; // ���ڳ���
    public int checkPointID; // ����ID
    public bool killBoss = false; // �Ƿ��ɱBOSS

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
