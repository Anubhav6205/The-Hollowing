using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Handles logic for picking up the phone in-game.
/// Plays ringing audio, cutscene video, voiceover, activates mic & timer,
/// and broadcasts a phone pickup event.
/// </summary>
public class PhonePickup : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Pickup prompt shown when player is nearby")]
    public TextMeshProUGUI pickupPromptText;

    [Tooltip("UI manager to show message after pickup")]
    public MessageUIManager msgUI;

    [Header("Audio")]
    [Tooltip("Phone ringing audio source")]
    public AudioSource phoneRingSource;

    [Tooltip("Voice message audio source (played once)")]
    public AudioSource voiceSource;

    [Header("Cutscene")]
    [Tooltip("Video player for the monologue cutscene")]
    public VideoPlayer monologPlayer;

    [Tooltip("GameObject holding RawImage or RenderTexture display")]
    public GameObject cutsceneDisplay;

    [Header("References")]
    [Tooltip("Script that manages the game timer")]
    public GameTimer gameTimer;

    [Tooltip("Script that manages mic-based loudness detection")]
    public MicMeter micMeter;

    [Tooltip("Story manager to enable navigation system after phone pickup")]
    public StoryManager storyManager;

    private bool isNear = false;
    private bool _voicePlayed = false;

    /// <summary>
    /// Sets up initial state, hides prompt and cutscene screen.
    /// </summary>
    void Start()
    {
        pickupPromptText.enabled = false;

        if (cutsceneDisplay != null)
            cutsceneDisplay.SetActive(false);

        // Auto-locate GameTimer and MicMeter if not manually assigned
        gameTimer ??= FindObjectOfType<GameTimer>();
        micMeter ??= FindObjectOfType<MicMeter>();
    }

    /// <summary>
    /// Listens for interaction input while near the phone.
    /// </summary>
    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.G))
            StartCoroutine(PickupPhoneSequence());
    }

    /// <summary>
    /// Starts phone ringing when flashlight is picked up.
    /// </summary>
    public void StartRinging()
    {
        if (phoneRingSource != null)
        {
            phoneRingSource.loop = true;
            phoneRingSource.Play();
        }
    }

    /// <summary>
    /// Full phone pickup sequence: handles UI, voice, video, timer, and mic activation.
    /// </summary>
    private IEnumerator PickupPhoneSequence()
    {
        // Hide pickup prompt
        pickupPromptText.enabled = false;

        // Stop ringing
        if (phoneRingSource != null && phoneRingSource.isPlaying)
            phoneRingSource.Stop();

        // Play voice recording once
        if (!_voicePlayed && voiceSource != null)
        {
            voiceSource.Play();
            _voicePlayed = true;
        }

        // Show video screen
        if (cutsceneDisplay != null)
            cutsceneDisplay.SetActive(true);

        // Pause game & mute audio
        Time.timeScale = 0f;
        AudioListener.pause = true;

        // Play cutscene video
        if (monologPlayer != null)
        {
            bool done = false;
            monologPlayer.loopPointReached += _ => done = true;
            monologPlayer.Play();
            yield return new WaitUntil(() => done);
        }

        // Show message
        msgUI?.ShowMessage("Phone picked up");

        // Hide video screen & resume game
        if (cutsceneDisplay != null)
            cutsceneDisplay.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Activate mic detection and timer
        micMeter?.ActivateMicSystem();
        gameTimer?.StartTimer();
        storyManager?.ActivateNavigationSystem();

        // Notify listeners
        EventManager.RaisePhonePickedUp();

        // Remove phone object
        Destroy(transform.parent.gameObject);
    }

    /// <summary>
    /// Displays pickup prompt when player enters the trigger zone.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = true;
            pickupPromptText.enabled = true;
        }
    }

    /// <summary>
    /// Hides pickup prompt when player leaves the trigger zone.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            pickupPromptText.enabled = false;
        }
    }
}
