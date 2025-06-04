using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;      // Singleton instance

    [Header("Lives Settings")]
    public int maxLives = 3;                 // Maximum number of lives the player can have
    private int currentLives;                // Current number of lives remaining

    [Header("Timer Settings")]
    public float timeLimit = 60f;            // Game time limit in seconds
    private float timer;                     // Internal timer counter
    public TextMeshProUGUI timerText;        // UI Text field to display remaining time

    [Header("Life Icons")]
    public Image[] lifeIcons;                // Array of UI images representing lives

    [Header("Height Display")]
    public TextMeshProUGUI heightText;       // UI Text field to display the highest height reached
    public Transform player;                 // Reference to the player's Transform for height tracking
    private float maxHeight;                 // The highest Y position reached by the player

    private void Awake()
    {
        // Implement singleton pattern: if no instance exists, assign this; otherwise destroy duplicates
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Initialize lives and timer when the scene starts
        currentLives = maxLives;
        timer = timeLimit;
        UpdateLivesUI();
    }

    private void Update()
    {
        // If the game is paused (Time.timeScale == 0), do not run game logic
        if (Time.timeScale == 0f)
            return;

        // Update the maximum height if the player has climbed higher
        if (player.position.y > maxHeight)
            maxHeight = player.position.y;
        heightText.text = "Height: " + maxHeight.ToString("F2"); // Display the height with two decimal places

        // Decrease the timer and update the timer UI
        timer -= Time.deltaTime;
        UpdateTimerUI();

        // If time runs out, trigger Game Over
        if (timer <= 0f)
            GameOver();
    }


    /// Called when the player takes damage: decrement lives, update UI, and check for Game Over.
    public void TakeDamage()
    {
        if (currentLives <= 0)
            return;

        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
            GameOver();
    }


    /// Update the life icons to reflect the current number of lives.
    private void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].enabled = (i < currentLives);
    }


    /// Update the timer text to show the remaining seconds (rounded up).
    private void UpdateTimerUI()
    {
        timerText.text = "Timer: " + Mathf.Ceil(timer).ToString();
    }


    /// Handle Game Over: stop acid sound, save final height, notify UIManager, and pause the game.
    public void GameOver()
    {
        // Stop the continuous acid sound effect
        AudioManager.Instance.StopAcidSound();

        // Store the final height so UIManager can display it on the Game Over screen
        PlayerPrefs.SetFloat("LastHeight", maxHeight);

        // Find the UIManager in the scene and show the Game Over UI
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("Cannot find UIManager in the scene");
        }

        // Pause all game logic
        Time.timeScale = 0f;
    }


    /// Called by UIManager.ReplayGame(): reset lives, timer, and height, and update the UI.
    public void ResetGameLogic()
    {
        currentLives = maxLives;
        timer = timeLimit;
        maxHeight = 0f;
        UpdateLivesUI();
    }
}
