using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuUI;
    public GameObject mainControlsUI;
    public GameObject mainSettingsUI;

    [Header("Pause Menu Panels")]
    public GameObject pauseMenuUI;
    public GameObject pauseControlsUI;
    public GameObject pauseSettingsUI;

    [Header("Game Over Panel")]
    public GameObject gameOverUI;
    public TextMeshProUGUI scoreText;

    [Header("Audio")]
    public AudioSource musicSource;
    public Slider musicSlider;

    private bool _isPaused = false;

    private void Start()
    {
        ShowMainMenu();
        Time.timeScale = 1f;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ShowMainMenu()
    {
        gameOverUI.SetActive(false);
        mainMenuUI.SetActive(true);
        mainControlsUI.SetActive(false);
        mainSettingsUI.SetActive(false);

        pauseMenuUI.SetActive(false);
        pauseControlsUI.SetActive(false);
        pauseSettingsUI.SetActive(false);

        Time.timeScale = 1f;
        _isPaused = false;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Ring");
    }

    public void ShowMainControls()
    {
        mainMenuUI.SetActive(false);
        mainControlsUI.SetActive(true);
    }
    
    public void ShowMainSettings()
    {
        mainMenuUI.SetActive(false);
        mainSettingsUI.SetActive(true);
    }

    public void onMusicVolumeChange()
    {
        musicSource.volume = musicSlider.value;
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        pauseControlsUI.SetActive(false);
        pauseSettingsUI.SetActive(false);

        Time.timeScale = 0f;
        _isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        pauseControlsUI.SetActive(false);
        pauseSettingsUI.SetActive(false);

        Time.timeScale = 1f;
        _isPaused = false;
    }

    public void ShowPauseControls()
    {
        pauseMenuUI.SetActive(false);
        pauseControlsUI.SetActive(true);
    }

    public void ShowPauseSettings()
    {
        pauseMenuUI.SetActive(false);
        pauseSettingsUI.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        PauseGame();
    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;

        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        if (scoreText != null)
            scoreText.text = $"..........";
    }

    public void ReplayGame()
    {
        PlayerPrefs.SetInt("FinalScore", 0); // Optional: clear score
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // For editor testing
    }

}
