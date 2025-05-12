using System.Collections;
using UnityEngine;

/// <summary>
/// Trigger-zone jump-scare that:
/// • Plays a scream sound
/// • Activates a jump-scare camera and flash image
/// • Temporarily disables the player rig
/// • Restores everything after a short delay
/// </summary>
[RequireComponent(typeof(Collider))]
public class JumpScare2 : MonoBehaviour
{
    [Header("Scene References")]
    [Tooltip("Scream sound effect")]
    public AudioSource scream;

    [Tooltip("Root object of the player controller (usually includes the main camera)")]
    public GameObject player;

    [Tooltip("Camera used for the jumpscare (starts inactive)")]
    public GameObject jumpCam;

    [Tooltip("White flash image UI shown during the scare")]
    public GameObject flashImg;

    [Tooltip("Ghost model or rig to briefly show during the scare")]
    public GameObject ghostModel;

    [Header("Settings")]
    [Tooltip("Duration in seconds the jumpscare effect lasts")]
    public float scareDuration = 2f;

    [Tooltip("Disable this trigger permanently after firing once")]
    public bool disableAfterUse = true;

    /// <summary>
    /// Ensures all scare elements are disabled at start.
    /// </summary>
    void Awake()
    {
        if (jumpCam)
            jumpCam.SetActive(false);
        if (flashImg)
            flashImg.SetActive(false);
        if (ghostModel)
            ghostModel.SetActive(false);

        // Ensure this collider is marked as a trigger
        GetComponent<Collider>().isTrigger = true;
    }

    /// <summary>
    /// Handles the jumpscare logic when the player enters the trigger.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Play scream sound
        if (scream)
            scream.Play();

        // Activate scare visuals
        if (ghostModel)
            ghostModel.SetActive(true);
        if (jumpCam)
            jumpCam.SetActive(true);
        if (player)
            player.SetActive(false);
        if (flashImg)
            flashImg.SetActive(true);

        // Start restore sequence
        StartCoroutine(RestoreAfterDelay());

        if (disableAfterUse)
        {
            // Disable only the collider and this script to prevent retriggers
            GetComponent<Collider>().enabled = false;
            enabled = false;
        }
    }

    /// <summary>
    /// Waits for the scare duration, then restores normal player state and removes the scare object.
    /// </summary>
    private IEnumerator RestoreAfterDelay()
    {
        yield return new WaitForSeconds(scareDuration);

        if (ghostModel)
            ghostModel.SetActive(false);
        if (player)
            player.SetActive(true);
        if (jumpCam)
            jumpCam.SetActive(false);
        if (flashImg)
            flashImg.SetActive(false);

        // Remove the entire trigger object after use
        Destroy(gameObject);
    }
}
