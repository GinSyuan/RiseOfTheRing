
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("=== Main Menu Panels & Background ===")]
    public GameObject mainMenuBackground;

    public GameObject mainMenuUI;

    public GameObject mainControlsUI;

    public GameObject mainSettingsUI;

    [Header("=== Pause Menu Panels ===")]
    public GameObject pauseMenuUI;

    public GameObject pauseControlsUI;

    public GameObject pauseSettingsUI;

    [Header("=== Game Over Panel ===")]
    public GameObject gameOverUI;

    public TextMeshProUGUI scoreText;

    [Header("=== BGM Volume Slider ===")]
    public Slider bgmVolumeSlider;

    // Internal state tracking
    private bool _isPaused = false;
    private bool _sceneLoaded = false;

    private void Awake()
    {
        // Ensure this UIManager is not destroyed when changing scenes
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // 1) Show the main menu (background + buttons) and pause the game
        ShowMainMenu();

        // 2) Play background music immediately (if AudioManager is already in the UI scene)
        if (AudioManager.Instance != null && AudioManager.Instance.bgmClip != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmClip, loop: true);
        }

        // 3) If a BGM slider is assigned in Inspector, initialize and bind its callback
        if (bgmVolumeSlider != null)
        {
            // Setting this value will fire OnBGMVolumeChange once; but we null‐check inside
            bgmVolumeSlider.value = (AudioManager.Instance != null)
                                   ? AudioManager.Instance.bgmVolume
                                   : 0.5f;

            // Bind the volume‐change callback. Whenever the slider value changes, OnBGMVolumeChange(float) is called.
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChange);
        }
    }

    private void Update()
    {
        // Only allow pressing Esc to toggle pause/resume if SampleScene is loaded
        if (_sceneLoaded && Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    /// <summary>
    /// Show the main menu: display background + main menu buttons, hide all other panels, pause game logic.
    /// </summary>
    public void ShowMainMenu()
    {
        // Ensure the background is visible
        if (mainMenuBackground != null)
            mainMenuBackground.SetActive(true);

        // Show main menu buttons
        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);

        // Hide the Controls subpanel (in case it was open)
        if (mainControlsUI != null)
            mainControlsUI.SetActive(false);

        // Hide the Settings subpanel (in case it was open)
        if (mainSettingsUI != null)
            mainSettingsUI.SetActive(false);

        // Hide any pause menu panels
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        if (pauseControlsUI != null)
            pauseControlsUI.SetActive(false);
        if (pauseSettingsUI != null)
            pauseSettingsUI.SetActive(false);

        // Hide the Game Over panel
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        // Pause game logic
        Time.timeScale = 0f;
        _isPaused = false;
    }

    /// <summary>
    /// Called by the "Start" button: hide main menu buttons and background,
    /// resume game logic, and load SampleScene in Additive mode.
    /// </summary>
    public void StartGame()
    {
        // Disable the Main Menu UI and its background so nothing from UI_Scene remains visible:
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        if (mainMenuBackground != null)
            mainMenuBackground.SetActive(false);

        // Also hide any subpanels (just in case):
        if (mainControlsUI != null)
            mainControlsUI.SetActive(false);
        if (mainSettingsUI != null)
            mainSettingsUI.SetActive(false);

        // Resume normal gameplay time
        Time.timeScale = 1f;
        _isPaused = false;

        // Finally, load the SampleScene additively (if not already loaded)
        if (!_sceneLoaded)
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
            _sceneLoaded = true;
        }
    }

    /// <summary>
    /// Called by the "Controls" button in the main menu: hide main menu buttons, show controls subpanel.
    /// </summary>
    public void ShowMainControls()
    {
        // Do NOT disable the background—keep it active so the controls panel sits on top.
        // Instead, only disable the mainMenuUI panel (the one with the buttons).
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        // Hide Settings subpanel (in case it was open)
        if (mainSettingsUI != null)
            mainSettingsUI.SetActive(false);

        // Show the Controls subpanel under the same background
        if (mainControlsUI != null)
            mainControlsUI.SetActive(true);
    }

    /// <summary>
    /// Called by the "Back" button inside the main menu's Controls: return to the main menu.
    /// </summary>
    public void BackFromMainControls()
    {
        // Simply call ShowMainMenu to re‐enable background + main menu buttons, and hide subpanels
        ShowMainMenu();
    }

    /// <summary>
    /// Called by the "Settings" button in the main menu: hide main menu buttons and show settings subpanel.
    /// </summary>
    public void ShowMainSettings()
    {
        // Hide the main menu buttons (but keep the background active)
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        // Hide Controls subpanel (in case it was open)
        if (mainControlsUI != null)
            mainControlsUI.SetActive(false);

        // Show the Settings subpanel under the same background
        if (mainSettingsUI != null)
            mainSettingsUI.SetActive(true);
    }

    /// <summary>
    /// Called by the "Back" button inside the main menu's Settings: return to the main menu.
    /// </summary>
    public void BackFromMainSettings()
    {
        // Return to the main menu by calling ShowMainMenu
        ShowMainMenu();
    }

    /// <summary>
    /// Pause the game: show the pause menu panel and pause time.
    /// </summary>
    public void PauseGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        if (pauseControlsUI != null)
            pauseControlsUI.SetActive(false);
        if (pauseSettingsUI != null)
            pauseSettingsUI.SetActive(false);

        Time.timeScale = 0f;
        _isPaused = true;
    }

    /// <summary>
    /// Resume the game from pause: hide pause menu and resume time.
    /// </summary>
    public void ResumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        if (pauseControlsUI != null)
            pauseControlsUI.SetActive(false);
        if (pauseSettingsUI != null)
            pauseSettingsUI.SetActive(false);

        Time.timeScale = 1f;
        _isPaused = false;
    }

    /// <summary>
    /// Called by the "Controls" button in the pause menu: show the pause controls subpanel.
    /// </summary>
    public void ShowPauseControls()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        if (pauseControlsUI != null)
            pauseControlsUI.SetActive(true);
    }

    /// <summary>
    /// Called by the "Settings" button in the pause menu: show the pause settings subpanel.
    /// </summary>
    public void ShowPauseSettings()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        if (pauseSettingsUI != null)
            pauseSettingsUI.SetActive(true);
    }

    /// <summary>
    /// Called by a "Back" button inside the pause subpanels: return to the main pause menu.
    /// </summary>
    public void BackToPauseMenu()
    {
        PauseGame();
    }

    /// <summary>
    /// Called by GameManager in SampleScene when the game ends:
    /// show the Game Over panel, display final height, and pause time.
    /// </summary>
    public void ShowGameOver()
    {
        // Hide any pause UI
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        if (pauseControlsUI != null)
            pauseControlsUI.SetActive(false);
        if (pauseSettingsUI != null)
            pauseSettingsUI.SetActive(false);

        // Show Game Over panel
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Display final height from PlayerPrefs
        float finalHeight = PlayerPrefs.GetFloat("LastHeight", 0f);
        if (scoreText != null)
            scoreText.text = $"Height: {finalHeight:F2}";

        // Pause game logic
        Time.timeScale = 0f;
        _isPaused = false;
    }

    /// <summary>
    /// Called by the "Main Menu" button in the Game Over screen:
    /// unload SampleScene and return to the main menu.
    /// </summary>
    public void BackToMainMenuFromGameOver()
    {
        if (_sceneLoaded)
        {
            SceneManager.UnloadSceneAsync("SampleScene");
            _sceneLoaded = false;
        }

        ShowMainMenu();
    }

    /// <summary>
    /// Called by the "Replay" button in the Game Over screen:
    /// reset game logic and reload SampleScene.
    /// </summary>
    public void ReplayGame()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.ResetGameLogic();

        BackToMainMenuFromGameOver();
    }

    /// <summary>
    /// Called by the Quit button: exit the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // Only visible in the Unity Editor
    }

    /// <summary>
    /// Callback for when the BGM volume slider changes (float version).
    /// Unity automatically passes the slider’s current value (0.0f–1.0f).
    /// </summary>
    /// <param name="volume">The new slider value (0–1).</param>
    public void OnBGMVolumeChange(float volume)
    {
        // If AudioManager is not yet initialized (SampleScene not loaded), do nothing
        if (AudioManager.Instance == null)
            return;

        // Otherwise, set the background music volume in AudioManager
        AudioManager.Instance.SetBGMVolume(volume);
    }
}
