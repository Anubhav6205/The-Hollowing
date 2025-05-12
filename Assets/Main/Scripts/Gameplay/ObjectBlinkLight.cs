using System.Collections;
using UnityEngine;

/// <summary>
/// Handles blinking light behavior for collectible objects like batteries or artifacts.
/// The light blinks when the object is active and visible, and turns off once collected.
/// </summary>
public class ObjectBlinkLight : MonoBehaviour
{
    [Tooltip("The Light component that should blink")]
    [SerializeField]
    private Light blinkLight;

    [Tooltip("How fast the light blinks (in seconds)")]
    [SerializeField]
    private float blinkInterval = 0.5f;

    // Reference to the coroutine handling the blink loop
    private Coroutine blinkRoutine;

    /// <summary>
    /// Subscribes to global events and disables the object/light on startup.
    /// </summary>
    void Awake()
    {
        // Subscribed early to handle events even if initially inactive
        EventManager.PhonePickedUp += OnPhonePickedUp;
        EventManager.BatteryCollected += OnBatteryCollected;

        // Hide object and disable light until activated
        gameObject.SetActive(false);
        if (blinkLight != null)
            blinkLight.enabled = false;
    }

    /// <summary>
    /// Ensures event cleanup when the object is destroyed.
    /// </summary>
    void OnDestroy()
    {
        EventManager.PhonePickedUp -= OnPhonePickedUp;
        EventManager.BatteryCollected -= OnBatteryCollected;
    }

    /// <summary>
    /// Called when the phone is picked up. Makes the object visible and starts the blinking light.
    /// </summary>
    private void OnPhonePickedUp()
    {
        gameObject.SetActive(true);
        blinkRoutine = StartCoroutine(BlinkLightRoutine());
    }

    /// <summary>
    /// Called when any battery is collected. Disables the blinking light if it's this object.
    /// </summary>
    /// <param name="collectedOne">The specific ObjectBlinkLight that was picked up</param>
    private void OnBatteryCollected(ObjectBlinkLight collectedOne)
    {
        // Only respond if this exact object was collected
        if (collectedOne != this)
            return;

        // Stop blinking and disable light
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        if (blinkLight != null)
            blinkLight.enabled = false;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Coroutine that toggles the light on and off at regular intervals.
    /// </summary>
    private IEnumerator BlinkLightRoutine()
    {
        while (true)
        {
            if (blinkLight != null)
                blinkLight.enabled = !blinkLight.enabled;

            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
