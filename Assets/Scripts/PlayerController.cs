using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6.5f;
    [SerializeField] private float sprintSpeed = 9.5f;
    [SerializeField] private float gravity = -18f;
    [SerializeField] private float jumpHeight = 0f;

    [Header("Sprint Stamina")]
    [SerializeField] private float maxStamina = 4f;
    [SerializeField] private float staminaDrainPerSecond = 1f;
    [SerializeField] private float staminaRegenPerSecond = 0.8f;
    [SerializeField] private float staminaRegenDelay = 1f;

    [Header("Look")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float lookSensitivity = 1.75f;
    [SerializeField] private float pitchClamp = 85f;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactMask = ~0;

    private CharacterController controller;
    private float velocityY;
    private float cameraPitch;
    private float stamina;
    private float lastSprintTime;

    public float StaminaNormalized => maxStamina <= 0f ? 0f : stamina / maxStamina;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stamina = maxStamina;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleInteraction();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -pitchClamp, pitchClamp);

        if (cameraPivot != null)
        {
            cameraPivot.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        bool sprintHeld = Input.GetKey(KeyCode.LeftShift);
        bool canSprint = stamina > 0f;
        bool moving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;

        bool sprinting = sprintHeld && canSprint && moving;
        float targetSpeed = sprinting ? sprintSpeed : walkSpeed;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;
        controller.Move(move * targetSpeed * Time.deltaTime);

        if (controller.isGrounded && velocityY < 0f)
        {
            velocityY = -2f;
        }

        if (jumpHeight > 0f && Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocityY += gravity * Time.deltaTime;
        controller.Move(Vector3.up * velocityY * Time.deltaTime);

        if (sprinting)
        {
            stamina = Mathf.Max(0f, stamina - staminaDrainPerSecond * Time.deltaTime);
            lastSprintTime = Time.time;
        }
        else if (Time.time - lastSprintTime >= staminaRegenDelay)
        {
            stamina = Mathf.Min(maxStamina, stamina + staminaRegenPerSecond * Time.deltaTime);
        }
    }

    private void HandleInteraction()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        Transform rayOrigin = cameraPivot != null ? cameraPivot : transform;
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, interactRange, interactMask))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            interactable?.Interact(this);
        }
    }
}
