using System;
using UnityEngine;

/// <summary>
/// Struct that binds a specific artifact ID to its corresponding AudioSource.
/// Used to trigger contextual audio when a certain artifact is collected.
/// </summary>
[Serializable]
public struct ArtifactAudioBinding
{
    [Tooltip("Which artifact ID will trigger this entry")]
    public int artifactId;

    [Tooltip("The AudioSource to play for this artifact")]
    public AudioSource audioSource;
}

/// <summary>
/// Listens for artifact pickup events and plays audio associated with the picked artifact ID.
/// Ensures audio doesn't play on awake and starts only on matching ID.
/// </summary>
public class ArtifactAudioManager : MonoBehaviour
{
    [Tooltip("List of artifact ID and audio source bindings")]
    [SerializeField]
    private ArtifactAudioBinding[] bindings;

    /// <summary>
    /// On Awake, stop all linked audio sources and ensure they do not play on awake.
    /// </summary>
    private void Awake()
    {
        foreach (var b in bindings)
        {
            if (b.audioSource != null)
            {
                b.audioSource.playOnAwake = false;
                b.audioSource.Stop();
            }
        }
    }

    /// <summary>
    /// Subscribes to the artifact picked event when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        EventManager.OnArtifactPicked += HandleArtifactPicked;
    }

    /// <summary>
    /// Unsubscribes from the artifact picked event when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        EventManager.OnArtifactPicked -= HandleArtifactPicked;
    }

    /// <summary>
    /// Plays the corresponding audio when an artifact with a matching ID is picked up.
    /// </summary>
    /// <param name="pickedId">ID of the picked artifact</param>
    private void HandleArtifactPicked(int pickedId)
    {
        foreach (var b in bindings)
        {
            if (b.artifactId == pickedId && b.audioSource != null)
            {
                b.audioSource.Play();
            }
        }
    }
}
