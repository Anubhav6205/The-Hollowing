using UnityEngine;

/// <summary>
/// Listens for the final artifact pickup and triggers end-game visuals and audio:
/// changes the skybox, disables fog, plays a bright sound, and disables mic/timer systems.
/// </summary>
public class EndSequenceManager : MonoBehaviour
{
    [Tooltip("Skybox Material to switch to once the final artifact is picked")]
    public Material finalSkybox;

    [Tooltip("Bright audio to play when the final artifact is picked")]
    public AudioSource brightAudio;

    /// <summary>
    /// Subscribes to the OnArtifactPicked event when enabled.
    /// </summary>
    void OnEnable()
    {
        EventManager.OnArtifactPicked += HandleArtifactPicked;
    }

    /// <summary>
    /// Unsubscribes from the OnArtifactPicked event when disabled.
    /// </summary>
    void OnDisable()
    {
        EventManager.OnArtifactPicked -= HandleArtifactPicked;
    }

    /// <summary>
    /// Called when any artifact is picked. If it's the final one, triggers the end-game sequence.
    /// </summary>
    /// <param name="id">ID of the picked artifact</param>
    void HandleArtifactPicked(int id)
    {
        Debug.Log("Artifact " + id + " picked");

        // When the 5th artifact is picked, trigger the finale
        if (id == 5)
        {
            Debug.Log("Final artifact picked");

            // 1) Swap the skybox to the brighter version
            if (finalSkybox != null)
            {
                RenderSettings.skybox = finalSkybox;

#if UNITY_2018_1_OR_NEWER
                // Update the global illumination if applicable
                DynamicGI.UpdateEnvironment();
#endif
            }

            // 2) Disable fog for a clear, bright environment
            RenderSettings.fog = false;

            // 3) Play the bright end-game audio cue
            if (brightAudio != null)
                brightAudio.Play();

            // 4) Stop the game timer, if it exists
            var gm = FindObjectOfType<GameTimer>();
            gm?.StopTimer();

            // 5) Deactivate mic scream detection system
            var mm = FindObjectOfType<MicMeter>();
            mm?.DeactivateMicSystem();
        }
    }
}
