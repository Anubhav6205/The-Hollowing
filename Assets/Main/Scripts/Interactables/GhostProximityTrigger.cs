using System.Collections;
using UnityEngine;

/// <summary>
/// Triggers a fade-out effect on a ghost GameObject when the player enters a proximity zone.
/// Plays an optional voice line before fading, and ensures the event occurs only once.
/// </summary>
public class GhostProximityTrigger : MonoBehaviour
{
    [Tooltip("Reference to the ghost GameObject to fade")]
    public GameObject ghostPerson;

    [Tooltip("Audio clip to play on proximity (e.g., 'Is that you?')")]
    public AudioClip callOutSound;

    // Internal flag to prevent re-triggering
    private bool triggered = false;

    /// <summary>
    /// Called when a collider enters this trigger zone.
    /// Checks if it's the player and initiates the fade-out sequence.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Play the callout sound at the trigger's position
            if (callOutSound != null)
                AudioSource.PlayClipAtPoint(callOutSound, transform.position);

            // Begin fading the ghost
            StartCoroutine(FadeOutGhost());
        }
    }

    /// <summary>
    /// Coroutine to gradually fade out the ghost's material transparency.
    /// </summary>
    IEnumerator FadeOutGhost()
    {
        if (ghostPerson == null)
            yield break;

        Renderer[] renderers = ghostPerson.GetComponentsInChildren<Renderer>();
        float fadeTime = 2f;
        float elapsed = 0f;

        // Force all materials to use transparent fade mode
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                m.SetFloat("_Mode", 2); // Fade mode
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetInt("_ZWrite", 0);
                m.DisableKeyword("_ALPHATEST_ON");
                m.EnableKeyword("_ALPHABLEND_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = 3000;
            }
        }

        // Gradually reduce alpha over time
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);

            foreach (Renderer r in renderers)
            {
                foreach (Material m in r.materials)
                {
                    if (m.HasProperty("_Color"))
                    {
                        Color color = m.color;
                        color.a = alpha;
                        m.color = color;
                    }
                }
            }

            yield return null;
        }

        // Fully hide the ghost after fade completes
        ghostPerson.SetActive(false);
    }
}
