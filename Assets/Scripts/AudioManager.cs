using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseSingleton<AudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip upgradeSound;
    [SerializeField] private AudioClip swordSlashSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip rockHitSound;

    private AudioSource backgroundSource;

    protected override void Awake()
    {
        base.Awake();
        
        backgroundSource = gameObject.AddComponent<AudioSource>();
        backgroundSource.clip = backgroundMusic;
        backgroundSource.loop = true;
        backgroundSource.playOnAwake = true;
        
        if (backgroundMusic != null)
        {
            backgroundSource.Play();
        }
    }

    private Dictionary<AudioClip, float> lastPlayTimes = new Dictionary<AudioClip, float>();
    [SerializeField] private float hitSoundInterval = 0.1f; // Khoảng cách tối thiểu giữa 2 lần phát âm thanh hit

    public void PlayClick()
    {
        PlayEffect(clickSound);
    }

    public void PlayCancel()
    {
        PlayEffect(cancelSound);
    }

    public void PlayWin()
    {
        PlayEffect(winSound);
    }

    public void PlayLose()
    {
        PlayEffect(loseSound);
    }

    public void PlayUpgrade()
    {
        PlayEffect(upgradeSound);
    }

    public void PlaySwordSlash()
    {
        PlayEffect(swordSlashSound);
    }

    public void PlayHit()
    {
        PlayEffectLimited(hitSound, hitSoundInterval);
    }

    public void PlayRockHit(Vector3 position)
    {
        if (IsNearOrOnScreen(position))
        {
            PlayEffectLimited(rockHitSound, hitSoundInterval);
        }
    }

    private bool IsNearOrOnScreen(Vector3 position)
    {
        Camera cam = Camera.main;
        if (cam == null) return true;
        Vector3 viewportPoint = cam.WorldToViewportPoint(position);
    
        bool isOnScreen = viewportPoint.z > 0 && 
                          viewportPoint.x > -0.1f && viewportPoint.x < 1.1f && 
                          viewportPoint.y > -0.1f && viewportPoint.y < 1.1f;
        
        if (isOnScreen) return true;

        float maxDistance = 15f; 
        return Vector3.Distance(cam.transform.position, position) < maxDistance;
    }

    private void PlayEffectLimited(AudioClip clip, float interval)
    {
        if (clip == null) return;

        if (lastPlayTimes.ContainsKey(clip))
        {
            if (Time.time - lastPlayTimes[clip] < interval)
                return;
        }

        lastPlayTimes[clip] = Time.time;
        PlayEffect(clip);
    }

    private void PlayEffect(AudioClip clip)
    {
        if (clip == null) return;

        GameObject audioObj = new GameObject("TempAudio_" + clip.name);
        AudioSource source = audioObj.AddComponent<AudioSource>();
        
        
        source.clip = clip;
        source.pitch = Random.Range(0.9f, 1.1f);
        source.Play();

        DontDestroyOnLoad(audioObj);
        Destroy(audioObj, clip.length);
    }

    public void SetBackgroundMusic(AudioClip clip)
    {
        if (backgroundSource.clip == clip) return;
        
        backgroundSource.Stop();
        backgroundSource.clip = clip;
        if (clip != null)
        {
            backgroundSource.Play();
        }
    }
}
