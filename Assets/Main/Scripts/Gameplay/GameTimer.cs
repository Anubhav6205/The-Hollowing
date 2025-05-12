using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Countdown timer that starts on player movement. When it reaches zero, it triggers death.
/// Can be started with delay and supports stopping manually (e.g. on game end).
/// </summary>
public class GameTimer : MonoBehaviour
{
    [Tooltip("UI Text to display the timer")]
    public TextMeshProUGUI timerText;

    [Tooltip("Initial time in seconds (default is 6 minutes = 360 seconds)")]
    public float startingTime = 360f;

    // Internal timer tracking
    private float currentTime;

    // Cached reference to movement script (used to check for player activity)
    private SimpleMovement playerMovement;

    // Tracks whether the timer is currently counting down
    private bool timerActive = false;

    [Tooltip("Reference to DeathManager that handles death logic when time runs out")]
    public DeathManager deathManager;

    // Prevents multiple death triggers
    private bool _deathTriggered = false;

    /// <summary>
    /// Initializes timer and hides it at the beginning of the game.
    /// </summary>
    void Start()
    {
        currentTime = startingTime;

        // Auto-locate movement controller in scene
        playerMovement = FindObjectOfType<SimpleMovement>();

        // Hide timer text initially
        if (timerText != null)
            timerText.enabled = false;
    }

    /// <summary>
    /// Updates the timer each frame. Starts counting when the player moves.
    /// Triggers death when countdown reaches zero.
    /// </summary>
    void Update()
    {
        if (!timerActive)
            return;

        if (playerMovement == null)
            return;

        // Detect any movement input
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        // Only count down when player moves
        if (isMoving && currentTime > 0f)
        {
            currentTime -= Time.deltaTime;

            // Clamp and handle zero-time event
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                timerActive = false;

                // Trigger death only once
                if (!_deathTriggered && deathManager != null)
                {
                    _deathTriggered = true;
                    deathManager.Die();
                }
            }
        }

        // Update the on-screen display each frame
        UpdateTimerDisplay();
    }

    /// <summary>
    /// Starts the timer after a given delay.
    /// Use when you want a short break before countdown begins (e.g., intro fades).
    /// </summary>
    /// <param name="delay">Time to wait before activating the timer</param>
    public IEnumerator DelayedStartTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartTimer();
    }

    /// <summary>
    /// Formats and updates the timer UI text.
    /// Displays real-world date followed by remaining time (MM:SS).
    /// </summary>
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        string date = System.DateTime.Now.ToString("MMM dd yyyy").ToUpper();
        timerText.text = date + " " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Starts or resumes the timer countdown.
    /// </summary>
    public void StartTimer()
    {
        timerActive = true;

        if (timerText != null)
        {
            UpdateTimerDisplay();
            timerText.enabled = true;
        }
    }

    /// <summary>
    /// Stops the timer countdown and hides the timer display.
    /// Useful when ending or resetting the game.
    /// </summary>
    public void StopTimer()
    {
        timerActive = false;

        if (timerText != null)
            timerText.enabled = false;
    }
}
