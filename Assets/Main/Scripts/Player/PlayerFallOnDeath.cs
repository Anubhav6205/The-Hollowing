using UnityEngine;

/// <summary>
/// Simulates a dramatic camera fall and wobble on player death:
/// • Plays a scream sound while rotating forward
/// • Triggers a "thump" on hitting the ground
/// • Applies a wobble motion after the fall completes
/// </summary>
public class PlayerFallOnDeath : MonoBehaviour
{
    private bool isFalling = false;
    private float fallVelocity = 0f;
    private float gravity = 100f;
    private Vector3 fallDirection;
    private float rotationAmount = 0f;
    private bool thumped = false;
    private float wobbleTimer = 0f;
    private bool screamPlayed = false;
    private bool thumpPlayed = false;

    [Header("Audio")]
    [Tooltip("Scream sound when fall begins")]
    public AudioSource screamSound;

    [Tooltip("Thump sound when hitting the ground")]
    public AudioSource thumpSound;

    [Tooltip("The player's main camera to rotate")]
    public GameObject mainCamera;

    /// <summary>
    /// Determines the direction of the fall on startup.
    /// </summary>
    void Start()
    {
        // Randomize the fall slightly to the left or right
        float side = Random.Range(-1f, 1f);
        fallDirection = new Vector3(side, 0f, 1f).normalized;
    }

    /// <summary>
    /// Drives the fall rotation and wobble once falling has started.
    /// </summary>
    void Update()
    {
        if (!isFalling)
            return;

        // Play scream once at the start of fall
        if (!screamPlayed && screamSound != null)
        {
            screamSound.Play();
            screamPlayed = true;
        }

        if (!thumped)
        {
            fallVelocity += gravity * Time.deltaTime;
            rotationAmount += fallVelocity * Time.deltaTime;

            // Rotate camera in fall direction
            Vector3 eulerRotation = fallDirection * rotationAmount;
            mainCamera.transform.localRotation = Quaternion.Euler(eulerRotation);

            // Stop at max rotation and play thump
            if (rotationAmount >= 80f)
            {
                rotationAmount = 80f;
                fallVelocity = 0f;
                thumped = true;

                if (thumpSound != null && !thumpPlayed)
                {
                    thumpSound.Play();
                    thumpPlayed = true;
                }
            }
        }
        else
        {
            // Apply subtle camera wobble after thump
            wobbleTimer += Time.deltaTime * 5f;
            float wobble = Mathf.Sin(wobbleTimer) * 4f;
            Vector3 eulerRotation = fallDirection * rotationAmount;
            eulerRotation.z += wobble;
            transform.localRotation = Quaternion.Euler(eulerRotation);
        }
    }

    /// <summary>
    /// Call this to begin the fall and death animation.
    /// </summary>
    public void StartFalling()
    {
        Debug.Log("started falling");
        isFalling = true;
    }
}
