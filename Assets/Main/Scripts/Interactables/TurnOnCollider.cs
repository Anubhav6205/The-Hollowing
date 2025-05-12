using UnityEngine;

/// <summary>
/// Enables the specified GameObjects when something enters the trigger.
/// All targets are disabled on start.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TurnOnCollider : MonoBehaviour 
{
    [Tooltip("These GameObjects will be enabled when something enters this trigger")]
    public GameObject[] targets;

    /// <summary>
    /// Sets the collider to trigger mode and disables all target GameObjects initially.
    /// </summary>
    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        foreach (var go in targets)
        {
            if (go != null)
                go.SetActive(false);
        }
    }

    /// <summary>
    /// Enables all target GameObjects when any object enters the trigger.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Optional: restrict to player only
        // if (!other.CompareTag("Player")) return;

        foreach (var go in targets)
        {
            if (go != null)
                go.SetActive(true);
        }

        // Optional: disable script after one-time use
        // enabled = false;
    }
}
