using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Battery HUD display which includes:
/// • Count of spare batteries
/// • Battery meter that depletes over time when flashlight is ON
/// • Auto-refill from spare batteries when depleted
/// </summary>
public class BatteryHUD : MonoBehaviour
{
	[Header("UI Refs")]
	[Tooltip("Text to show number of batteries")]
	public TextMeshProUGUI countText;

	[Tooltip("UI Image to represent the battery fill meter (set to Filled)")]
	public Image meterImage;

	[Header("Meter Settings")]
	[Tooltip("Time in seconds it takes to drain one full battery")]
	public float drainDuration = 10f;

	// Internal state
	private int _batteryCount = 1;
	private float _charge = 1f;
	private bool _draining = false;
	private Coroutine _drainRoutine;

	/// <summary>
	/// Exposes the current battery fill level for external access.
	/// </summary>
	public float CurrentFill => _charge;

	void Awake()
	{
		// Hide battery HUD elements at start until flashlight is picked up
		countText.gameObject.SetActive(false);
		meterImage.gameObject.SetActive(false);
	}

	void OnEnable()
	{
		EventManager.FlashlightPickedUp += HandleFlashAcquired;
		EventManager.BatteryCountChanged += OnCountChanged;
		EventManager.FlashlightToggled += OnFlashlightToggled;
	}

	void OnDisable()
	{
		EventManager.FlashlightPickedUp -= HandleFlashAcquired;
		EventManager.BatteryCountChanged -= OnCountChanged;
		EventManager.FlashlightToggled -= OnFlashlightToggled;
	}

	void Start()
	{
		// Initialize HUD values
		countText.text = $"Batteries: {_batteryCount}";
		meterImage.fillAmount = _charge;
	}

	/// <summary>
	/// Called when the flashlight is first picked up.
	/// Displays and initializes the HUD.
	/// </summary>
	void HandleFlashAcquired()
	{
		countText.gameObject.SetActive(true);
		meterImage.gameObject.SetActive(true);

		countText.text = $"Batteries: {_batteryCount}";
		meterImage.fillAmount = _charge;
	}

	/// <summary>
	/// Called when battery count changes (e.g., pickup).
	/// </summary>
	void OnCountChanged(int newCount)
	{
		Debug.Log("Count changed");
		Debug.Log(newCount);

		_batteryCount = newCount;
		countText.text = $"Batteries: {_batteryCount}";

		// If flashlight was empty and now we have more batteries, refill meter
		if (_charge <= 0f && _batteryCount > 0)
		{
			_charge = 1f;
			meterImage.fillAmount = _charge;
		}
	}

	/// <summary>
	/// Starts or stops the battery drain routine depending on flashlight state.
	/// </summary>
	void OnFlashlightToggled(bool isOn)
	{
		if (_drainRoutine != null)
			StopCoroutine(_drainRoutine);

		if (isOn)
		{
			_drainRoutine = StartCoroutine(DrainRoutine());
		}
		else
		{
			_draining = false;
		}
	}

	/// <summary>
	/// Coroutine that continuously drains the battery while the flashlight is on.
	/// If depleted, consumes a spare battery and refills.
	/// </summary>
	IEnumerator DrainRoutine()
	{
		_draining = true;

		while (_draining)
		{
			_charge -= Time.deltaTime / drainDuration;

			if (_charge <= 0f)
			{
				if (_batteryCount > 1)
				{
					// Use spare battery
					_batteryCount--;
					_charge = 1f;
				}
				else
				{
					// Out of batteries
					_batteryCount = 0;
					_charge = 0f;
					_draining = false;
					countText.text = $"Batteries: {_batteryCount}";
					break;
				}

				countText.text = $"Batteries: {_batteryCount}";
			}

			meterImage.fillAmount = _charge;
			yield return null;
		}
	}
}
