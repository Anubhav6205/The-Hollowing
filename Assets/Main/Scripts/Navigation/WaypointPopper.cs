using UnityEngine;

/// <summary>
/// Triggers a waypoint pop event when the player enters the zone.
/// Each instance is tied to a specific key (e.g., "D1", "D3") and only triggers once.
/// </summary>
public class WaypointPopper : MonoBehaviour
{
    [Tooltip("Key this waypoint represents, e.g. D1, D3 …")]
    public string waypointKey = "D1";

    // Internal flag to prevent double-popping
    private bool _hasPopped = false;

    /// <summary>
    /// When the player enters the trigger, raise a waypoint event and disable this script.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("has popped? ");
        Debug.Log(_hasPopped);

        if (_hasPopped)
            return;

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("WAYPOINT POPPED");

        EventManager.PopWaypoint(waypointKey);
        _hasPopped = true;

        // Disable script to prevent re-use (or you could use Destroy(gameObject) if desired)
        Destroy(this);
    }
}
