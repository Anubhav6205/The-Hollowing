using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the flashlight behavior:
/// • Turns ON/OFF with flicker animation
/// • Drains battery based on BatteryHUD
/// • Prevents use when dead or battery is empty
/// • Can enter endless flicker mode when player dies
/// </summary>
public class Flashlight : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The flashlight GameObject to toggle")]
    public GameObject flashlight;

    [Tooltip("Sound to play when flashlight turns on")]
    public AudioSource turnOn;

    [Tooltip("Sound to play when flashlight turns off")]
    public AudioSource turnOff;

    [Tooltip("Reference to the BatteryHUD for battery level tracking")]
    public BatteryHUD batteryHUD;

    [Header("State")]
    [Tooltip("True if flashlight is currently on")]
    public bool on = false;

    [Tooltip("True if flashlight is currently off")]
    public bool off = true;

    [Tooltip("If true, flashlight is flickering (during on/off or death)")]
    private bool isFlickering = false;

    [Tooltip("If false, flashlight input is ignored")]
    public bool canUseFlashlight = false;

    [Tooltip("If true, flashlight enters infinite flicker mode (e.g., on death)")]
    public bool isDead = false;

    void Start()
    {
        flashlight.SetActive(false);
    }

    void Update()
    {
        // Skip all input if not usable or already flickering due to death
        if (!canUseFlashlight || (isDead && !isFlickering))
            return;

        // Enter endless flicker if dead
        if (isDead)
        {
            StartCoroutine(FlickerAndDie());
            return;
        }

        // Force shutoff if battery is empty
        if (batteryHUD.CurrentFill <= 0f)
        {
            if (flashlight.activeSelf)
                ShutdownFlashlight();
            return;
        }

        // Handle flashlight toggle input
        if (off && Input.GetKeyDown(KeyCode.F) && !isFlickering)
            StartCoroutine(FlickerAndTurnOn());
        else if (on && Input.GetKeyDown(KeyCode.F) && !isFlickering)
            ShutdownFlashlight();
    }

    /// <summary>
    /// Turns the flashlight off and notifies listeners.
    /// </summary>
    void ShutdownFlashlight()
    {
        flashlight.SetActive(false);
        if ( // only if component is still enabled...
            turnOff != null
            && turnOff.enabled
            // …and its GameObject is active…
            && turnOff.gameObject.activeInHierarchy
        )
        {
            turnOff?.Play();
        }
        off = true;
        on = false;
        EventManager.RaiseFlashlightToggled(false);
    }

    /// <summary>
    /// Coroutine to flicker the flashlight before turning it on.
    /// </summary>
    IEnumerator FlickerAndTurnOn()
    {
        EventManager.RaiseFlashlightToggled(true);
        isFlickering = true;
        turnOn?.Play();

        int flickers = Random.Range(2, 4);
        for (int i = 0; i < flickers; i++)
        {
            flashlight.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));
            flashlight.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));
        }

        flashlight.SetActive(true);
        on = true;
        off = false;
        isFlickering = false;
    }

    /// <summary>
    /// Coroutine for endless flickering when flashlight is in 'dead' state.
    /// </summary>
    IEnumerator FlickerAndDie()
    {
        isFlickering = true;
        while (true)
        {
            flashlight.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            flashlight.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }
}
