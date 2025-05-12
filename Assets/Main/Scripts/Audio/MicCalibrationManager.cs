using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles microphone threshold calibration UI:
/// captures mic loudness, updates calibration bars based on threshold slider,
/// persists threshold value, and loads the game scene on continue.
/// </summary>
public class MicCalibrationManager : MonoBehaviour
{
    [Header("References")]

    [Tooltip("Reference to the MicManager component for getting microphone loudness")]
    public MicManager mic;

    [Tooltip("Parent transform containing calibration bar images")]
    public Transform barsParent;

    [Tooltip("UI slider used to set the loudness threshold")]
    public Slider thresholdSlider;

    [Tooltip("Button that advances to the main game")]
    public Button continueButton;

    [Tooltip("Text displayed while the game scene is loading")]
    public TextMeshProUGUI loadingText;

    // Internal cache of all bar images under barsParent, used for the live meter
    private Image[] _bars;

    /// <summary>
    /// Initialize references, hide loading text, set up event listeners,
    /// and cache bar images.
    /// </summary>
    void Awake()
    {
        // Hide loading text until continue is clicked
        loadingText.gameObject.SetActive(false);

        // Fetch all Image components under barsParent (including inactive ones)
        _bars = barsParent.GetComponentsInChildren<Image>(true);

        // Save threshold immediately whenever the slider value changes
        thresholdSlider.onValueChanged.AddListener(OnThresholdChanged);

        // Proceed to the game scene when Continue is clicked
        continueButton.onClick.AddListener(OnContinue);
    }

    /// <summary>
    /// Called when the threshold slider value changes;
    /// saves the new threshold to PlayerPrefs.
    /// </summary>
    /// <param name="newValue">The updated threshold slider value</param>
    void OnThresholdChanged(float newValue)
    {
        // Persist the selected threshold value
        PlayerPrefs.SetFloat("MicGameOverThreshold", newValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Updates the calibration meter each frame based on current mic loudness
    /// relative to the threshold.
    /// </summary>
    void Update()
    {
        // Get current loudness from mic
        float loud = mic.GetLoudness();

        // Calculate how many bars should be active
        int activeCount = Mathf.RoundToInt((loud / thresholdSlider.value) * _bars.Length);

        // Update each bar's color: red if active, faded otherwise
        for (int i = 0; i < _bars.Length; i++)
        {
            _bars[i].color = (i < activeCount)
                ? Color.red
                : new Color(1f, 1f, 1f, 0.2f);
        }
    }

    /// <summary>
    /// Shows the loading text and loads the main game scene.
    /// </summary>
    void OnContinue()
    {
        // Display loading indicator
        loadingText.gameObject.SetActive(true);

        // Load scene with build index 4 (main game)
        SceneManager.LoadScene(4);
    }
}
