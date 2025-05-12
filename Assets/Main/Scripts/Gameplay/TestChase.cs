using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NOT BEING USED. NOT DELETED TO PREVENT LAST MOMENT ERRORS 
/// Enables a chase mechanic using NavMeshAgent when the player enters a trigger zone.
/// The assigned agent will continuously move toward the target (typically the player).
/// </summary>
[RequireComponent(typeof(Collider))]
public class TestChase : MonoBehaviour
{
	[Header("Who should chase?")]

	[Tooltip("The GameObject that has the NavMeshAgent")]
	public NavMeshAgent chaserAgent;

	[Tooltip("The transform of whatever to chase (usually the player)")]
	public Transform target;

	// Internal flag to track whether chasing has started
	private bool _isChasing = false;

	/// <summary>
	/// Ensures the collider is a trigger and checks for valid agent and target assignments.
	/// </summary>
	void Awake()
	{
		// Enforce trigger mode on collider
		var col = GetComponent<Collider>();
		col.isTrigger = true;

		if (chaserAgent == null)
			Debug.LogWarning("No chaserAgent assigned on " + name);
		if (target == null)
			Debug.LogWarning("No target assigned on " + name);
	}

	/// <summary>
	/// Starts the chase when the target (e.g., player) enters the trigger zone.
	/// Disables the trigger afterward to prevent repeat activation.
	/// </summary>
	void OnTriggerEnter(Collider other)
	{
		if (_isChasing) return;

		// Only start chasing if the player or specific target entered
		if (other.transform != target && !other.CompareTag("Player"))
			return;

		_isChasing = true;

		// Disable this trigger so it doesn't fire again
		GetComponent<Collider>().enabled = false;
	}

	/// <summary>
	/// Continuously sets the chaser's destination to the target's current position.
	/// </summary>
	void Update()
	{
		if (!_isChasing || chaserAgent == null || target == null) return;

		// Continuously move toward the target
		chaserAgent.SetDestination(target.position);
	}
}
