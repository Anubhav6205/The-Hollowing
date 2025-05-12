using System.Collections;
using UnityEngine;

/// <summary>
/// Plays an audio clip for a fixed duration when the player enters the trigger.
/// Automatically stops the sound after the specified time.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TimedTriggerAudio : MonoBehaviour
{
    [Tooltip("The AudioSource to play when triggered. If left empty, will grab the one on this GameObject")]
    public AudioSource audioSource;

    [Tooltip("How many seconds to play the audio before stopping")]
    public float playDuration = 2f;

    // Internal flag to prevent retriggering while audio is already playing
    private bool _isPlaying = false;

    /// <summary>
    /// Validates and prepares components on awake.
    /// </summary>
    void Awake()
    {
        // Auto-assign AudioSource if not manually set
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Warn and disable if still missing
        if (audioSource == null)
        {
            Debug.LogError($"{name}: No AudioSource found for TimedTriggerAudio");
            enabled = false;
            return;
        }

        // Ensure the collider is set as a trigger
        var col = GetComponent<Collider>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    /// <summary>
    /// Starts playing audio when the player enters the trigger zone.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Optional tag check: Uncomment if you want only the player to trigger it
        // if (!other.CompareTag("Player")) return;

        if (!_isPlaying)
            StartCoroutine(PlayForSeconds());
    }

    /// <summary>
    /// Plays the audio for a set number of seconds, then stops it.
    /// </summary>
    private IEnumerator PlayForSeconds()
    {
        _isPlaying = true;
        audioSource.Play();
        yield return new WaitForSeconds(playDuration);
        audioSource.Stop();
        _isPlaying = false;
    }
}
