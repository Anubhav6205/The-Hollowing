using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Alternates between two tip/info messages on a UI TextMeshPro element every few seconds.
/// Useful for intro/outro screens to cycle helpful or fun facts.
/// </summary>
public class TipAlternator : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Text component where tips will be shown")]
    public TextMeshProUGUI textComponent;

    [Header("Messages")]
    [TextArea]
    [Tooltip("Primary tip message to show first")]
    public string tipText = "Tip: For the fullest story experience, we recommend watching the entire intro without skipping.";

    [TextArea]
    [Tooltip("Secondary info message to alternate with")]
    public string infoText = "Do you know? The Hollowing's story and assets were crafted with ChatGPT, SORA, ElevenLabs, HailuoAI and more.";

    [Header("Timing")]
    [Tooltip("Time in seconds between switching the messages")]
    public float switchInterval = 5f;

    private bool _showingTip = true;

    void Awake()
    {
        // Auto-assign text component if not set
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        // Start the alternating tip loop
        StartCoroutine(AlternateTexts());
    }

    /// <summary>
    /// Coroutine that swaps tip/info messages at regular intervals.
    /// </summary>
    IEnumerator AlternateTexts()
    {
        while (true)
        {
            textComponent.text = _showingTip ? tipText : infoText;
            _showingTip = !_showingTip;
            yield return new WaitForSeconds(switchInterval);
        }
    }
}
