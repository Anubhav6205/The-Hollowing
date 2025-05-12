using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Displays a temporary UI message when an item is picked up.
/// Automatically hides the message after a short delay.
/// </summary>
public class PickupUIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel to show/hide on pickup")]
    public GameObject CollectedMessage;

    [Tooltip("Text component that displays the item name")]
    public TextMeshProUGUI CollectedMessageText;

    [Header("Settings")]
    [Tooltip("Duration the message stays visible")]
    public float displayDuration = 2f;

    void Start()
    {
        // Ensure message is hidden at the start
        if (CollectedMessage != null)
            CollectedMessage.SetActive(false);
    }

    /// <summary>
    /// Call this to show a pickup notification.
    /// </summary>
    /// <param name="itemName">Name of the item collected</param>
    public void ShowPickupMessage(string itemName)
    {
        if (CollectedMessage == null || CollectedMessageText == null)
            return;

        CollectedMessageText.text = $"{itemName} picked up";
        CollectedMessage.SetActive(true);

        // Begin coroutine to hide message after delay
        StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// Hides the message after the configured delay.
    /// </summary>
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        CollectedMessage.SetActive(false);
    }
}
