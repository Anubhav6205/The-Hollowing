using UnityEngine;

/// <summary>
/// Triggers a dolly-based cutscene when the player enters this zone.
/// Disables player movement during the cutscene and re-enables it afterward.
/// Optionally starts a ghost action and pops a navigation waypoint.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CutsceneTrigger : MonoBehaviour
{
    [Tooltip("Drives the VCam dolly + priority swap")]
    public CMCutscene cutScene;

    [Tooltip("Your player movement script to disable/re-enable")]
    public MonoBehaviour simpleMovement;

    [Tooltip("Ghost logic to trigger when cutscene starts")]
    public EnemyChaseWithDelay ghost;

    [Tooltip("Waypoint ID to pop after cutscene ends")]
    public string popWaypoint;

    // Tracks whether the waypoint has been popped already
    private bool _hasPopped = false;

    /// <summary>
    /// Ensures the collider is set as a trigger.
    /// </summary>
    void Awake()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    /// <summary>
    /// Initiates the cutscene sequence when the player enters the trigger.
    /// Disables player movement and begins camera + ghost action.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Disable this trigger to prevent repeated activation
        GetComponent<Collider>().enabled = false;

        // Disable player controls
        if (simpleMovement != null)
            simpleMovement.enabled = false;

        // Trigger ghost action and start the cutscene
        ghost.BeginActionSequence();
        cutScene.StartCutscene();
    }

    /// <summary>
    /// Called by CMCutscene when the camera path finishes.
    /// Re-enables player control and triggers a navigation waypoint (once).
    /// </summary>
    public void EndCutscene()
    {
        Debug.Log("poped");
        Debug.Log(popWaypoint);

        if (!_hasPopped)
        {
            Debug.Log("CUTSCENE TRIGGER POPPED");
            EventManager.PopWaypoint(popWaypoint);
            _hasPopped = true;
        }

        if (simpleMovement != null)
            simpleMovement.enabled = true;
    }
}
