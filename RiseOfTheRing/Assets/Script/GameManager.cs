using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    public TextMeshProUGUI timerText;                   // UI Text to display remaining time

    [Header("Life Icons")]
    public Image[] lifeIcons;       // Array of SpriteRenderers for heart icons

    [Header("Game Over & Max Height")]
    public TextMeshProUGUI heightText;                  // UI Text to display max height reached
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
        // Hide game over UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Initialize lives and timer at game start
        currentLives = maxLives;
        timer = timeLimit;
        UpdateLivesUI();   // Display all life icons initially
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (player.position.y > maxHeight)
        {
            maxHeight = player.position.y;
        }

        heightText.text = "Height: " + maxHeight.ToString("F2");

        timer -= Time.deltaTime;
        UpdateTimerUI();

        if (timer <= 0f)
        {
            GameOver();
        }
    }


    /// Called when the player takes damage (e.g., collides with an enemy).
    /// Decrements life count and updates UI. Triggers Game Over if lives reach zero.
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


    /// Updates the visibility of heart-icon SpriteRenderers
    /// based on the current number of lives.
    private void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            // Show the icon if its index is less than currentLives, hide otherwise
            lifeIcons[i].enabled = (i < currentLives);
        }
    }


    /// Updates the UI text showing remaining time, using ceiling of the timer value.
    private void UpdateTimerUI()
    {
        timerText.text = "Timer: " + Mathf.Ceil(timer).ToString();
    }


    /// Activates the Game Over panel, displays max height, and pauses the game.
    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        heightText.text = "Height: " + maxHeight.ToString("F2");
        Time.timeScale = 0f;  // Pause the game
    }


    /// Optional method to retrieve the current number of lives.
    public int GetCurrentLives()
    {
        return currentLives;
    }
}
