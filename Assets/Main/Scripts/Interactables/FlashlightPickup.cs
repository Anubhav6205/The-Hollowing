using TMPro; // ✅ Important: Add this for TextMeshPro support
using UnityEngine;

/// <summary>
/// Handles the pickup logic for the flashlight object in the game.
/// Enables flashlight usage, triggers UI messages, phone ringing, and flashlight visuals.
/// </summary>
public class FlashlightPickup : MonoBehaviour
{
    [Tooltip("Manager for showing on-screen messages")]
    public MessageUIManager msgUI;

    [Tooltip("UI prompt shown when near the flashlight (e.g., 'Press G to pick up')")]
    public TextMeshProUGUI pickupPromptText;

    // References
    private Flashlight flashlightScript;
    private PhonePickup phonePickup;

    // Internal flag to track if player is near
    private bool isNear = false;

    /// <summary>
    /// Finds required components and hides pickup prompt at the start.
    /// </summary>
    void Start()
    {
        flashlightScript = FindObjectOfType<Flashlight>();
        phonePickup = FindObjectOfType<PhonePickup>();
        pickupPromptText.enabled = false;
    }

    /// <summary>
    /// Listens for pickup input while the player is near the flashlight.
    /// </summary>
    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.G))
        {
            PickupFlashlight();
        }
    }

    /// <summary>
    /// Called when the player picks up the flashlight.
    /// Enables usage, triggers events, starts phone ringing, and destroys the flashlight object.
    /// </summary>
    private void PickupFlashlight()
    {
        // Show pickup message
        msgUI?.ShowMessage("Flashlight picked up");

        // Hide pickup prompt
        pickupPromptText.enabled = false;

        // Start phone ringing after flashlight is picked up
        if (phonePickup != null)
            phonePickup.StartRinging();

        // Enable flashlight functionality
        if (flashlightScript != null)
        {
            flashlightScript.canUseFlashlight = true;
            EventManager.RaiseFlashlightPickedUp();

            // Turn on flashlight immediately
            flashlightScript.on = true;
            flashlightScript.off = false;
            EventManager.RaiseFlashlightToggled(true);
            flashlightScript.flashlight.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Flashlight script not found!");
        }

        // Remove the pickup object (assumed to be one level above)
        Destroy(transform.parent.gameObject);
    }

    /// <summary>
    /// Enables pickup prompt when player enters the trigger zone.
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
    /// Disables pickup prompt when player leaves the trigger zone.
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
