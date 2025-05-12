using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activates a ghost model to run and fade out when the player enters the trigger.
/// The trigger only becomes active after a specific artifact is picked up.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MovingGhostTrigger : MonoBehaviour
{
    [Header("Activation Settings")]
    [Tooltip("Which artifact ID enables this trigger")]
    [SerializeField]
    private int requiredArtifactId = 1;

    [Header("Chase Settings")]
    [Tooltip("The model (and its Animator) that will run")]
    [SerializeField]
    private GameObject ghostModel;

    [Tooltip("Sound to play when the chase starts")]
    [SerializeField]
    private AudioSource audioSource;

    [Tooltip("Run speed (units per second)")]
    [SerializeField]
    private float modelSpeed = 5f;

    [Tooltip("How many seconds the model runs before fading")]
    [SerializeField]
    private float runDuration = 3f;

    [Tooltip("How many seconds to fade out the model")]
    [SerializeField]
    private float fadeDuration = 1f;

    private Collider _collider;
    private bool _isActive = false;

    /// <summary>
    /// Initializes the trigger in an inactive state and hides the ghost model.
    /// </summary>
    void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false; // Start disabled

        if (ghostModel != null)
            ghostModel.SetActive(false);

        // Optional direct activation for test purposes (can be removed)
        OnArtifactPicked(1);
    }

    /// <summary>
    /// Subscribes to artifact picked events.
    /// </summary>
    void OnEnable()
    {
        EventManager.OnArtifactPicked += OnArtifactPicked;
    }

    /// <summary>
    /// Unsubscribes from artifact picked events.
    /// </summary>
    void OnDisable()
    {
        EventManager.OnArtifactPicked -= OnArtifactPicked;
    }

    /// <summary>
    /// Activates the trigger once the required artifact is collected.
    /// </summary>
    private void OnArtifactPicked(int id)
    {
        if (id == requiredArtifactId)
        {
            _isActive = true;
            _collider.enabled = true;
        }
    }

    /// <summary>
    /// Starts the ghost run-and-fade sequence when the player enters the trigger.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!_isActive || !other.CompareTag("Player"))
            return;

        _collider.enabled = false; // prevent multiple triggers
        StartCoroutine(RunAndFadeRoutine());
    }

    /// <summary>
    /// Coroutine that handles showing the ghost, running it forward, fading it out, and cleanup.
    /// </summary>
    private IEnumerator RunAndFadeRoutine()
    {
        // 1) Activate ghost model and play audio
        if (ghostModel != null)
        {
            ghostModel.SetActive(true);
            audioSource?.Play();
        }

        // 2) Move ghost forward for runDuration
        float elapsed = 0f;
        while (elapsed < runDuration)
        {
            if (ghostModel != null)
            {
                ghostModel.transform.Translate(
                    ghostModel.transform.forward * modelSpeed * Time.deltaTime,
                    Space.World
                );
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3) Fade out all renderers over fadeDuration
        if (ghostModel != null)
        {
            var renderers = ghostModel.GetComponentsInChildren<Renderer>();
            var originalColors = new List<Color[]>();

            // Cache original material colors
            foreach (var r in renderers)
            {
                var mats = r.materials;
                var cols = new Color[mats.Length];
                for (int i = 0; i < mats.Length; i++)
                    cols[i] = mats[i].color;
                originalColors.Add(cols);
            }

            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                for (int ri = 0; ri < renderers.Length; ri++)
                {
                    var r = renderers[ri];
                    var mats = r.materials;
                    for (int mi = 0; mi < mats.Length; mi++)
                    {
                        if (mats[mi].HasProperty("_Color"))
                        {
                            Color c = originalColors[ri][mi];
                            c.a = alpha;
                            mats[mi].color = c;
                        }
                    }
                    r.materials = mats;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Hide and stop ghost after fade
            ghostModel.SetActive(false);
            audioSource?.Stop();
        }

        // 4) Destroy this trigger object to prevent reactivation
        Destroy(gameObject);
    }
}
