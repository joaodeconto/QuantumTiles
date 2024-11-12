using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip winSound;
    public AudioClip introSound;

    [Header("Audio Source")]
    private AudioSource audioSource;

    private void Awake()
    {
        // Initialize the audio source component
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        GameManager.OnGameOver += PlayWinSound;
        GameManager.OnMatch += PlayMatchSound;
        GameManager.OnMismatch += PlayMismatchSound;
        GameManager.OnFlip += PlayFlipSound;

        //PlayBGSound();
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= PlayWinSound;
        GameManager.OnMatch -= PlayMatchSound;
        GameManager.OnMismatch -= PlayMismatchSound;
        GameManager.OnFlip -= PlayFlipSound;

    }

    // Method to play the flip sound
    public void PlayFlipSound()
    {
        PlaySound(flipSound);
    }

    // Method to play the match sound
    public void PlayMatchSound()
    {
        PlaySound(matchSound);
    }

    // Method to play the mismatch sound
    public void PlayMismatchSound()
    {
        PlaySound(mismatchSound);
    }

    // Method to play the win sound
    public void PlayWinSound()
    {
        PlaySound(winSound);
    }

    // Method to play the intro sound
    public void PlayBGSound()
    {
        PlaySound(introSound);
    }

    // Private helper method to play a sound clip
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
