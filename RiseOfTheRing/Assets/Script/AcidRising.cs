using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// AcidRising moves the acid GameObject upward over time, with a gradual acceleration.
/// It can be paused for a specified duration
public class AcidRising : MonoBehaviour
{
    [Header("Rising Settings")]
    public float baseRiseSpeed = 1f;   // Initial upward speed of the acid
    public float acceleration = 0.1f;  // Amount the speed increases per second

    private float currentRiseSpeed;    // Tracks the current upward speed
    private bool isPaused = false;     // Indicates whether the acid is temporarily paused

    private void Start()
    {
        AudioManager.Instance.PlayAcidSound();
        // Initialize current rise speed with the base value
        currentRiseSpeed = baseRiseSpeed;
    }

    private void Update()
    {
        // If paused, skip rising logic
        if (isPaused)
            return;

        // Gradually increase the rising speed over time
        currentRiseSpeed += acceleration * Time.deltaTime;

        // Move the acid upward by currentRiseSpeed
        transform.Translate(Vector2.up * currentRiseSpeed * Time.deltaTime);
    }


    /// Temporarily pause the acid's upward movement for a given duration.
    public void PauseRising(float seconds)
    {
        if (!isPaused)
            StartCoroutine(PauseCoroutine(seconds));
    }


    /// Coroutine that handles the pause duration.
    private IEnumerator PauseCoroutine(float seconds)
    {
        isPaused = true;                // Mark as paused
        AudioManager.Instance.StopAcidSound();

        yield return new WaitForSeconds(seconds);

        isPaused = false;               // Resume rising after wait
        AudioManager.Instance.PlayAcidSound();
    }


    /// If the player touches the acid, trigger Game Over in GameManager.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
