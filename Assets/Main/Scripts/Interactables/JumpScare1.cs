using System.Collections;
using UnityEngine;

/// <summary>
/// Trigger-based jump scare component:
/// • Activates a hidden scare object when player enters trigger
/// • Optionally plays a sound
/// • Moves the object (or relies on root motion)
/// • Destroys itself after a delay
/// • Duration can be tweaked in-game using + / - keys
/// </summary>
[RequireComponent(typeof(Collider))]
public class JumpScare1 : MonoBehaviour
{
    [Header("Scare Object")]
    [Tooltip("Object that appears and moves toward the player")]
    [SerializeField] private GameObject jumpscareObject;

    [Header("One-shot SFX (optional)")]
    [Tooltip("Audio clip that plays once when scare triggers")]
    [SerializeField] private AudioSource scareAudio;

    [Header("Movement (ignored if Use Root Motion is true)")]
    [Tooltip("If true, uses Animator root motion instead of manual movement")]
    [SerializeField] private bool useRootMotion = false;

    [Tooltip("Speed at which the object moves (if not using root motion)")]
    [SerializeField] private float speed = 4f;

    [Tooltip("Direction to move the object (in local space)")]
    [SerializeField] private Vector3 localMoveDir = Vector3.forward;

    [Header("Duration (seconds)")]
    [Tooltip("Time before the object and trigger are destroyed")]
    [SerializeField, Min(0.1f)] private float destroyDelay = 1.5f;

    [Header("In-game Adjust Keys (optional)")]
    [Tooltip("Key to increase the delay time")]
    [SerializeField] private KeyCode longerKey = KeyCode.Equals;

    [Tooltip("Key to decrease the delay time")]
    [SerializeField] private KeyCode shorterKey = KeyCode.Minus;

    [Tooltip("Step size when adjusting delay")]
    [SerializeField] private float stepSize = 0.5f;

    [Tooltip("Minimum allowed delay time")]
    [SerializeField] private float minDelay = 0.5f;

    // Internals
    private Animator _anim;
    private bool _moving;
    private bool _scarePlayed;

    /// <summary>
    /// Prepares the scare object and disables it at startup.
    /// </summary>
    private void Awake()
    {
        if (jumpscareObject != null)
        {
            jumpscareObject.SetActive(false);
            _anim = jumpscareObject.GetComponent<Animator>();
            if (_anim) _anim.applyRootMotion = useRootMotion;
        }
    }

    /// <summary>
    /// Handles movement if enabled and delay-time adjustment keys before activation.
    /// </summary>
    private void Update()
    {
        if (!_scarePlayed)
            HandleDurationInput();

        if (!_moving || jumpscareObject == null || useRootMotion)
            return;

        // Move object manually if root motion is disabled
        jumpscareObject.transform.Translate(
            localMoveDir.normalized * speed * Time.deltaTime,
            Space.Self);
    }

    /// <summary>
    /// Activates the jumpscare when the player enters the trigger zone.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (_scarePlayed || !other.CompareTag("Player") || jumpscareObject == null)
            return;

        _scarePlayed = true;

        // Show scare object and play sound
        jumpscareObject.SetActive(true);
        if (scareAudio != null) scareAudio.Play();

        // Begin movement and destruction countdown
        _moving = true;
        StartCoroutine(DestroyAfterDelay());
    }

    /// <summary>
    /// Allows designers/testers to tweak how long the jumpscare lasts using hotkeys.
    /// </summary>
    private void HandleDurationInput()
    {
        if (Input.GetKeyDown(longerKey))
            destroyDelay += stepSize;

        if (Input.GetKeyDown(shorterKey))
            destroyDelay = Mathf.Max(minDelay, destroyDelay - stepSize);
    }

    /// <summary>
    /// Destroys the scare object and trigger after the configured delay.
    /// </summary>
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);

        if (jumpscareObject != null)
            Destroy(jumpscareObject);

        Destroy(gameObject); // also remove the trigger itself
    }
}
