using System.Collections;
using UnityEngine;

/// <summary>
/// NOT BEING USED. NOT DELETED TO PREVENT LAST MOMENT ERRORS 
/// Triggers a jumpscare where the camera pans toward a ghost outside a window,
/// the ghost pops out briefly, and then retreats while sound plays.
/// This is activated once after a battery is collected.
/// </summary>
public class WindowJumpScareController : MonoBehaviour
{
	[Header("Ghost & Look Target")]
	[Tooltip("The ghost GameObject (initially inactive)")]
	public GameObject ghost;

	[Tooltip("Empty child placed where you want the camera to face")]
	public Transform ghostLookTarget;

	[Header("Timings")]
	[Tooltip("Duration to pan camera toward the ghost")]
	public float panDuration = 0.5f;

	[Tooltip("Pause duration while looking at the ghost")]
	public float holdDuration = 0.3f;

	[Tooltip("How far forward the ghost moves (local Z axis)")]
	public float ghostMoveDistance = 0.5f;

	[Tooltip("Duration the ghost moves forward/back")]
	public float ghostMoveDuration = 0.2f;

	[Header("Audio (optional)")]
	[Tooltip("SFX to play at the start of the scare")]
	public AudioSource scareSfx;

	[Header("References")]
	[Tooltip("Your FPS camera transform")]
	public Transform playerCamera;

	// Internal state tracking
	private bool hasScared = false;
	private Quaternion originalCamRot;

	/// <summary>
	/// Subscribes to the battery collection event.
	/// </summary>
	void OnEnable()
	{
		EventManager.BatteryCollected += OnBatteryCollected;
	}

	/// <summary>
	/// Unsubscribes from the battery collection event.
	/// </summary>
	void OnDisable()
	{
		EventManager.BatteryCollected -= OnBatteryCollected;
	}

	/// <summary>
	/// Starts the jumpscare sequence once when a battery is collected.
	/// </summary>
	/// <param name="blinker">The blinking object collected (ignored here)</param>
	private void OnBatteryCollected(ObjectBlinkLight blinker)
	{
		if (hasScared) return;
		hasScared = true;
		StartCoroutine(JumpScareSequence());
	}

	/// <summary>
	/// Main coroutine to handle the sequence of events in the jumpscare.
	/// </summary>
	private IEnumerator JumpScareSequence()
	{
		// 1) Activate the ghost object
		ghost.SetActive(true);

		// 2) Play scare sound
		scareSfx?.Play();

		// 3) Save current camera rotation
		originalCamRot = playerCamera.rotation;

		// 4) Calculate look direction toward ghost
		Vector3 dir = ghostLookTarget.position - playerCamera.position;
		Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

		// 5) Smoothly rotate camera to face ghost
		yield return StartCoroutine(LerpRotation(originalCamRot, targetRot, panDuration));

		// 6) Pause while facing ghost
		yield return new WaitForSeconds(holdDuration);

		// 7) Ghost moves forward slightly
		Vector3 startLocal = ghost.transform.localPosition;
		Vector3 endLocal = startLocal + Vector3.forward * ghostMoveDistance;
		yield return StartCoroutine(MoveLocal(ghost.transform, startLocal, endLocal, ghostMoveDuration));

		// 8) Hold position briefly
		yield return new WaitForSeconds(holdDuration);

		// 9) Ghost retreats back to original position
		yield return StartCoroutine(MoveLocal(ghost.transform, endLocal, startLocal, ghostMoveDuration));

		// 10) Smoothly rotate camera back to original rotation
		yield return StartCoroutine(LerpRotation(targetRot, originalCamRot, panDuration));

		// 11) Reset everything
		playerCamera.rotation = originalCamRot;
		ghost.SetActive(false);
	}

	/// <summary>
	/// Smoothly rotates the camera from one rotation to another over a duration.
	/// </summary>
	private IEnumerator LerpRotation(Quaternion from, Quaternion to, float duration)
	{
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			playerCamera.rotation = Quaternion.Slerp(from, to, t / duration);
			yield return null;
		}
		playerCamera.rotation = to;
	}

	/// <summary>
	/// Smoothly moves a transform between two local positions over a duration.
	/// </summary>
	private IEnumerator MoveLocal(Transform tr, Vector3 from, Vector3 to, float duration)
	{
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			tr.localPosition = Vector3.Lerp(from, to, t / duration);
			yield return null;
		}
		tr.localPosition = to;
	}
}
