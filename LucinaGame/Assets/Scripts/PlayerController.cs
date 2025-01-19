using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 8f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 5f;  // Speed when crouching
    public float maxSprintTime = 3f;
    public float sprintTime = 0f;

    [Header("Crouch")]
    public float crouchHeight = 0.5f;  // Height of the player when crouching
    public float standingHeight = 2f;  // Height of the player when standing
    private bool isCrouching = false;

    [Header("MISC")]
    private CharacterController controller;
    private Vector3 velocity;
    public float lookSensitivity = 2f;
    private float verticalRotation = 0f;
    public Camera playerCamera;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;
    [SerializeField] private float footstepInterval = 0.5f;
    private float timeSinceLastStep = 0f;

    [Header("Camera Tilt")]
    [SerializeField] private Transform _cameraParent;
    [SerializeField] private float _tiltSpeed = 5;
    [SerializeField] private float _maxTilt = 2;
    private float _currentTiltZ;

    [Header("Head Bobbing")]
    public float walkBobbingAmount = 0.05f;  // How much the head bobs when walking
    public float sprintBobbingAmount = 0.1f;  // How much the head bobs when sprinting
    public float crouchBobbingAmount = 0.02f;  // How much the head bobs when crouching
    private float defaultCameraHeight;
    private float timeBobbing = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultCameraHeight = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        // Player movement
        HandleMovement();

        // Camera tilt based on movement
        UpdateTilt();

        // Mouse look
        MouseLook();

        // Head bobbing effect
        HandleHeadBobbing();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = walkSpeed;

        // Sprinting logic
        sprintTime += Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && sprintTime >= maxSprintTime)
        {
            currentSpeed = sprintSpeed;
        }

        // Crouching logic (you have to hold the button)
        if (Input.GetKey(KeyCode.C))
        {
            controller.height = crouchHeight;
            currentSpeed = crouchSpeed;
            isCrouching = true;

            // Move the camera down when crouching
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultCameraHeight - (defaultCameraHeight - crouchHeight), // Move the camera down
                playerCamera.transform.localPosition.z
            );
        }
        else
        {
            controller.height = standingHeight;
            isCrouching = false;

            // Reset camera position to default height when standing
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultCameraHeight, // Camera returns to the standing height
                playerCamera.transform.localPosition.z
            );
        }

        // Apply movement and gravity
        controller.Move(currentSpeed * Time.deltaTime * move);
        controller.Move(velocity * Time.deltaTime);

        // Footstep sounds
        if (x != 0 || z != 0) // If there is any movement (W, A, S, D)
        {
            timeSinceLastStep += Time.deltaTime;

            if (currentSpeed == sprintSpeed)
            {
                footstepInterval = 0.35f;
            }
            else if (isCrouching)
            {
                footstepInterval = 0.7f;
            }
            else
            {
                footstepInterval = 0.62f;
            }

            if (timeSinceLastStep >= footstepInterval)
            {
                timeSinceLastStep = 0f;
                PlayRandomFootstepSound();
            }
        }
    }

    private void UpdateTilt()
    {
        // Get movement input (using horizontal axis, which affects camera tilt)
        float moveX = Input.GetAxis("Horizontal");
        float targetTiltZ = moveX == 0 ? 0 : _maxTilt * -Mathf.Sign(moveX);

        _currentTiltZ = Mathf.Lerp(_currentTiltZ, targetTiltZ, Time.deltaTime * _tiltSpeed);

        var cameraRotation = _cameraParent.localEulerAngles;
        cameraRotation.z = _currentTiltZ;
        _cameraParent.localEulerAngles = cameraRotation;
    }

    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // Sounds for footsteps
    private void PlayRandomFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            audioSource.PlayOneShot(footstepSounds[randomIndex], 0.5f);
        }
    }

    private void HandleHeadBobbing()
    {
        if (isCrouching)
        {
            timeBobbing = Mathf.Lerp(timeBobbing, 0, Time.deltaTime * 10);
        }
        else
        {
            float targetBobbingAmount = 0f;
            float bobbingFrequency = 10f;

            if (Input.GetKey(KeyCode.LeftShift))  // Running
            {
                targetBobbingAmount = sprintBobbingAmount * 0.5f;  // Decreased amount for running
                bobbingFrequency = 20f;  // Faster frequency for running
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))  // Walking
            {
                targetBobbingAmount = walkBobbingAmount;  // Normal amount for walking
                bobbingFrequency = 10f;  // Normal frequency for walking
            }

            timeBobbing = Mathf.Lerp(timeBobbing, targetBobbingAmount, Time.deltaTime * 5);

            // Apply head bobbing
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultCameraHeight + Mathf.Sin(Time.time * bobbingFrequency) * timeBobbing,  // Use dynamic frequency
                playerCamera.transform.localPosition.z
            );
        }
    }
}
