using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the player's death sequence: disables controls, triggers fall, fades screen,
/// displays blinking "Game Over" text, and reloads the scene on input.
/// </summary>
public class DeathManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Full-screen black Image for fade")]
    public Image backgroundFade;

    [Tooltip("Text that says 'YOU DIED…'")]
    public GameObject gameOverText;

    [Header("Timings")]
    [Tooltip("Seconds to fade to black")]
    public float fadeDuration = 3f;

    [Tooltip("Seconds to wait after fade before blinking")]
    public float postFadeDelay = 0f;

    [Tooltip("How fast to blink the text")]
    public float blinkInterval = 5f;

    [Tooltip("Reference to the PlayerFallOnDeath component to trigger the fall animation on death")]
    public PlayerFallOnDeath playerFallOnDeath;

    // Internal references and state tracking
    [Tooltip("Cached TextMeshProUGUI component for controlling the game-over text appearance")]
    private TextMeshProUGUI _textComp;

    [Tooltip("Whether the blink text is currently visible")]
    private bool _blinkVisible;

    [Tooltip("Accumulated time since last blink toggle")]
    private float blinkTimer = 0f;

    [Tooltip("Toggles between different text variants on each blink")]
    private bool textShuffle = true;

    [Tooltip("Prevents the death sequence from starting multiple times")]
    private bool _deathRoutineStarted = false;

    /// <summary>
    /// Sets up references and hides UI elements before gameplay begins.
    /// </summary>
    void Awake()
    {
        // Hide fade background and reset its transparency
        if (backgroundFade != null)
        {
            backgroundFade.gameObject.SetActive(false);
            backgroundFade.color = Color.clear;
        }

        // Cache the TextMeshPro component and hide the game-over text object
        if (gameOverText != null)
        {
            _textComp = gameOverText.GetComponentInChildren<TextMeshProUGUI>();
            gameOverText.SetActive(false);
        }
    }

    /// <summary>
    /// Begins the death sequence once, preventing duplicate invocations.
    /// </summary>
    public void Die()
    {
        if (_deathRoutineStarted)
            return;

        _deathRoutineStarted = true;
        StartCoroutine(DeathRoutine());
    }

    /// <summary>
    /// Handles disabling the player, fading to black, blinking text, and reloading the scene on input.
    /// </summary>
    IEnumerator DeathRoutine()
    {
        // 1) Disable player movement
        var player = FindObjectOfType<SimpleMovement>();
        if (player != null)
            player.isDead = true;

        // 2) Disable flashlight
        var torch = FindObjectOfType<Flashlight>();
        if (torch != null)
            torch.isDead = true;

        // 3) Trigger player fall animation
        if (playerFallOnDeath != null)
            playerFallOnDeath.StartFalling();

        // 4) Fade screen to black over fadeDuration seconds
        if (backgroundFade != null)
        {
            backgroundFade.gameObject.SetActive(true);
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float a = Mathf.Clamp01(t / fadeDuration);
                backgroundFade.color = new Color(0, 0, 0, a);
                yield return null;
            }
            backgroundFade.color = new Color(0, 0, 0, 1);
        }

        // 5) Optional pause so the player can absorb the hit
        yield return new WaitForSeconds(postFadeDelay);

        // 6) Show and initialize blinking game-over text
        if (_textComp != null)
        {
            gameOverText.SetActive(true);
            _blinkVisible = true;
            textShuffle = true;
            _textComp.alpha = 1f;
        }

        // 7) Blink loop: toggle visibility and update text until any key is pressed
        while (!Input.anyKeyDown)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInterval)
            {
                blinkTimer = 0f;
                _blinkVisible = !_blinkVisible;
                textShuffle = !_blinkVisible;
                UpdateBlinkText();
                _textComp.alpha = _blinkVisible ? 1f : 0f;
            }
            yield return null; // wait one frame
        }

        // 8) Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Updates the content of the blinking text based on the current shuffle state.
    /// </summary>
    void UpdateBlinkText()
    {
        if (_textComp == null)
            return;

        if (textShuffle)
            _textComp.text =
                "<color=red>I WARNED YOU</color>\n<size=30><color=white> </color></size>";
        else
            _textComp.text =
                "<color=red>I WARNED YOU</color>\n<size=30><color=white>PRESS ANY KEY TO RESTART</color></size>";
    }
}
