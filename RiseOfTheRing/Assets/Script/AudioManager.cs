using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// AudioManager handles playing one-shot sound effects (SFX) such as jump, damage, and power-up.
/// It uses a single AudioSource (sfxSource) that is manually assigned in the Inspector.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source (assign manually in the Inspector)")]
    public AudioSource sfxSource;   // AudioSource for playing one-shot SFX (Jump, Damage, PowerUp)

    [Header("Audio Clips")]
    public AudioClip jumpClip;      // Clip to play when the player jumps
    public AudioClip damageClip;    // Clip to play when the player takes damage
    public AudioClip powerupClip;   // Clip to play when the player picks up a power-up

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;    // Master volume for all one-shot sound effects

    private void Awake()
    {
        // Ensure only one AudioManager exists in the scene (singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
        else
        {
            Destroy(gameObject); // Destroy any extra duplicate
            return;
        }
    }


    /// Play the jump sound effect as a one-shot on the sfxSource.
    public void PlayJumpSound()
    {
        if (jumpClip == null || sfxSource == null)
            return;

        // Play the jump clip once at the configured sfxVolume
        sfxSource.PlayOneShot(jumpClip, sfxVolume);
    }


    /// Play the damage sound effect as a one-shot on the sfxSource.
    public void PlayDamageSound()
    {
        if (damageClip == null || sfxSource == null)
            return;

        // Play the damage clip once at the configured sfxVolume
        sfxSource.PlayOneShot(damageClip, sfxVolume);
    }


    /// Play the power-up pickup sound effect as a one-shot on the sfxSource.
    public void PlayPowerupSound()
    {
        if (powerupClip == null || sfxSource == null)
            return;

        // Play the power-up pickup clip once at the configured sfxVolume
        sfxSource.PlayOneShot(powerupClip, sfxVolume);
    }


    /// Adjust the global one-shot SFX volume at runtime.
    public void SetSFXVolume(float vol)
    {
        sfxVolume = Mathf.Clamp01(vol); // Keep volume between 0 and 1
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
}
