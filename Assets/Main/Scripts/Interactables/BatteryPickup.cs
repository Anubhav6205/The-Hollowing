using TMPro;
using UnityEngine;

/// <summary>
/// Handles the pickup logic for flashlight batteries.
/// Displays a pickup prompt, plays audio, sends relevant events,
/// and destroys the battery object after collection.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BatteryPickup : MonoBehaviour
{
    [Header("UI Prompt")]
    [Tooltip("Prompt shown when player is near (e.g., 'Press G to pick up')")]
    [SerializeField]
    private TextMeshProUGUI pickupPrompt;

    [Header("Messages (optional)")]
    [Tooltip("Message UI manager to show pickup notifications")]
    [SerializeField]
    private MessageUIManager msgUI;

    [Tooltip("Sound to play when battery is picked up")]
    public AudioSource batteryPickupSound;

    // Internal flag to track if player is within pickup range
    private bool isNear = false;

    // Static counter for how many batteries the player has collected
    private static int s_batteryCount = 1;

    /// <summary>
    /// Hides the pickup prompt on game start.
    /// </summary>
    void Start()
    {
        pickupPrompt.enabled = false;
    }

    void Awake()
    {
        BatteryPickup.ResetBatteryCount(); // <─ new helper
    }

    /// <summary>
    /// Checks for interaction input when the player is near.
    /// </summary>
    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.G))
            Collect();
    }

    /// <summary>
    /// Called when the player picks up the battery.
    /// Notifies systems, plays sound, updates count, and destroys the battery object.
    /// </summary>
    private void Collect()
    {
        // Notify any objects like blinkers that this battery has been collected
        var blinker = GetComponentInChildren<ObjectBlinkLight>();
        EventManager.RaiseBatteryCollected(blinker);

        // Play pickup sound
        if (batteryPickupSound != null)
            batteryPickupSound.Play();

        // Show pickup message
        msgUI?.ShowMessage("Battery collected");

        // Increment global battery count and notify HUD/UI systems
        s_batteryCount++;
        EventManager.RaiseBatteryCountChanged(s_batteryCount);

        // Hide the prompt and destroy this object
        pickupPrompt.enabled = false;
        Destroy(gameObject);
    }

    /// <summary>
    /// Shows the pickup prompt when the player enters range.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = true;
            pickupPrompt.enabled = true;
        }
    }

    public static void ResetBatteryCount() => s_batteryCount = 1;

    /// <summary>
    /// Hides the pickup prompt when the player leaves range.
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            pickupPrompt.enabled = false;
        }
    }
}
