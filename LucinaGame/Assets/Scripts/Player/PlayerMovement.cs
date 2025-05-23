using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Basics")]
    public AudioClip landingSound;
    public GameObject monster;
    private bool wasGroundedLastFrame = true;
    private float fallStartY;
    private bool isFalling = false;
    public bool outside = true;
    public bool makingNoise = false;
    public float walkSpeed = 5f;
    private float lookSensitivity;
    public Slider sensitivitySlider;
    public float gravity = -9.81f;
    public Camera _camera;
    public Transform cameraParent;
    public Transform flashLight;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode crouchKey2 = KeyCode.LeftControl;
    public KeyCode pauseKey = KeyCode.Escape;
    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private bool moving = false;
    private bool isPaused = false;

    [Header("Sprinting")]
    public float sprintSpeed = 8f;
    private Quaternion targetRotation;

    [Header("Crouching")]
    public float crouchSpeed = 3f;
    private float crouchHeight = 0.5f;
    private float standingHeight = 2f;
    private bool isCrouching = false;

    [Header("Stamina")]
    public float playerStamina = 100.0f;
    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private bool hasRegenerated = true;

    [Header("Stamina Regen")]
    [Range(0, 50)] [SerializeField] private float staminaDrain = 25f;
    [Range(0.001f, 1)] [SerializeField] private float staminaRegen = 0.05f;

    [Header("Stamina Speed")]
    [SerializeField] private Image staminaProgressUI = null;
    [SerializeField] private CanvasGroup sliderCanvasGroup = null;

    [Header("Camera Embellishment")]
    public float tiltSpeed = 5;
    public float maxTilt = 2;
    public float walkBobbingAmount = 0.05f; 
    public float sprintBobbingAmount = 0.1f;
    public float crouchBobbingAmount = 0.02f;
    private float currentTilt;
    private float defaultCameraHeight;
    private float timeBobbing = 0f;
    private float targetBobbingAmount = 0f;
    private float bobbingFrequency = 10f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;
    private float footstepInterval = 0.7f;
    private float timeSinceLastStep = 0f;
    // Audio sources we care about pausing
    public AudioSource ambience;
    public AudioSource glitch;
    public AudioSource jumpscare;
    public AudioSource heartbeat;
    public AudioSource breathing;
    public AudioSource siren;
    public AudioSource[] pauseSources;
    public bool isSprinting = false;

    [Header("Random")]
    // these are for  the flashlight while sprinting
    public float swayAmount = 0f;
    public float swaySpeed = 3f; 
    public float swayIntensity = 5f; 

    [Header("Pause Menu")]
    public Canvas pauseCanvas;
    private MonsterAI monsterAI;
    public Interactor interactor;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (monster != null)
        {
            monsterAI = monster.GetComponent<MonsterAI>();
        }

        // mouse stuff
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 2f);
        lookSensitivity = sensitivitySlider.value;
        sensitivitySlider.onValueChanged.AddListener(SetSens);

        // random
        defaultCameraHeight = _camera.transform.localPosition.y; // for crouching
        sliderCanvasGroup.alpha = 0; // stamina UI is not there when full on start
        targetRotation = flashLight.rotation; // inital flashlight rotation

        pauseCanvas.gameObject.SetActive(false);
    }

    void SetSens(float sens) {
        lookSensitivity = sens;
        PlayerPrefs.SetFloat("Sensitivity", lookSensitivity);
    }

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        int angelCount = 0;

        foreach (Collider col in colliders)
        {
            if (col.GetComponent<AngelAI>() != null)
                angelCount++;
        }

        if (angelCount >= 3 && !isPlaying)
        {
            isPlaying = true;
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeIn(creepyAudioSource, fadeDuration, creepyMaxVolume));
        }
        else if (angelCount < 3 && isPlaying)
        {
            isPlaying = false;
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeOut(creepyAudioSource, fadeDuration));
        }
        if (!isPaused) 
        {
            // player movement
            HandleMovement();

            // camera tilt based on movement
            UpdateTilt();

            // mouse movement
            if(!interactor.inNote){
                MouseLook();
            }

            // head bobbing effect
            HandleHeadBobbing();

            // for flashlight while sprinting
            SmoothRotateFlashlight();
        }

        // for pausing
        HandlePausing();
    }

    private void HandleMovement()
    {
        // moving
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = walkSpeed; // default

        if (x != 0 || z != 0) { // get variable thats used for a few things
            moving = true;
        } else{
            moving = false;
            makingNoise = false;
        }

        if (Input.GetKeyDown(sprintKey) && !isCrouching && hasRegenerated && moving)
        {
            targetRotation = Quaternion.Euler(30, 0, 0); // flashlight rotation while sprinting
        }

        if (Input.GetKeyUp(sprintKey))
        {
            targetRotation = Quaternion.Euler(0, 0, 0); // reset flashlight rotation
        }

        // sprinting logic
        if (Input.GetKey(sprintKey) && !isCrouching && hasRegenerated && moving) // update speed and drain stamina
        {
            currentSpeed = sprintSpeed;
            playerStamina -= staminaDrain * Time.deltaTime;
            UpdateStamina(1);

            if (playerStamina <= 0) {
                hasRegenerated = false;
                sliderCanvasGroup.alpha = 0;
            }
        } 
        else // regen the stamina
        {
            flashLight.Rotate(0, 0 , 0);
            currentSpeed = walkSpeed;
            if (playerStamina != maxStamina) {
                playerStamina += staminaRegen + Time.deltaTime;
                UpdateStamina(1);

                if (playerStamina >= maxStamina) {
                    playerStamina = maxStamina;
                    sliderCanvasGroup.alpha = 0;
                    hasRegenerated = true;
                }
            }
        }

        // crouching logic
        if (Input.GetKey(crouchKey) || Input.GetKey(crouchKey2) ) // update speed to crouch speed
        {
            makingNoise = false;
            controller.height = crouchHeight;
            currentSpeed = crouchSpeed;
            isCrouching = true;

            // move camera down while crouching
            _camera.transform.localPosition = new Vector3(
                _camera.transform.localPosition.x,
                defaultCameraHeight - (defaultCameraHeight - crouchHeight), 
                _camera.transform.localPosition.z
            );
        }
        else // update speed back to default
        {
            controller.height = standingHeight;
            isCrouching = false;

            // reset camera to normal height
            _camera.transform.localPosition = new Vector3(
                _camera.transform.localPosition.x,
                defaultCameraHeight,
                _camera.transform.localPosition.z
            );
        }

        bool isGrounded = controller.isGrounded;

        if (!isGrounded && !isFalling)
        {
            isFalling = true;
            fallStartY = transform.position.y;
        }

        if (isFalling && isGrounded)
        {
            float fallDistance = fallStartY - transform.position.y;

            if (fallDistance > 1.5f)
            { 
                makingNoise = true;

                float normalizedFall = Mathf.Clamp01((fallDistance - 1.5f) / 15f);
                float volume = Mathf.Pow(normalizedFall, 2);

                audioSource.PlayOneShot(landingSound, volume);
            }

            isFalling = false;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -12f;
        } else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move((currentSpeed * move + velocity) * Time.deltaTime);
        
        wasGroundedLastFrame = isGrounded;

        isSprinting = currentSpeed == sprintSpeed;
        
        if (((currentSpeed * move + velocity) * Time.deltaTime).x >= 0.035 || ((currentSpeed * move + velocity) * Time.deltaTime).x <= -0.035 || ((currentSpeed * move + velocity) * Time.deltaTime).z >= 0.035 || ((currentSpeed * move + velocity) * Time.deltaTime).z <= -0.035)
        {
            if (!isCrouching && !outside)
            {
                makingNoise = true;
            }
        }

        // footstep sounds
        if (moving)
        {
            timeSinceLastStep += Time.deltaTime;

            if (isSprinting)
            {
                footstepInterval = 0.35f;
            }
            else if (isCrouching)
            {
                footstepInterval = 0.8f;
            }
            else
            {
                footstepInterval = 0.65f;
            }

            if (timeSinceLastStep >= footstepInterval)
            {
                timeSinceLastStep = 0f;
                PlayRandomFootstepSound();
            }
        }
    }

    private void SmoothRotateFlashlight()
    {
        Quaternion cameraRotation = _camera.transform.rotation;

        // If sprinting apply add down tilt
        if (Input.GetKey(sprintKey) && !isCrouching && hasRegenerated && moving)
        {
            Quaternion sprintRotation = Quaternion.Euler(30, cameraRotation.eulerAngles.y, 0);
            swayAmount = Mathf.Sin(Time.time * swaySpeed) * swayIntensity;
            flashLight.rotation = Quaternion.Slerp(flashLight.rotation, 
                sprintRotation * Quaternion.Euler(0, swayAmount, 0), Time.deltaTime * 5f);
        }
        else // make follow camera
        {
            flashLight.rotation = Quaternion.Slerp(flashLight.rotation, cameraRotation, Time.deltaTime * 5f);
        }
    }


    void UpdateStamina(int value){ // stamina UI bar
        staminaProgressUI.fillAmount = playerStamina / maxStamina;

        if (value == 0){
            sliderCanvasGroup.alpha = 0;
        } else {
            sliderCanvasGroup.alpha = 1;
        }
    }

    private void UpdateTilt() // adds slight tilt when pressing A and D
    {
        float moveX = Input.GetAxis("Horizontal");
        float targetTiltZ = moveX == 0 ? 0 : maxTilt * -Mathf.Sign(moveX);

        currentTilt = Mathf.Lerp(currentTilt, targetTiltZ, Time.deltaTime * tiltSpeed);

        var cameraRotation = cameraParent.localEulerAngles;
        cameraRotation.z = currentTilt;
        cameraParent.localEulerAngles = cameraRotation;
    }

    void MouseLook() // moves the camera to where you are looking
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        _camera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void PlayRandomFootstepSound() // sounds for footsteps --- janky needs better solution
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Length);
            if(isCrouching){
                audioSource.PlayOneShot(footstepSounds[randomIndex], 0.1f);
            } else if (isSprinting){
                audioSource.PlayOneShot(footstepSounds[randomIndex], 0.4f);
            } else {
                audioSource.PlayOneShot(footstepSounds[randomIndex], 0.3f);
            }
        }
    }

    private void HandleHeadBobbing() // adds "realistic" movement of camera
    {
        if (isCrouching) // crouching
        {
            timeBobbing = Mathf.Lerp(timeBobbing, 0, Time.deltaTime * 10);
        }
        else
        {
            targetBobbingAmount = 0f;
            bobbingFrequency = 10f;

            if (Input.GetKey(sprintKey) && moving && hasRegenerated && !isCrouching) // sprinting
            {
                targetBobbingAmount = sprintBobbingAmount * 0.5f;
                bobbingFrequency = 20f;
            }
            else if (moving)  // walking
            {
                targetBobbingAmount = walkBobbingAmount; 
                bobbingFrequency = 10f;
            }

            timeBobbing = Mathf.Lerp(timeBobbing, targetBobbingAmount, Time.deltaTime * 5);

            // apply head bobbing
            _camera.transform.localPosition = new Vector3(
                _camera.transform.localPosition.x,
                defaultCameraHeight + Mathf.Sin(Time.time * bobbingFrequency) * timeBobbing,
                _camera.transform.localPosition.z
            );
        }
    }

    [SerializeField] private float detectionRadius = 40f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AudioSource creepyAudioSource;
    [SerializeField] private float creepyMaxVolume = 0.75f;

    private Coroutine fadeRoutine;
    private bool isPlaying = false;

    private IEnumerator FadeIn(AudioSource audioSource, float duration, float maxVolume)
    {
        if (!audioSource.isPlaying) audioSource.Play();
        float time = 0f;
        audioSource.volume = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, time / duration);
            yield return null;
        }

        audioSource.volume = maxVolume;
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    private void HandlePausing() 
    {
        if (Input.GetKeyDown(pauseKey))
        {
            isPaused = !isPaused;
            Cursor.lockState = isPaused ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = isPaused;
            Time.timeScale = isPaused ? 0 : 1;
            
            if (isPaused) 
            {
                audioSource.Pause();
                ambience.Pause();
                glitch.Pause();
                jumpscare.Pause();
                heartbeat.Pause();
                breathing.Pause();
                siren.Pause();
                for (int i=0; i<pauseSources.Length; i++) {
                    pauseSources[i].Pause();
                }
            }
            else 
            {
                audioSource.UnPause();
                ambience.UnPause();
                glitch.UnPause();
                jumpscare.UnPause();
                heartbeat.UnPause();
                breathing.UnPause();
                siren.UnPause();
                for (int i=0; i<pauseSources.Length; i++) {
                    pauseSources[i].UnPause();
                }

                interactor.closeNote();                   
            }

            pauseCanvas.gameObject.SetActive(isPaused);
        }

    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseCanvas.gameObject.SetActive(isPaused);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void quitGame()
    {
        Application.Quit();
    }

   private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("houseWall") && outside == true)
    {
        Debug.Log("inside");
        outside = false;

        if (moving && !isCrouching)
        {
            makingNoise = true;
        }

        if(monster.activeSelf){
            Debug.Log("stop monster");
            monsterAI.WarpAndStop();
        }
    }
}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("houseWall") && outside == false)
        {
            Debug.Log("outside");
            outside = true;
            makingNoise = false;

            if (GameManager.Instance.momentos >= 1 && monster.activeSelf)
            {
                Debug.Log("start monster");
                monsterAI.ReactivateMovement();
            }
        }
    }
}