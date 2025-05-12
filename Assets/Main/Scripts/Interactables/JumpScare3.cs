using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Jump-scare trigger that spawns a ghost using NavMeshAgent to run toward the player,
/// plays a scream sound, and deactivates itself after a brief dramatic delay.
/// </summary>
[RequireComponent(typeof(Collider))]
public class JumpScare3 : MonoBehaviour
{
    [Header("Ghost & Movement")]
    [Tooltip("Assign a disabled ghost GameObject in the scene")]
    public GameObject ghostObject;

    [Tooltip("How fast the NavMeshAgent should run")]
    public float runSpeed = 20f;

    [Tooltip("Stop when within this distance of player")]
    public float stopDistance = 0.5f;

    [Header("Audio")]
    [Tooltip("Jumpscare audio clip (AudioSource on this GameObject)")]
    public AudioSource scareClip;

    [Tooltip("How long after the ghost hits you before cleanup")]
    public float postDelay = 1f;

    // Internal state
    private AudioSource _audio;
    private Transform _player;
    private NavMeshAgent _agent;
    private bool _triggered = false;

    /// <summary>
    /// Caches required references and disables the ghost at startup.
    /// </summary>
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = ghostObject.GetComponent<NavMeshAgent>();
        ghostObject.SetActive(false); // ensure ghost is hidden initially
    }

    /// <summary>
    /// When the player enters the trigger, starts the scare sequence.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player"))
            return;

        _triggered = true;
        GetComponent<Collider>().enabled = false;

        StartCoroutine(RunThroughPlayer());
    }

    /// <summary>
    /// Coroutine that activates, moves, and then disables the ghost.
    /// </summary>
    IEnumerator RunThroughPlayer()
    {
        // 1) Activate the ghost
        ghostObject.SetActive(true);

        // 2) Set NavMeshAgent to chase the player
        _agent.speed = runSpeed;
        _agent.isStopped = false;
        _agent.SetDestination(_player.position);

        // 3) Play scare audio
        if (scareClip != null)
            scareClip.Play();

        // 4) Optional: screen flash, disable player controls, etc.

        // 5) Wait for postDelay seconds before cleanup
        yield return new WaitForSeconds(postDelay);

        // 6) Stop and hide the ghost
        _agent.isStopped = true;
        ghostObject.SetActive(false);

        // 7) Stop audio
        if (scareClip != null)
            scareClip.Stop();

        // 8) Optionally re-enable player control here if needed
    }
}
