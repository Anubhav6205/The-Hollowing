using System;
using UnityEngine;

/// <summary>
/// Handles audio playback for "House A" environment.
/// Starts background audio when the phone is picked up,
/// stops it when a battery is collected, and triggers a one-time waypoint event.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class HouseAManager : MonoBehaviour
{
    // Cached reference to the AudioSource component
    private AudioSource src;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Caches the AudioSource and sets loop mode.
    /// </summary>
    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.loop = true;

        // Note: PlayOnAwake is assumed to be off in the inspector
    }

    /// <summary>
    /// Subscribes to global events when enabled.
    /// </summary>
    void OnEnable()
    {
        EventManager.PhonePickedUp += StartHouseAudio;
        EventManager.BatteryCollected += StopHouseAudio;
    }

    /// <summary>
    /// Unsubscribes from global events when disabled.
    /// </summary>
    void OnDisable()
    {
        EventManager.PhonePickedUp -= StartHouseAudio;
        EventManager.BatteryCollected -= StopHouseAudio;
    }

    /// <summary>
    /// Plays the looping house audio when the phone is picked up.
    /// </summary>
    private void StartHouseAudio()
    {
        if (!src.isPlaying)
            src.Play();
    }

    /// <summary>
    /// Stops the house audio after a battery is collected, and triggers a one-time waypoint popup.
    /// </summary>
    /// <param name="_">Unused ObjectBlinkLight parameter (from event)</param>
    private void StopHouseAudio(ObjectBlinkLight _)
    {
        if (src.isPlaying)
            src.Stop();
    }
}
