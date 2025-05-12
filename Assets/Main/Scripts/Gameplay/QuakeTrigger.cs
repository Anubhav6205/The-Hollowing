using System.Collections;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Triggers a simulated earthquake when the player enters the trigger zone:
/// shakes the camera, plays rattle audio, throws room objects, and re-freezes them once settled.
/// </summary>
[RequireComponent(typeof(Collider))]
public class QuakeTrigger : MonoBehaviour
{
    [Header("Room Pieces")]
    [Tooltip("Parent of all the chairs + table Rigidbodies")]
    public Transform piecesParent;

    [Header("Throw Settings")]
    [Tooltip("How hard to throw them")]
    public float forceMin = 5f;

    [Tooltip("Max throw force applied to objects")]
    public float forceMax = 15f;

    [Header("Timing")]
    [Tooltip("Delay before the pieces fly out")]
    public float throwDelay = 2f;

    [Tooltip("How many seconds the quake (shake + rattle) lasts")]
    public float quakeDuration = 3f;

    [Header("Audio")]
    [Tooltip("Rattle audio (AudioSource on this same GameObject)")]
    public AudioSource rattleAudio;

    [Header("Camera Shake")]
    [Tooltip("The Cinemachine Virtual Camera you want to shake")]
    public CinemachineVirtualCamera cinemachineCam;

    [Tooltip("Noise profile strength")]
    public float shakeAmplitude = 1.5f;

    [Tooltip("Noise profile frequency")]
    public float shakeFrequency = 2f;

    // Internal
    private Rigidbody[] _pieces;
    private CinemachineBasicMultiChannelPerlin _perlin;

    /// <summary>
    /// Initializes all physics and camera shake settings.
    /// Freezes all rigidbodies and disables camera shake until triggered.
    /// </summary>
    void Awake()
    {
        // 1) Cache all child rigidbodies and freeze them
        _pieces = piecesParent.GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in _pieces)
            rb.isKinematic = true;

        // 2) Get Perlin noise component for shake
        if (cinemachineCam == null)
        {
            Debug.LogError("QuakeTrigger: CinemachineCam is not assigned!", this);
        }
        else
        {
            _perlin = cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (_perlin == null)
            {
                Debug.LogError($"{cinemachineCam.name} is missing a Basic Multi-Channel Perlin noise extension!", cinemachineCam);
            }
            else if (_perlin.m_NoiseProfile == null)
            {
                Debug.LogError($"{cinemachineCam.name}'s Perlin has no Noise Profile assigned!", cinemachineCam);
            }
            else
            {
                // You may optionally reset these here:
                // _perlin.m_AmplitudeGain = 0f;
                // _perlin.m_FrequencyGain = 0f;
            }
        }
    }

    /// <summary>
    /// Starts the quake sequence when the player enters the trigger.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Prevent retriggering
        GetComponent<Collider>().enabled = false;
        StartCoroutine(QuakeCoroutine());
    }

    /// <summary>
    /// The main earthquake sequence coroutine: handles shake, sound, throw, and reset.
    /// </summary>
    IEnumerator QuakeCoroutine()
    {
        // 1) Start camera shake
        if (_perlin != null)
        {
            _perlin.m_AmplitudeGain = shakeAmplitude;
            _perlin.m_FrequencyGain = shakeFrequency;
        }

        // 2) Play rattle sound
        rattleAudio?.Play();

        // 3) Delay before throwing pieces
        yield return new WaitForSeconds(throwDelay);

        // 4) Enable physics and apply force to each piece
        foreach (var rb in _pieces)
        {
            rb.isKinematic = false;

            // Calculate outward direction with upward bias
            var dir = (rb.transform.position - piecesParent.position).normalized + Vector3.up * 0.5f;
            dir.Normalize();

            rb.AddForce(dir * Random.Range(forceMin, forceMax), ForceMode.Impulse);
        }

        // 5) Let the quake last for a few seconds
        yield return new WaitForSeconds(quakeDuration);

        // 6) Stop the rattle audio
        rattleAudio?.Stop();

        // 7) Stop camera shake
        if (_perlin != null)
        {
            _perlin.m_AmplitudeGain = 0f;
            _perlin.m_FrequencyGain = 0f;
        }

        // 8) Wait until all objects have settled (velocity close to zero)
        bool moving;
        do
        {
            moving = false;
            foreach (var rb in _pieces)
            {
                if (rb.velocity.sqrMagnitude > 0.01f)
                {
                    moving = true;
                    break;
                }
            }
            yield return null;
        } while (moving);

        // 9) Re-freeze all pieces
        foreach (var rb in _pieces)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
