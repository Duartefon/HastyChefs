using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerController_P_Oceans : NetworkBehaviour
{
    [Header("Player System")]
    [SerializeField] private Stamina stamina;
    [SerializeField] private Downed downed;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 1f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;

    [Header("References")]
    [SerializeField] private Camera playerCamera;

    private CharacterController characterController;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (downed == null) downed = GetComponent<Downed>();
        enabled = false; // Update stays off until we know we're the owner
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isOwner)
        {
            // Remote copy: no input, no camera, no local physics.
            if (playerCamera != null) Destroy(playerCamera.gameObject);
            characterController.enabled = false; // don't fight the NetworkTransform
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogError("PlayerController: playerCamera is not assigned.", this);
            return;
        }

        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        enabled = true;
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();

        if (!isOwner) return;

        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        bool isGrounded = IsGrounded();
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        bool isDowned = downed != null && downed.IsDowned;
        if (isDowned) moveInput = Vector2.zero; // downed: no walking, but gravity and looking still work

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        bool wantsSprint = sprintAction.action.IsPressed() && isMoving;
        bool isSprinting = wantsSprint && stamina.CanSprint;
        stamina.Tick(isSprinting, Time.deltaTime);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (jumpAction.action.WasPressedThisFrame() && isGrounded && !isDowned)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>() * lookSensitivity;

        verticalRotation -= lookInput.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookInput.x);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.03f, Vector3.down * groundCheckDistance);
    }
#endif
}