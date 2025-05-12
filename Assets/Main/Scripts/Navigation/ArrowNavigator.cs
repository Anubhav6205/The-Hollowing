using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI arrow navigator that points toward a queue of world-space waypoints.
/// It rotates the arrow and adjusts its color based on distance,
/// advancing to the next waypoint when EventManager.PopWaypoint(key) is called.
/// </summary>
public class ArrowNavigator : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("UI arrow transform to rotate toward targets")]
    public RectTransform arrowRect;

    [Tooltip("Arrow image whose color will change with distance")]
    public Image arrowImg;

    [Header("Camera")]
    [Tooltip("Reference to the player camera")]
    public Camera playerCamera;

    [Header("Colour")]
    [Tooltip("Distance at which arrow becomes fully tinted")]
    public float maxTintDistance = 60f;

    [Tooltip("Arrow color when close to target")]
    public Color nearColour = Color.white;

    [Tooltip("Arrow color when far from target")]
    public Color farColour = new Color(1f, 0.3f, 0.3f);

    [Header("Waypoints (queue order)")]
    [Tooltip("List of waypoints to guide the player through")]
    public List<Waypoint> queue = new List<Waypoint>();

    [System.Serializable]
    public class Waypoint
    {
        public string key; // e.g., "D1", "D2"
        public Transform target; // world-space location
    }

    // Internal state
    private Vector2 _lockedAnchorPos;
    private int _currentIndex = 0;
    private Coroutine _delayedPop = null;

    private Dictionary<string, float> _lastPopTimeByKey = new Dictionary<string, float>();
    private const float KEY_COOLDOWN = 0f; // Add a delay if needed to prevent spamming

    void Awake()
    {
        _lockedAnchorPos = arrowRect.anchoredPosition;

        if (queue.Count > 0)
            RotateTo(queue[0].target);
    }

    void OnEnable()
    {
        EventManager.OnWaypointPop += SchedulePop;
    }

    void OnDisable()
    {
        EventManager.OnWaypointPop -= SchedulePop;
    }

    void Update()
    {
        if (_currentIndex >= queue.Count)
            return;

        // Keep the arrow anchored in place
        arrowRect.anchoredPosition = _lockedAnchorPos;

        // Continuously rotate to current target
        RotateTo(queue[_currentIndex].target);
    }

    /// <summary>
    /// Schedules a delayed response to a waypoint pop request.
    /// </summary>
    void SchedulePop(string poppedKey)
    {
        // Ignore repeat requests within cooldown
        if (_lastPopTimeByKey.TryGetValue(poppedKey, out var last) &&
            Time.time - last < KEY_COOLDOWN)
            return;

        _lastPopTimeByKey[poppedKey] = Time.time;

        if (_delayedPop != null)
            StopCoroutine(_delayedPop);

        _delayedPop = StartCoroutine(DelayedHandlePop(poppedKey));
    }

    /// <summary>
    /// Waits a moment before checking the key and advancing to the next waypoint.
    /// </summary>
    IEnumerator DelayedHandlePop(string poppedKey)
    {
        _delayedPop = null;

        if (_currentIndex >= queue.Count || queue[_currentIndex].key != poppedKey)
            yield break;

        _currentIndex++;

        if (_currentIndex >= queue.Count)
        {
            arrowRect.gameObject.SetActive(false);
        }
        else
        {
            RotateTo(queue[_currentIndex].target);
        }
    }

    /// <summary>
    /// Rotates the arrow UI element toward the given world-space target.
    /// Tints the arrow color based on distance.
    /// </summary>
    void RotateTo(Transform target)
    {
        if (target == null)
            return;

        Vector3 screenPoint = playerCamera.WorldToScreenPoint(target.position);

        // Flip behind-the-camera direction
        if (screenPoint.z < 0f)
        {
            screenPoint.x = Screen.width - screenPoint.x;
            screenPoint.y = Screen.height - screenPoint.y;
        }

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = ((Vector2)screenPoint - screenCenter).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        arrowRect.localRotation = Quaternion.Euler(0, 0, angle - 90f);

        float distance = Vector3.Distance(playerCamera.transform.position, target.position);
        float t = Mathf.Clamp01(distance / maxTintDistance);
        arrowImg.color = Color.Lerp(nearColour, farColour, t);
    }

    /// <summary>
    /// Manually pops the next waypoint. Useful for scripted events.
    /// </summary>
    public void PopNextDestination()
    {
        if (_currentIndex < queue.Count)
            SchedulePop(queue[_currentIndex].key);
    }
}
