using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// PowerUp handles the behavior when the player collects a power-up.
/// It plays a power-up sound via AudioManager and pauses the acid's rising for a set duration.
public class PowerUp : MonoBehaviour
{
    [Header("Settings")]
    public float pauseDuration = 2f;  // How many seconds to pause the acid rising


    /// Called when another Collider2D collides with this PowerUp
    /// If it is the player, play SFX, pause the acid, and destroy the power-up.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collider belongs to the player
        if (collision.collider.CompareTag("Player"))
        {
            // Play power-up pickup sound via AudioManager
            AudioManager.Instance.PlayPowerupSound();

            // Find the AcidRising component in the scene and pause it
            AcidRising acid = FindObjectOfType<AcidRising>();
            if (acid != null)
            {
                acid.PauseRising(pauseDuration);
            }

            // Destroy this power-up object so it can't be used again
            Destroy(gameObject);
        }
    }
}

