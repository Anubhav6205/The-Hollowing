using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>
/// Plays the outro video and allows skipping with the Space key.
/// Transitions to the next scene automatically on video end or manual skip.
/// </summary>
public class OutroManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Text that shows skip or loading prompts")]
    public TextMeshProUGUI skipText;

    [Tooltip("Message shown when skipping is available")]
    public string normalPrompt = "Press Space to skip";

    [Tooltip("Message shown while loading next scene")]
    public string loadingPrompt = "Loading...";

    [Tooltip("Assigned VideoPlayer playing the outro")]
    public VideoPlayer _vp;

    private bool _hasEnded = false;

    void Awake()
    {
        // Get the VideoPlayer on this GameObject if not assigned
        if (_vp == null)
            _vp = GetComponent<VideoPlayer>();

        _vp.loopPointReached += OnVideoFinished;

        // Show initial skip prompt
        if (skipText != null)
            skipText.text = normalPrompt;
    }

    void Start()
    {
        _vp.Play();
    }

    void Update()
    {
        if (_hasEnded)
            return;

        // Allow player to skip the outro
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndOutro();
        }
    }

    /// <summary>
    /// Called automatically when video finishes.
    /// </summary>
    void OnVideoFinished(VideoPlayer vp)
    {
        EndOutro();
    }

    /// <summary>
    /// Ends the outro and loads the next scene.
    /// </summary>
    void EndOutro()
    {
        if (_hasEnded)
            return;

        _hasEnded = true;

        if (skipText != null)
            skipText.text = loadingPrompt;

        SceneManager.LoadScene(2); // Adjust this index to match your end scene
    }
}
