using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls enemy chase behavior based on different action types (delayed chase, slow intro, burst sprint).
/// Handles NavMesh navigation, audio feedback, and player death triggering.
/// </summary>
public class EnemyChaseWithDelay : MonoBehaviour
{
    public enum ActionType
    {
        StandAndChase = 1,         // wait, then chase at constant speed
        SlowForwardThenChase,      // creep forward during delay, then chase
        BurstChase,                // chase with speed bursts after delay
    }

    [Header("References")]
    [Tooltip("The NavMeshAgent used for movement")]
    public NavMeshAgent agent;

    [Tooltip("Player transform (usually tagged 'Player')")]
    public Transform player;

    [Tooltip("The visual GameObject for the ghost/enemy")]
    public GameObject chaserObject;

    [Header("Detection Ranges")]
    [Tooltip("How far the ghost can detect the player")]
    public float sightRange = 100f;

    [Tooltip("Distance at which the ghost kills the player")]
    public float attackRange = 5f;

    [Tooltip("What layer the player belongs to")]
    public LayerMask whatIsPlayer;

    [Header("Animation / Delay Settings")]
    [Tooltip("How long to wait before chasing begins")]
    public float wakeDelay = 3f;

    [Tooltip("Type of chase behavior to follow")]
    public ActionType actionId = ActionType.StandAndChase;

    [Header("Movement Speeds")]
    public float chaseSpeed = 5f;
    public float slowSpeed = 2f;

    [Header("Burst Settings (BurstChase only)")]
    public float burstSpeed = 10f;
    public float burstNormalDuration = 4f;
    public float burstDuration = 10f;

    [Header("Audio Sources")]
    [Tooltip("Growl played once when the ghost wakes up")]
    public AudioSource wakeGrowl;

    [Tooltip("Growl loop during chase")]
    public AudioSource chaseGrowl;

    [Tooltip("Footstep loop during chase")]
    public AudioSource footsteps;

    [Tooltip("Growl played when in attack range")]
    public AudioSource attackGrowl;

    [Tooltip("Optional ambient or chase sound")]
    public AudioSource chaseSound;

    [Tooltip("Death manager to trigger death sequence on player")]
    public DeathManager deathManager;

    // Internal state
    private bool _canChase = false;

    void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        agent.speed = chaseSpeed;
    }

    void Start()
    {
        if (chaserObject != null)
            chaserObject.SetActive(false);

        agent.enabled = true;
    }

    /// <summary>
    /// Starts the ghost's behavior based on the configured action type.
    /// </summary>
    public void BeginActionSequence()
    {
        if (wakeGrowl != null)
            wakeGrowl.Play();

        if (chaserObject != null)
            chaserObject.SetActive(true);

        _canChase = false;
        agent.enabled = true;

        switch (actionId)
        {
            case ActionType.StandAndChase:
                StartCoroutine(StandThenChase());
                break;
            case ActionType.SlowForwardThenChase:
                StartCoroutine(SlowForwardThenChase());
                break;
            case ActionType.BurstChase:
                StartCoroutine(BurstChaseSequence());
                break;
        }
    }

    /// <summary>
    /// Immediately stops all movement, audio, and visuals related to the ghost.
    /// </summary>
    public void StopChase()
    {
        _canChase = false;
        agent.enabled = false;

        if (chaserObject != null)
            chaserObject.SetActive(false);

        chaseSound?.Stop();
        wakeGrowl?.Stop();
        chaseGrowl?.Stop();
        footsteps?.Stop();
        attackGrowl?.Stop();
    }

    /// <summary>
    /// Waits for wakeDelay, then enables chase mode.
    /// </summary>
    private IEnumerator StandThenChase()
    {
        yield return new WaitForSeconds(wakeDelay);
        _canChase = true;
    }

    /// <summary>
    /// Creeps forward slowly during wakeDelay, then switches to chase mode.
    /// </summary>
    private IEnumerator SlowForwardThenChase()
    {
        agent.speed = slowSpeed;
        float t = 0f;
        while (t < wakeDelay && agent.enabled)
        {
            t += Time.deltaTime;
            agent.SetDestination(transform.position + transform.forward * 5f);
            yield return null;
        }
        agent.speed = chaseSpeed;
        _canChase = true;
    }

    /// <summary>
    /// Alternates between normal and burst chase speeds in a loop.
    /// </summary>
    private IEnumerator BurstChaseSequence()
    {
        yield return new WaitForSeconds(wakeDelay);
        _canChase = true;

        while (_canChase && agent.enabled)
        {
            agent.speed = chaseSpeed;
            yield return new WaitForSeconds(burstNormalDuration);

            agent.speed = burstSpeed;
            yield return new WaitForSeconds(burstDuration);
        }

        agent.speed = chaseSpeed;
    }

    /// <summary>
    /// Checks for distance to player and updates behavior accordingly.
    /// </summary>
    void Update()
    {
        if (!_canChase)
            return;

        float dist = Vector3.Distance(transform.position, player.position);
        Debug.Log(dist);
        Debug.Log(_canChase);

        if (dist <= attackRange)
        {
            KillPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    /// <summary>
    /// Plays chase audio and continuously updates the agent's destination to follow the player.
    /// </summary>
    private void ChasePlayer()
    {
        Debug.Log("chase player");

        if (chaseSound != null && !chaseSound.isPlaying)
            chaseSound.Play();

        if (chaseGrowl != null && !chaseGrowl.isPlaying)
            chaseGrowl.Play();

        if (footsteps != null && !footsteps.isPlaying)
            footsteps.Play();

        if (attackGrowl != null && attackGrowl.isPlaying)
            attackGrowl.Stop();

        agent.SetDestination(player.position);
    }

    /// <summary>
    /// Triggers the player's death and plays the attack sound.
    /// </summary>
    private void KillPlayer()
    {
        Debug.Log("player killed");

        if (attackGrowl != null && !attackGrowl.isPlaying)
            attackGrowl.Play();

        if (chaseGrowl != null && chaseGrowl.isPlaying)
            chaseGrowl.Stop();

        if (footsteps != null && footsteps.isPlaying)
            footsteps.Stop();

        if (deathManager)
            deathManager.Die();
    }
}
