using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles main menu button interactions:
/// • Play: loads the gameplay scene
/// • Options: opens options panel
/// • Quit: exits the application
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Tooltip("Name of the gameplay scene to load when Play is pressed")]
    public string playSceneName = "Level1";

    [Tooltip("Options menu panel to show/hide")]
    public GameObject optionsPanel;

    /// <summary>
    /// Called when the Play button is pressed.
    /// Loads the gameplay scene.
    /// </summary>
    public void OnPlayPressed()
    {
        Debug.Log("play");
        SceneManager.LoadScene(playSceneName); // You may also use build index if preferred
    }

    /// <summary>
    /// Called when the Options button is pressed.
    /// Displays the options menu.
    /// </summary>
    public void OnOptionsPressed()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    /// <summary>
    /// Called when the Quit button is pressed.
    /// Exits the game or stops play mode if in Unity Editor.
    /// </summary>
    public void OnQuitPressed()
    {
        Debug.Log("quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
