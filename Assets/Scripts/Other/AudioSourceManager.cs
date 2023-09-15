using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    private static AudioSourceManager _Instance;
    [SerializeField] private AudioSource BGMAudioSource;
    [SerializeField] private AudioSource soundAudioSource;
    [SerializeField] private AudioSource walkSoundAudioSource;
    private Dictionary<string, AudioClip> audioClips;

    public static AudioSourceManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<AudioSourceManager>();
                if (_Instance == null)
                {
                    GameObject gameObject = new GameObject("AudioSourceManager");
                    _Instance = gameObject.AddComponent<AudioSourceManager>();
                }
            }
            return _Instance;
        }
    }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        BGMAudioSource = GetComponents<AudioSource>()[0];
        soundAudioSource = GetComponents<AudioSource>()[1];
        walkSoundAudioSource = GetComponents<AudioSource>()[2];
        audioClips = new Dictionary<string, AudioClip>();

        PlayBGM(GlobalAudioClips.BGM1);
    }

    private void Update()
    {
        if (GameManager.Instance.isPaused == true)
        {
            walkSoundAudioSource.Stop();
        }
    }

    /// <summary>
    /// ������Ƶ��Դ
    /// </summary>
    /// <param name="path">·��</param>
    /// <returns></returns>
    public AudioClip LoadAudio(string path)
    {
        return Resources.Load<AudioClip>(path);
    }

    /// <summary>
    /// ������Ƶ��Դ�������ֵ�
    /// </summary>
    /// <param name="path">·��</param>
    /// <returns></returns>
    public AudioClip GetAudio(string path)
    {
        if (!audioClips.ContainsKey(path))
        {
            audioClips[path] = LoadAudio(path);
        }
        return audioClips[path];
    }

    /// <summary>
    /// BGM����
    /// </summary>
    /// <param name="name">·����</param>
    /// <param name="volume">����</param>
    public void PlayBGM(string name, float volume = 1.0f)
    {
        BGMAudioSource.Stop();
        BGMAudioSource.clip = GetAudio(name);
        BGMAudioSource.loop = true;
        BGMAudioSource.volume = volume;
        BGMAudioSource.Play();
    }

    /// <summary>
    /// ֹͣ����BGM
    /// </summary>
    public void StopBGM()
    {
        BGMAudioSource.Stop();
    }

    /// <summary>
    /// ��Ч����
    /// </summary>
    /// <param name="name">·����</param>
    /// <param name="volume">����</param>
    public void PlaySound(string name, float volume = 1.0f)
    {
        soundAudioSource.PlayOneShot(LoadAudio(name), volume);
    }

    /// <summary>
    /// ʹָ��AudioSource������Ч
    /// </summary>
    /// <param name="audioSource">������Ч��AudioSource</param>
    /// <param name="name">·����</param>
    /// <param name="volume">����</param>
    public void PlaySound(AudioSource audioSource, string name, float volume = 1.0f)
    {
        audioSource.PlayOneShot(LoadAudio(name), volume);
    }

    public void PlayPlayerWalkSound()
    {
        walkSoundAudioSource.clip = LoadAudio(GlobalAudioClips.PlayerWalk);
        walkSoundAudioSource.loop = true;
        walkSoundAudioSource.Play();
    }

    public void StopPlayerWalkSound()
    {
        walkSoundAudioSource.Stop();
    }
}

public class GlobalAudioClips
{
    #region ������Ч

    // ����˵���ť��Ч
    public const string ClickSound = "Music/SFX/Other/Click Button";

    // ����̵갴ť��Ч
    public const string ShopSound = "Music/SFX/Other/Shop Button";

    // �浵��Ч
    public const string CheckPoint = "Music/SFX/Other/CheckPoint";

    #endregion

    #region �����Ч

    // �����Ծ
    public const string PlayerJump = "Music/SFX/Player/PlayerJump";

    // �������
    public const string PlayerHurt = "Music/SFX/Player/PlayerHurt";

    // ����ṥ��
    public const string PlayerLightAttack = "Music/SFX/Player/PlayerLightAttack";

    // ����ع���
    public const string PlayerHeavyAttack = "Music/SFX/Player/PlayerHeavyAttack";

    // ������
    public const string PlayerShoot = "Music/SFX/Player/PlayerShoot";

    // ��ҳ��
    public const string PlayerDash = "Music/SFX/Player/PlayerDash";

    // ������
    public const string PlayerLand = "Music/SFX/Player/PlayerLand";

    // ��ұ���
    public const string PlayerWalk = "Music/SFX/Player/PlayerWalk";

    #endregion

    #region ������Ч

    // �����ܻ�
    public const string EnemyHurt = "Music/SFX/EnemyHurt";

    #endregion

    #region Ģ������Ч

    // �Ļ�
    public const string MushroomAttack1 = "Music/SFX/Mushroom/MushroomAttack1";

    // ˺ҧ
    public const string MushroomAttack2 = "Music/SFX/Mushroom/MushroomAttack2";

    // ���䶾Һ
    public const string MushroomShoot = "Music/SFX/Mushroom/MushroomShoot";

    // Ģ��������
    public const string MushroomDead = "Music/SFX/Mushroom/MushroomDead";

    #endregion

    #region �粼����Ч

    // �粼������
    public const string GoblinAttack = "Music/SFX/Goblin/GoblinAttack";

    // ը����ը
    public const string GoblinBombExplosion = "Music/SFX/Goblin/GoblinBombExplosion";

    // �粼������
    public const string GoblinDead = "Music/SFX/Goblin/GoblinDead";

    #endregion

    #region ������ʿ��Ч

    // ����1�ε�
    public const string SamuraiDrawNormal = "Music/SFX/Samurai/SamuraiDrawNormal";

    // ����1�ӿ�
    public const string SamuraiAttack1 = "Music/SFX/Samurai/SamuraiAttack1";

    // ����2�ε�
    public const string SamuraiDraw = "Music/SFX/Samurai/SamuraiDraw";

    // ����2�ӿ�
    public const string SamuraiAttack2 = "Music/SFX/Samurai/SamuraiAttack2";

    // ����3�ӿ�
    public const string SamuraiAttack3 = "Music/SFX/Samurai/SamuraiAttack3";

    // ����4����
    public const string SamuraiAttack4 = "Music/SFX/Samurai/SamuraiAttack4";

    #endregion

    #region ��������

    public const string BGM1 = "Music/BGM/BGM1";

    public const string BossFight = "Music/BGM/BossFight";

    #endregion
}
