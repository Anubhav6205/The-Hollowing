using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages game pause and resume behavior.
/// Loads a separate pause scene (UI) additively and handles cursor state.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("Pause Settings")]
    private bool _isPaused = false;
    private const string pauseScene = "PauseMenu";

    [Header("Feedback Text (optional)")]
    [Tooltip("Optional UI text to show a temporary feedback message")]
    public TextMeshProUGUI feedbackText;

    void Awake()
    {
        // Hide feedback text at startup
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Toggle pause/resume on ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var sc = SceneManager.GetSceneByName(pauseScene);

            // Show a tip if pause scene isn't ready
            if (!sc.IsValid() || !sc.isLoaded)
                ShowTempFeedback("Current scene is loading. Please try pausing after a few seconds.");

            // Toggle pause state
            if (!_isPaused)
                Pause();
            else
                Resume();
        }
    }

    /// <summary>
    /// Pauses game time, unlocks cursor, and shows pause scene.
    /// </summary>
    void Pause()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene(pauseScene, LoadSceneMode.Additive);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isPaused = true;
    }

    /// <summary>
    /// Resumes game time, hides cursor, and unloads pause scene.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;

        var sc = SceneManager.GetSceneByName(pauseScene);
        if (sc.IsValid() && sc.isLoaded)
            SceneManager.UnloadSceneAsync(pauseScene);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isPaused = false;
    }

    /// <summary>
    /// Quits to main menu scene.
    /// </summary>
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2); // Replace with actual main menu scene index if needed
    }

    /// <summary>
    /// Shows a temporary text message in the corner.
    /// </summary>
    void ShowTempFeedback(string message)
    {
        if (feedbackText == null)
            return;

        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        StopCoroutine(nameof(HideFeedbackRoutine));
        StartCoroutine(nameof(HideFeedbackRoutine));
    }

    /// <summary>
    /// Hides feedback message after delay.
    /// </summary>
    IEnumerator HideFeedbackRoutine()
    {
        yield return new WaitForSecondsRealtime(3f);
        feedbackText.gameObject.SetActive(false);
    }
}
