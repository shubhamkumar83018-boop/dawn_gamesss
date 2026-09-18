using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player controller for 7 Minutes Before Dawn.
/// Handles walk, run, crouch, interact. Integrates noise system.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.2f;

    [Header("Look")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float topClamp = 70f;
    [SerializeField] private float bottomClamp = -30f;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private LayerMask interactMask;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] walkFootsteps;
    [SerializeField] private AudioClip[] runFootsteps;
    [SerializeField] private AudioClip landClip;

    private CharacterController cc;
    private Vector3 velocity;
    private bool grounded;
    private bool isCrouching;
    private bool isSprinting;
    private float cinemachineTargetPitch;
    private float footstepTimer;
    private float footstepInterval = 0.45f;

    // Input values
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool sprintHeld;
    private bool crouchPressed;
    private bool interactPressed;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.GameOver) return;

        GroundCheck();
        HandleMovement();
        HandleLook();
        HandleFootsteps();
        HandleInteract();
        ApplyGravity();
    }

    void GroundCheck()
    {
        grounded = cc.isGrounded;
        if (grounded && velocity.y < 0f) velocity.y = -2f;
    }

    void HandleMovement()
    {
        isSprinting = sprintHeld && !isCrouching;
        float speed = isCrouching ? crouchSpeed : (isSprinting ? runSpeed : walkSpeed);

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        cc.Move(move * speed * Time.deltaTime);

        // Jump
        if (jumpPressed && grounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        jumpPressed = false;

        // Noise
        if (move.magnitude > 0.1f && grounded)
        {
            NoiseType noise = isSprinting ? NoiseType.Run : (isCrouching ? NoiseType.Crouch : NoiseType.Walk);
            NoiseSystem.Instance?.MakeNoise(noise, transform.position);
        }
    }

    void HandleLook()
    {
        cinemachineTargetPitch = Mathf.Clamp(
            cinemachineTargetPitch - lookInput.y * mouseSensitivity,
            bottomClamp, topClamp);
        if (cameraRoot != null)
            cameraRoot.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
    }

    void HandleFootsteps()
    {
        if (!grounded || moveInput.magnitude < 0.1f) return;
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            float interval = isSprinting ? footstepInterval * 0.6f : footstepInterval;
            footstepTimer = interval;
            AudioClip[] clips = isSprinting ? runFootsteps : walkFootsteps;
            if (clips != null && clips.Length > 0)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
    }

    void HandleInteract()
    {
        if (!interactPressed) return;
        interactPressed = false;

        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this);
                NoiseSystem.Instance?.MakeNoise(NoiseType.Interact, transform.position);
            }
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    // Input callbacks (connect from Input System)
    public void OnMove(InputValue value)   => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value)   => lookInput = value.Get<Vector2>();
    public void OnJump(InputValue value)   => jumpPressed = value.isPressed;
    public void OnSprint(InputValue value) => sprintHeld = value.isPressed;
    public void OnCrouch(InputValue value) { if (value.isPressed) isCrouching = !isCrouching; }
    public void OnInteract(InputValue value) { if (value.isPressed) interactPressed = true; }
}

public interface IInteractable
{
    void Interact(PlayerController player);
    string GetPrompt();
}
