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
    /// 加载音频资源
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public AudioClip LoadAudio(string path)
    {
        return Resources.Load<AudioClip>(path);
    }

    /// <summary>
    /// 加载音频资源并存入字典
    /// </summary>
    /// <param name="path">路径</param>
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
    /// BGM播放
    /// </summary>
    /// <param name="name">路径名</param>
    /// <param name="volume">音量</param>
    public void PlayBGM(string name, float volume = 1.0f)
    {
        BGMAudioSource.Stop();
        BGMAudioSource.clip = GetAudio(name);
        BGMAudioSource.loop = true;
        BGMAudioSource.volume = volume;
        BGMAudioSource.Play();
    }

    /// <summary>
    /// 停止播放BGM
    /// </summary>
    public void StopBGM()
    {
        BGMAudioSource.Stop();
    }

    /// <summary>
    /// 音效播放
    /// </summary>
    /// <param name="name">路径名</param>
    /// <param name="volume">音量</param>
    public void PlaySound(string name, float volume = 1.0f)
    {
        soundAudioSource.PlayOneShot(LoadAudio(name), volume);
    }

    /// <summary>
    /// 使指定AudioSource播放音效
    /// </summary>
    /// <param name="audioSource">播放音效的AudioSource</param>
    /// <param name="name">路径名</param>
    /// <param name="volume">音量</param>
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
    #region 其它音效

    // 点击菜单按钮音效
    public const string ClickSound = "Music/SFX/Other/Click Button";

    // 点击商店按钮音效
    public const string ShopSound = "Music/SFX/Other/Shop Button";

    // 存档音效
    public const string CheckPoint = "Music/SFX/Other/CheckPoint";

    #endregion

    #region 玩家音效

    // 玩家跳跃
    public const string PlayerJump = "Music/SFX/Player/PlayerJump";

    // 玩家受伤
    public const string PlayerHurt = "Music/SFX/Player/PlayerHurt";

    // 玩家轻攻击
    public const string PlayerLightAttack = "Music/SFX/Player/PlayerLightAttack";

    // 玩家重攻击
    public const string PlayerHeavyAttack = "Music/SFX/Player/PlayerHeavyAttack";

    // 玩家射箭
    public const string PlayerShoot = "Music/SFX/Player/PlayerShoot";

    // 玩家冲刺
    public const string PlayerDash = "Music/SFX/Player/PlayerDash";

    // 玩家落地
    public const string PlayerLand = "Music/SFX/Player/PlayerLand";

    // 玩家奔跑
    public const string PlayerWalk = "Music/SFX/Player/PlayerWalk";

    #endregion

    #region 敌人音效

    // 敌人受击
    public const string EnemyHurt = "Music/SFX/EnemyHurt";

    #endregion

    #region 蘑菇人音效

    // 拍击
    public const string MushroomAttack1 = "Music/SFX/Mushroom/MushroomAttack1";

    // 撕咬
    public const string MushroomAttack2 = "Music/SFX/Mushroom/MushroomAttack2";

    // 喷射毒液
    public const string MushroomShoot = "Music/SFX/Mushroom/MushroomShoot";

    // 蘑菇人死亡
    public const string MushroomDead = "Music/SFX/Mushroom/MushroomDead";

    #endregion

    #region 哥布林音效

    // 哥布林劈砍
    public const string GoblinAttack = "Music/SFX/Goblin/GoblinAttack";

    // 炸弹爆炸
    public const string GoblinBombExplosion = "Music/SFX/Goblin/GoblinBombExplosion";

    // 哥布林死亡
    public const string GoblinDead = "Music/SFX/Goblin/GoblinDead";

    #endregion

    #region 发狂武士音效

    // 攻击1拔刀
    public const string SamuraiDrawNormal = "Music/SFX/Samurai/SamuraiDrawNormal";

    // 攻击1挥砍
    public const string SamuraiAttack1 = "Music/SFX/Samurai/SamuraiAttack1";

    // 攻击2拔刀
    public const string SamuraiDraw = "Music/SFX/Samurai/SamuraiDraw";

    // 攻击2挥砍
    public const string SamuraiAttack2 = "Music/SFX/Samurai/SamuraiAttack2";

    // 攻击3挥砍
    public const string SamuraiAttack3 = "Music/SFX/Samurai/SamuraiAttack3";

    // 攻击4剑气
    public const string SamuraiAttack4 = "Music/SFX/Samurai/SamuraiAttack4";

    #endregion

    #region 背景音乐

    public const string BGM1 = "Music/BGM/BGM1";

    public const string BossFight = "Music/BGM/BossFight";

    #endregion
}
