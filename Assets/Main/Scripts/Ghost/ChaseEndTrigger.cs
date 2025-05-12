using UnityEngine;

/// <summary>
/// Trigger that stops a ghost chase when the player enters the zone.
/// Useful for escape areas or safe zones at the end of a pursuit.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ChaseEndTrigger : MonoBehaviour
{
    [Tooltip("Which ghost to stop when the player escapes")]
    public EnemyChaseWithDelay ghostToStop;

    /// <summary>
    /// Stops the ghost chase when the player enters this trigger.
    /// </summary>
    /// <param name="other">The collider that entered the trigger</param>
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        ghostToStop.StopChase();
    }
}
