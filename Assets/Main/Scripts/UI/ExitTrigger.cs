using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Trigger zone that loads a new scene when the player enters it.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ExitTrigger : MonoBehaviour
{
    void Awake()
    {
        // Ensure the collider is set as a trigger
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react to the player
        if (!other.CompareTag("Player"))
            return;

        // Disable collider to prevent re-triggering
        GetComponent<Collider>().enabled = false;

        // Load scene with build index 6 (adjust as needed)
        SceneManager.LoadScene(6);
    }
}
