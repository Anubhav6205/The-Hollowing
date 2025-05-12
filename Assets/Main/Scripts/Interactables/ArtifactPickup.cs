using TMPro;
using UnityEngine;

/// <summary>
/// Represents a collectible artifact in the game. Shows a pickup prompt when near,
/// sends events on collection, shows a message, and destroys itself.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ArtifactPickup : MonoBehaviour
{
    [Header("Inspector Settings")]
    [Tooltip("Unique ID assigned to this artifact (e.g., 1 for truck, 2 for ribbon, etc.)")]
    [SerializeField]
    private int artifactID;

    [Tooltip("Prompt shown to player when in range (e.g., 'Press G to collect')")]
    [SerializeField]
    private TextMeshProUGUI pickupPrompt;

    [Header("Messages (optional)")]
    [Tooltip("UI manager to show a collection message")]
    [SerializeField]
    private MessageUIManager msgUI;

    // Internal flag to detect if player is within pickup range
    private bool isNear = false;

    /// <summary>
    /// Hides the pickup prompt on game start.
    /// </summary>
    void Start() => pickupPrompt.enabled = false;

    /// <summary>
    /// Checks for input while the player is near the artifact.
    /// </summary>
    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.G))
            Collect();
    }

    /// <summary>
    /// Called when the player collects the artifact.
    /// Sends global events, shows UI messages, and destroys the artifact object.
    /// </summary>
    private void Collect()
    {
        // Display context-specific pickup message
        string message = artifactID switch
        {
            1 => "Lilly's toy truck collected!",
            2 => "Found Lilly's ribbon!",
            3 => "Lilly's doll collected!",
            4 => "Found Lilly's pendant!",
            5 => "Got Lilly's favourite wind chimes!",
            _ => "Unknown artifact.",
        };

        // Pop a waypoint based on which artifact was picked
        if (artifactID == 1)
        {
            Debug.Log("artifact 1 popped");
            EventManager.PopWaypoint("D2");
        }
        else if (artifactID == 2)
        {
            Debug.Log("artifact 2 popped");
            EventManager.PopWaypoint("D6");
        }
        else if (artifactID == 3)
        {
            Debug.Log("artifact 3 popped");
            EventManager.PopWaypoint("D11");
        }
        else if (artifactID == 4)
        {
            Debug.Log("artifact 4 popped");
            EventManager.PopWaypoint("D15");
        }
        else if (artifactID == 5)
        {
            Debug.Log("artifact 5 popped");
            EventManager.PopWaypoint("D22");
        }

        // Show the pickup message on screen
        msgUI?.ShowMessage(message);

        // Notify any listeners that an artifact has been picked
        EventManager.ArtifactPicked(artifactID);

        // Hide the prompt and destroy the root object (in case model is nested)
        pickupPrompt.enabled = false;
        var root = transform.parent != null ? transform.parent.gameObject : gameObject;
        Destroy(root);
    }

    /// <summary>
    /// Enables the pickup prompt when the player enters range.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = true;
            pickupPrompt.enabled = true;
        }
    }

    /// <summary>
    /// Disables the pickup prompt when the player leaves range.
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNear = false;
            pickupPrompt.enabled = false;
        }
    }
}
