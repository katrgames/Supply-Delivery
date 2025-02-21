using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    private AudioSource audioSource;

    [Header("Main Music")]
    public AudioClip mainMusic; // Assign this in the Inspector

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps music playing across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }


        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign and play the background music
        audioSource.clip = mainMusic;
        audioSource.loop = true; // Enable looping
        audioSource.playOnAwake = false; // Prevent auto-play before setup

        if (mainMusic != null)
        {
            audioSource.Play(); // Play music on start
        }
    }
}
