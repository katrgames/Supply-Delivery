using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    public GameObject settingsPanel; // Assign in Inspector
    public Slider volumeSlider; // Assign in Inspector
    public Button restartButton,
        closeButton;

    private bool isPaused = false;

    public AudioSource audioSource; // Assign in Inspector
    public AudioClip backgroundMusic; // Assign in Inspector

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // Loops music
            audioSource.Play();
        }
        // Load saved volume
        volumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 1f);
        audioSource.volume = volumeSlider.value;

        // Add listeners
        volumeSlider.onValueChanged.AddListener(SetVolume);
        restartButton.onClick.AddListener(RestartGame);
        closeButton.onClick.AddListener(ToggleSettings);

        // Hide settings initially
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        // Open settings with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        isPaused = !isPaused;
        settingsPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1; // Pause/resume game
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Ensure game runs normally
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
