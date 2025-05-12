using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>
/// Plays a splash screen video and automatically transitions to the next scene when it ends.
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
public class SplashScreenManager : MonoBehaviour
{
    void Start()
    {
        // Get the VideoPlayer component and hook up the event
        var vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += OnVideoFinished;
        vp.Play();
    }

    /// <summary>
    /// Called when the splash video finishes playing.
    /// </summary>
    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(1); // Load the next scene (index 1). Adjust if needed.
    }
}
