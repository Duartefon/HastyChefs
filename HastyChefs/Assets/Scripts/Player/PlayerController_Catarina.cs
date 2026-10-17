using System;
using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
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
    [SerializeField] private InputActionReference moveAction;   // Vector2
    [SerializeField] private InputActionReference lookAction;   // Vector2 (Mouse/Delta)
    [SerializeField] private InputActionReference jumpAction;   // Button
    [SerializeField] private InputActionReference sprintAction; // Button

    [Header("References")]
    [SerializeField] private Camera playerCamera;

    private CharacterController characterController;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    protected override void OnSpawned()
    {
        base.OnSpawned();
        enabled = isOwner;

        if (!isOwner)
            Destroy(playerCamera.gameObject);
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            enabled = false;
            return;
        }
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
        {
            velocity.y = -2f;
        }

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float currentSpeed = sprintAction.action.IsPressed() ? sprintSpeed : moveSpeed;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (jumpAction.action.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

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