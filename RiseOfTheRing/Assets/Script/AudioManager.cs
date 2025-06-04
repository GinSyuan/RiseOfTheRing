using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;   // AudioSource for one-shot sound effects (e.g., jump, damage)
    public AudioSource acidSource;  // AudioSource for continuous acid sound
    public AudioSource bgmSource;   // AudioSource for background music

    [Header("Audio Clips")]
    public AudioClip jumpClip;      // Clip to play when the player jumps
    public AudioClip damageClip;    // Clip to play when the player takes damage
    public AudioClip powerupClip;   // Clip to play when the player picks up a power-up
    public AudioClip acidClip;      // Clip for the acid sound effect
    public AudioClip bgmClip;       // Clip for the background music

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;    // Master volume for all SFX
    [Range(0f, 1f)] public float acidVolume = 0.5f;  // Volume for the acid sound
    [Range(0f, 1f)] public float bgmVolume = 0.5f;   // Master volume for background music

    private void Awake()
    {
        // Implement singleton pattern so there is only one AudioManager across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // If a BGM clip is already assigned, start playing it on loop
        if (bgmSource != null && bgmClip != null)
        {
            PlayBGM(bgmClip, loop: true);
        }
    }


    /// Play a one-shot jump sound at the current SFX volume.
    public void PlayJumpSound()
    {
        if (jumpClip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(jumpClip, sfxVolume);
    }


    /// Play a one-shot damage sound at the current SFX volume.
    public void PlayDamageSound()
    {
        if (damageClip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(damageClip, sfxVolume);
    }


    /// Play a one-shot power-up sound at the current SFX volume.
    public void PlayPowerupSound()
    {
        if (powerupClip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(powerupClip, sfxVolume);
    }


    /// Start playing the acid loop if it’s not already playing.
    public void PlayAcidSound()
    {
        if (acidClip == null || acidSource == null) return;
        if (acidSource.isPlaying && acidSource.clip == acidClip) return;

        acidSource.clip = acidClip;
        acidSource.volume = acidVolume;
        acidSource.loop = true;
        acidSource.Play();
    }

 
    /// Stop the acid sound if it is currently playing.
    public void StopAcidSound()
    {
        if (acidSource == null) return;
        if (acidSource.isPlaying)
            acidSource.Stop();
    }


    /// Adjust the volume of the acid sound dynamically.
    public void SetAcidVolume(float vol)
    {
        acidVolume = Mathf.Clamp01(vol); // Ensure volume is between 0 and 1
        if (acidSource != null)
            acidSource.volume = acidVolume;
    }


    /// Play or loop the specified background music clip.
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.isPlaying && bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = loop;
        bgmSource.Play();
    }


    /// Stop the background music if it is playing.
    public void StopBGM()
    {
        if (bgmSource == null) return;
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }


    /// Set the background music volume. This method is public so the UI slider can call it.
    public void SetBGMVolume(float vol)
    {
        bgmVolume = Mathf.Clamp01(vol); // Clamp between 0 and 1
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
    }


    /// Adjust the SFX master volume. 
    public void SetSFXVolume(float vol)
    {
        sfxVolume = Mathf.Clamp01(vol); // Clamp between 0 and 1
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
}
