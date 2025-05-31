// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Lives Settings")]
    public int maxLives = 3;                 // Maximum number of lives
    private int currentLives;                // Current number of lives

    [Header("Timer Settings")]
    public float timeLimit = 60f;            // Game time limit in seconds
    private float timer;                     // Internal timer counter
    public Text timerText;                   // UI Text to display remaining time

    [Header("Life Icons")]
    public SpriteRenderer[] lifeIcons;       // Array of SpriteRenderers for heart icons

    [Header("Game Over & Max Height")]
    public Text heightText;                  // UI Text to display max height reached
    public GameObject gameOverPanel;         // Game Over UI panel
    public Transform player;                 // Reference to player Transform for height tracking

    private float maxHeight;                 // Highest Y position reached by the player

    private void Awake()
    {
        // Initialize singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize lives and timer at game start
        currentLives = maxLives;
        timer = timeLimit;
        UpdateLivesUI();   // Display all life icons initially
    }

    private void Update()
    {
        // If the game is paused (timeScale = 0), skip timer update
        if (Time.timeScale == 0f)
            return;

        // Decrease timer and update UI
        timer -= Time.deltaTime;
        UpdateTimerUI();

        // Track the highest Y position of the player
        if (player.position.y > maxHeight)
        {
            maxHeight = player.position.y;
        }

        // If time has run out, trigger Game Over
        if (timer <= 0f)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Called when the player takes damage (e.g., collides with an enemy).
    /// Decrements life count and updates UI. Triggers Game Over if lives reach zero.
    /// </summary>
    public void TakeDamage()
    {
        // Only decrement if there are lives left
        if (currentLives <= 0)
            return;

        currentLives--;
        Debug.Log($"TakeDamage(): Lives decreased to {currentLives}");
        UpdateLivesUI();

        // If lives drop to zero or below, trigger Game Over
        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Updates the visibility of heart-icon SpriteRenderers
    /// based on the current number of lives.
    /// </summary>
    private void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            // Show the icon if its index is less than currentLives, hide otherwise
            lifeIcons[i].enabled = (i < currentLives);
        }
    }

    /// <summary>
    /// Updates the UI text showing remaining time, using ceiling of the timer value.
    /// </summary>
    private void UpdateTimerUI()
    {
        timerText.text = Mathf.Ceil(timer).ToString();
    }

    /// <summary>
    /// Activates the Game Over panel, displays max height, and pauses the game.
    /// </summary>
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        heightText.text = "Height: " + maxHeight.ToString("F2");
        Time.timeScale = 0f;  // Pause the game
    }

    /// <summary>
    /// Optional method to retrieve the current number of lives.
    /// </summary>
    public int GetCurrentLives()
    {
        return currentLives;
    }
}
