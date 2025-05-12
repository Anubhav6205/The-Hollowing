using UnityEngine;

/// <summary>
/// Simple first-person movement script:
/// • Walk, run, jump, and apply gravity
/// • Includes fatigue system with recovery
/// • Head-bob and footstep sounds
/// • Look-back mechanic (over-the-shoulder)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class SimpleMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpHeight = 1.5f;

    [Header("Gravity")]
    public float gravity = -9.81f;

    [Tooltip("Multiplier applied to gravity when falling")]
    public float fallMultiplier = 2.5f;

    [Header("Mouse-look")]
    public Transform playerCamera;
    public float sensX = 2f;
    public float sensY = 0.7f;

    [Header("Head-bob (subtle)")]
    public float bobSpeedWalk = 20f;
    public float bobSpeedRun = 15f;
    public float bobXWalk = 5f;
    public float bobYWalk = 5f;
    public float bobXRun = 5f;
    public float bobYRun = 5f;

    [Header("Footstep audio")]
    public AudioSource walkSource;
    public AudioSource runSource;
    public float stepRateWalk = 0.55f;
    public float stepRateRun = 0.35f;

    [Header("Fatigue")]
    [Tooltip("How long you can run before you fatigue (seconds)")]
    public float minRunTime = 5f;
    public float maxRunTime = 7f;

    [Tooltip("How long you must walk to recover (seconds)")]
    public float recoveryTime = 3f;

    [Tooltip("Optional sound to play the moment you hit fatigue")]
    public AudioSource fatigueSource;

    [Header("State")]
    public bool isDead = false;

    [Header("Look-Back")]
    [Tooltip("Hold this to smoothly look over your shoulder")]
    public KeyCode lookBackKey = KeyCode.C;

    [Tooltip("Yaw offset (degrees) when looking back")]
    public float lookBackAngle = 180f;

    [Tooltip("Seconds it takes to blend into/out of look-back")]
    public float lookBackSmoothTime = 0.1f;

    // Internal
    private CharacterController cc;
    private Vector3 velocity;
    private float camPitch,
        bobTimer,
        stepTimer;
    private Vector3 camStart;
    private float runTimer,
        walkTimer;
    private bool isFatigued;
    private float _currentYawOffset = 0f;
    private float _yawOffsetVelocity;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        camStart = playerCamera.localPosition;
        Cursor.lockState = CursorLockMode.Locked;

        // Fatigue setup
        runTimer = Random.Range(minRunTime, maxRunTime);
        isFatigued = false;

        if (fatigueSource != null)
        {
            fatigueSource.loop = false;
            fatigueSource.playOnAwake = false;
            fatigueSource.Stop();
        }
    }

    void Update()
    {
        if (isDead)
        {
            StopFootsteps();
            return;
        }

        HandleMovement();
        Look();
        HeadBob();
        FootSteps();
    }

    /// <summary>
    /// Handles walk, run, jump, gravity, and fatigue.
    /// </summary>
    void HandleMovement()
    {
        bool grounded = cc.isGrounded;
        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector3 input =
            transform.right * Input.GetAxis("Horizontal")
            + transform.forward * Input.GetAxis("Vertical");
        bool moving = input.sqrMagnitude > 0.01f;

        bool wantRun = Input.GetKey(KeyCode.LeftShift);
        bool isRunning = wantRun && !isFatigued && moving;

        if (isRunning)
        {
            runTimer -= Time.deltaTime;
            if (runTimer <= 0f)
            {
                isFatigued = true;
                walkTimer = recoveryTime;
                fatigueSource?.Play();
            }
        }
        else if (isFatigued)
        {
            walkTimer -= Time.deltaTime;
            if (walkTimer <= 0f)
            {
                isFatigued = false;
                runTimer = Random.Range(minRunTime, maxRunTime);
            }
        }

        float speed = isRunning ? runSpeed : walkSpeed;
        cc.Move(input * speed * Time.deltaTime);

        if (grounded && Input.GetButtonDown("Jump"))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (velocity.y < 0f)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Handles first-person camera look and smooth look-back.
    /// </summary>
    void Look()
    {
        float mx = Input.GetAxis("Mouse X") * sensX;
        transform.Rotate(Vector3.up * mx);

        float my = Input.GetAxis("Mouse Y") * sensY;
        camPitch = Mathf.Clamp(camPitch - my, -70f, 70f);

        float targetYaw = Input.GetKey(lookBackKey) ? lookBackAngle : 0f;
        _currentYawOffset = Mathf.SmoothDamp(
            _currentYawOffset,
            targetYaw,
            ref _yawOffsetVelocity,
            lookBackSmoothTime
        );

        playerCamera.localEulerAngles = new Vector3(camPitch, _currentYawOffset, 0f);
    }

    /// <summary>
    /// Applies head-bob animation based on movement and speed.
    /// </summary>
    void HeadBob()
    {
        bool moving = Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f;
        bool grounded = cc.isGrounded;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isFatigued && moving;

        if (moving && grounded)
        {
            bobTimer += Time.deltaTime * (isRunning ? bobSpeedRun : bobSpeedWalk);

            float bx = isRunning ? bobXRun : bobXWalk;
            float by = isRunning ? bobYRun : bobYWalk;

            playerCamera.localPosition =
                camStart
                + new Vector3(Mathf.Sin(bobTimer) * bx, Mathf.Abs(Mathf.Sin(bobTimer)) * by, 0f);
        }
        else
        {
            bobTimer = 0f;
            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                camStart,
                Time.deltaTime * 7f
            );
        }
    }

    /// <summary>
    /// Manages footstep audio for walking and running.
    /// </summary>
    void FootSteps()
    {
        bool moving = Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f;
        bool grounded = cc.isGrounded;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isFatigued && moving;

        if (!grounded || !moving)
        {
            StopFootsteps();
            return;
        }

        AudioSource target = isRunning ? runSource : walkSource;
        if (
            target != null // only if component is still enabled...
            && target.enabled
            // …and its GameObject is active…
            && target.gameObject.activeInHierarchy
            && !target.isPlaying
        )
        {
            StopFootsteps();
            target.loop = true;
            target.Play();
        }
    }

    /// <summary>
    /// Stops all currently playing footstep sounds.
    /// </summary>
    void StopFootsteps()
    {
        if (walkSource != null && walkSource.isPlaying)
            walkSource.Stop();
        if (runSource != null && runSource.isPlaying)
            runSource.Stop();
    }
}
