using UnityEngine;

/// <summary>
/// Captures microphone input and calculates the loudness level in real-time.
/// Used for features like scream detection or voice-based triggers.
/// </summary>
public class MicManager : MonoBehaviour
{
    // Internal mic recording clip
    private AudioClip micRecord;

    // Name of the microphone device
    private string micName;

    [Tooltip("How many audio samples to analyze per frame")]
    private int sampleWindow = 128;

    /// <summary>
    /// Automatically starts recording from the first available microphone on startup.
    /// </summary>
    void Start()
    {
        // Check if any microphone device is available
        if (Microphone.devices.Length > 0)
        {
            // Use the first available microphone
            micName = Microphone.devices[0];

            // Start continuous recording (10 sec buffer, 44100 Hz sample rate)
            micRecord = Microphone.Start(micName, true, 10, 44100);
        }
    }

    /// <summary>
    /// Computes and returns the current loudness level from the mic.
    /// </summary>
    /// <returns>A float representing the mic volume (amplified)</returns>
    public float GetLoudness()
    {
        // Return 0 if mic is not recording or clip is null
        if (micRecord == null || !Microphone.IsRecording(micName))
            return 0f;

        // Prepare a sample array to read recent audio data
        float[] samples = new float[sampleWindow];

        // Get mic's current position and move back by the sample window
        int micPos = Microphone.GetPosition(micName) - sampleWindow;

        // If we don't have enough data yet, return 0
        if (micPos < 0) return 0f;

        // Read the audio data from the mic clip
        micRecord.GetData(samples, micPos);

        // Calculate peak loudness value in the sample window
        float maxVolume = 0f;
        foreach (var sample in samples)
        {
            float wavePeak = Mathf.Abs(sample);
            if (wavePeak > maxVolume)
            {
                maxVolume = wavePeak;
            }
        }

        // Apply a fixed amplification factor to boost the output value
        float amplifiedVolume = maxVolume * 15f;  // Moderate boost, not too small, not too crazy
        return amplifiedVolume;
    }
}
