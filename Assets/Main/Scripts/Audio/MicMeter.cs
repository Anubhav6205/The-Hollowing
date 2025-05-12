using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Displays a real-time microphone loudness meter using UI bars,
/// monitors for scream/loud peaks, and triggers a Game Over sequence if threshold is exceeded.
/// </summary>
public class MicMeter : MonoBehaviour
{
    [Tooltip("Reference to MicManager for accessing mic loudness")]
    public MicManager mic;

    [Tooltip("Loudness value above which the meter turns red")]
    public float dangerThreshold = 1.5f;

    [Tooltip("Loudness value at which Game Over is triggered")]
    public float gameOverThreshold = 3.0f;

    [Tooltip("Bar color when mic input is safe")]
    public Color normalColor = Color.green;

    [Tooltip("Bar color when mic input is dangerously loud")]
    public Color dangerColor = Color.red;

    [Tooltip("Maximum bar width (currently unused)")]
    public float maxWidth = 200f;

    [Tooltip("GameObject to display 'Game Over' message")]
    public GameObject gameOverText;

    [Tooltip("Black screen fade image on Game Over")]
    public Image backgroundFade;

    [Tooltip("Parent of all bar images used to display loudness visually")]
    public Transform micMeterBarsParent;

    // Cached image bars from micMeterBarsParent
    private Image[] micBarImages;

    // Internal state: meter UI and blinking text
    private RectTransform meterRect;
    private bool gameOver = false;

    // Cached reference to TMP text inside gameOverText
    private TextMeshProUGUI textComponent;

    // Tracks whether blinking text is visible or not
    private bool blinkVisible = true;

    [Tooltip("Is the mic loudness system currently active?")]
    private bool micActive = false;

    [Tooltip("Reference to the falling animation trigger script")]
    public PlayerFallOnDeath fallScript;

    [Tooltip("Reference to the death manager that controls the fade + text + reload")]
    public DeathManager deathManager;

    /// <summary>
    /// Initializes UI elements and fetches references at the start of the scene.
    /// </summary>
    void Start()
    {
        // Load saved mic threshold from calibration (or fallback to default)
        gameOverThreshold = PlayerPrefs.GetFloat("MicGameOverThreshold", gameOverThreshold);
        Debug.Log(gameOverThreshold);

        // Setup game-over UI text component
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
            textComponent = gameOverText.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
                textComponent.alpha = 0f;
        }

        // Hide and reset black screen
        if (backgroundFade != null)
        {
            backgroundFade.gameObject.SetActive(false);
            backgroundFade.color = new Color(0, 0, 0, 0);
        }

        // Cache all bar images and hide the mic UI
        if (micMeterBarsParent != null)
        {
            micMeterBarsParent.gameObject.SetActive(false);
            micBarImages = micMeterBarsParent.GetComponentsInChildren<Image>(true);
        }
    }

    /// <summary>
    /// Continuously monitors mic input and updates the UI bars.
    /// Triggers Game Over if loudness exceeds threshold.
    /// </summary>
    void Update()
    {
        if (!micActive)
            return;

        if (!gameOver)
        {
            if (mic == null || micBarImages == null)
                return;

            float loudness = mic.GetLoudness();
            UpdateBars(loudness);

            if (loudness >= gameOverThreshold)
            {
                TriggerGameOver();
            }
        }
    }

    /// <summary>
    /// Lights up mic meter bars proportionally based on loudness.
    /// </summary>
    /// <param name="loudness">Current loudness from mic</param>
    void UpdateBars(float loudness)
    {
        if (micBarImages == null)
            return;

        // Calculate how many bars to light up
        int activeBars = Mathf.RoundToInt((loudness / gameOverThreshold) * micBarImages.Length);

        for (int i = 0; i < micBarImages.Length; i++)
        {
            if (i < activeBars)
            {
                // Active bars: white or red based on loudness level
                micBarImages[i].color = (loudness >= dangerThreshold) ? Color.red : Color.white;
            }
            else
            {
                // Inactive bars: transparent white
                micBarImages[i].color = new Color(1f, 1f, 1f, 0.2f);
            }
        }
    }

    /// <summary>
    /// Triggers the death sequence when the player is too loud.
    /// </summary>
    public void TriggerGameOver()
    {
        gameOver = true;
        deathManager.Die();
    }

    /// <summary>
    /// Toggles blinking Game Over text (currently unused).
    /// </summary>
    void UpdateBlinkText()
    {
        if (textComponent == null)
            return;

        if (blinkVisible)
            textComponent.text =
                "<color=red>I WARNED YOU</color>\n<size=30><color=white>PRESS ANY KEY TO RESTART</color></size>";
        else
            textComponent.text =
                "<color=red>YOU SHOULD HAVE LISTENED</color>\n<size=30><color=white> </color></size>";
    }

    /// <summary>
    /// Enables mic loudness detection and shows the UI meter.
    /// </summary>
    public void ActivateMicSystem()
    {
        micActive = true;
        if (micMeterBarsParent != null)
            micMeterBarsParent.gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables mic detection and hides the loudness meter UI.
    /// </summary>
    public void DeactivateMicSystem()
    {
        micActive = false;
        if (micMeterBarsParent != null)
            micMeterBarsParent.gameObject.SetActive(false);
    }
}
