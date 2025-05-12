using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>
/// Plays the intro video and allows the player to skip it with the Space key.
/// Automatically transitions to the main menu after the video finishes or is skipped.
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
public class IntroManager : MonoBehaviour
{
    [Header("Scene Setup")]
    [Tooltip("Name of the scene to load when the intro ends or is skipped")]
    public string sceneToLoad = "Scene_MainMenu";

    [Header("UI")]
    [Tooltip("Text element showing skip/loading prompts")]
    public TextMeshProUGUI skipText;

    [Tooltip("What to show when the player can skip")]
    public string normalPrompt = "Press Space to skip";

    [Tooltip("What to show once skipping or video end triggers load")]
    public string loadingPrompt = "Loading...";

    private VideoPlayer _videoPlayer;
    private bool _hasEnded = false;

    void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.loopPointReached += OnVideoFinished;

        if (skipText != null)
            skipText.text = normalPrompt;
    }

    void Update()
    {
        if (_hasEnded)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerLoad();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        TriggerLoad();
    }

    /// <summary>
    /// Loads the main menu scene and updates the UI.
    /// </summary>
    void TriggerLoad()
    {
        if (_hasEnded)
            return;

        _hasEnded = true;

        if (skipText != null)
            skipText.text = loadingPrompt;

        Debug.Log("moving");
        SceneManager.LoadScene(sceneToLoad);
    }
}
