using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Displays a temporary UI message with fade-in, hold, and fade-out animation.
/// Destroys itself automatically after the full sequence.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MessageItem : MonoBehaviour
{
    [Tooltip("Seconds it takes to fade in")]
    public float fadeIn = 0.4f;

    [Tooltip("How long the message stays fully visible")]
    public float hold = 3f;

    [Tooltip("Seconds it takes to fade out")]
    public float fadeOut = 0.4f;

    private CanvasGroup cg;
    private TextMeshProUGUI tmp;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// Sets the message text and starts the fade sequence.
    /// </summary>
    public void Init(string message)
    {
        tmp.text = message;
        cg.alpha = 0f;
        StartCoroutine(Lifetime());
    }

    /// <summary>
    /// Handles fade-in, hold, and fade-out over time.
    /// </summary>
    IEnumerator Lifetime()
    {
        // Fade in
        for (float t = 0; t < fadeIn; t += Time.deltaTime)
        {
            cg.alpha = t / fadeIn;
            yield return null;
        }
        cg.alpha = 1f;

        // Hold message
        yield return new WaitForSeconds(hold);

        // Fade out
        for (float t = 0; t < fadeOut; t += Time.deltaTime)
        {
            cg.alpha = 1f - (t / fadeOut);
            yield return null;
        }

        Destroy(gameObject);
    }
}
