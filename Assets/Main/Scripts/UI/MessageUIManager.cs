using UnityEngine;

/// <summary>
/// Manages the creation of temporary on-screen messages ("toasts").
/// Instantiates a MessageItem prefab under a container to display user messages.
/// </summary>
public class MessageUIManager : MonoBehaviour
{
    [Header("Drag in your container here")]
    [Tooltip("Parent UI container where messages will be spawned")]
    public RectTransform messageContainer;

    [Header("Drag your MessageItem prefab here")]
    [Tooltip("Prefab with the MessageItem script attached")]
    public GameObject messageItemPrefab;

    /// <summary>
    /// Spawns a new message and triggers its fade-in/out animation.
    /// </summary>
    /// <param name="text">Message to show</param>
    public void ShowMessage(string text)
    {
        if (messageContainer == null || messageItemPrefab == null)
            return;

        // Create a new message instance as a child of the container
        var go = Instantiate(messageItemPrefab, messageContainer);
        go.GetComponent<MessageItem>().Init(text);
    }
}
