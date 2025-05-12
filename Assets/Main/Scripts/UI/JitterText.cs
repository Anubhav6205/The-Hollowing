using UnityEngine;
using TMPro;

/// <summary>
/// Applies a jittering effect to TextMeshPro UI text by randomly offsetting its position each frame.
/// Useful for horror or glitchy visual styles.
/// </summary>
public class JitterText : MonoBehaviour
{
    [Tooltip("Maximum offset applied in any direction each frame")]
    public float jitterAmount = 2f;

    private Vector3 startPos;
    private TMP_Text textMesh;

    void Start()
    {
        // Cache the original position and reference to the TMP component
        textMesh = GetComponent<TMP_Text>();
        startPos = textMesh.rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Apply a random offset within a circle of radius jitterAmount
        Vector2 jitter = Random.insideUnitCircle * jitterAmount;
        textMesh.rectTransform.anchoredPosition = startPos + (Vector3)jitter;
    }
}
