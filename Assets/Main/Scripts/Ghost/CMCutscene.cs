using System.Collections;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Plays a cutscene using a Cinemachine dolly camera along a path.
/// Handles camera priority swap, dolly movement, optional music, and notifies trigger when complete.
/// </summary>
public class CMCutscene : MonoBehaviour
{
    [Header("VCams")]
    [Tooltip("Your gameplay VCam")]
    public CinemachineVirtualCamera playerVcam;

    [Tooltip("The Virtual Camera on the Dolly track")]
    public CinemachineVirtualCamera cutsceneVcam;

    [Header("Dolly-track Settings")]
    [Tooltip("The TrackedDolly component on your cutsceneVcam")]
    public CinemachineTrackedDolly dollyCart;

    [Tooltip("Distance along the path to travel")]
    public float pathLength = 10f;

    [Tooltip("Units per second")]
    public float speed = 5f;

    [Header("Hold time at end")]
    [Tooltip("Seconds to wait before ending")]
    public float waitTime = 2f;

    [Tooltip("Optional music to play during the cutscene")]
    public AudioSource music;

    [Header("Hookup")]
    [Tooltip("Drag in the CutsceneTrigger that drives this cutscene")]
    public CutsceneTrigger cutsceneTrigger;

    // Internal state
    private int _origPlayerPrio, _origCutscenePrio;
    private float _currentDistance;
    private bool _isPlaying = false;

    /// <summary>
    /// Caches original camera priorities and ensures dollyCart is assigned.
    /// </summary>
    void Awake()
    {
        _origPlayerPrio = playerVcam.Priority;
        _origCutscenePrio = cutsceneVcam.Priority;

        if (dollyCart == null)
            dollyCart = cutsceneVcam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    /// <summary>
    /// Updates dolly position while the cutscene is playing.
    /// </summary>
    void Update()
    {
        if (!_isPlaying)
            return;

        // Advance along dolly path
        _currentDistance += Time.deltaTime * speed;
        dollyCart.m_PathPosition = Mathf.Min(pathLength, _currentDistance);

        // End cutscene once max distance is reached
        if (_currentDistance >= pathLength)
        {
            _isPlaying = false;
            StartCoroutine(EndCutsceneCoroutine());
        }
    }

    /// <summary>
    /// Starts the cutscene by enabling dolly movement and swapping camera priorities.
    /// </summary>
    public void StartCutscene()
    {
        music?.Play();

        _currentDistance = 0f;
        dollyCart.m_PathPosition = 0f;

        // Switch to cutscene camera
        cutsceneVcam.Priority = _origPlayerPrio + 1;
        _isPlaying = true;
    }

    /// <summary>
    /// Waits at the end of the cutscene before restoring camera priorities
    /// and notifying the CutsceneTrigger to resume gameplay.
    /// </summary>
    private IEnumerator EndCutsceneCoroutine()
    {
        Debug.Log("end cutscene");

        // Hold final dolly frame
        yield return new WaitForSeconds(waitTime);

        // Restore original camera priorities
        cutsceneVcam.Priority = _origCutscenePrio;
        playerVcam.Priority = _origPlayerPrio;

        // Stop music if playing
        if (cutsceneTrigger != null)
        {
            music?.Stop();
            cutsceneTrigger.EndCutscene();
        }
    }
}
