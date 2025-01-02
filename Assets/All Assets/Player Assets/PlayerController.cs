using UnityEngine;

/// <summary>
/// Controls player movement including walking, running, and jumping mechanics
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Movement variables that can be adjusted in the Unity Inspector
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;      // Base movement speed
    [SerializeField] private float runSpeed = 8f;       // Speed when running (holding shift)
    [SerializeField] private float jumpForce = 5f;      // Force applied when jumping
    [SerializeField] private float gravityMultiplier = 2.5f;  // Increases gravity effect for better feel

    [Header("Camera Settings")]
    private Camera playerCamera;    // Remove SerializeField, make private again
    
    // Component and state variables
    private CharacterController controller;  // Reference to the CharacterController component
    private Vector3 moveDirection;          // Current movement direction
    private float currentSpeed;             // Current movement speed (walk/run)
    private bool isGrounded;                // Tracks if player is touching ground
    private float verticalVelocity;         // Vertical movement for jumping/falling
    private float rotationX = 0f;

    // Initialize components
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        
        // Setup camera and cursor
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("Player camera not found! Please add a Camera as a child of the player.");
            enabled = false;
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Main update loop for movement
    private void Update()
    {
        HandleMouseLook();
        isGrounded = controller.isGrounded;
        HandleMovement();
        HandleGravity();
        HandleJump();

        // Press Escape to toggle cursor lock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? 
                              CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }

    /// <summary>
    /// Handles horizontal movement input and direction
    /// </summary>
    private void HandleMovement()
    {
        // Get input axes for movement
        float horizontal = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right arrows
        float vertical = Input.GetAxisRaw("Vertical");      // W/S or Up/Down arrows

        // Toggle between walk and run speeds
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Calculate movement vector and normalize it
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Apply movement if input is detected
        if (movement.magnitude >= 0.1f)
        {
            // Convert local direction to world space
            moveDirection = transform.TransformDirection(movement) * currentSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;  // No movement if no input
        }
    }

    /// <summary>
    /// Handles gravity application and vertical movement
    /// </summary>
    private void HandleGravity()
    {
        // Apply gravity when in air
        if (!isGrounded)
        {
            verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity = -2f;  // Small downward force when grounded
        }

        // Apply vertical movement
        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    /// <summary>
    /// Handles jump input and mechanics
    /// </summary>
    private void HandleJump()
    {
        // Jump when space is pressed and player is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    /// <summary>
    /// Handles mouse look input and camera rotation
    /// </summary>
    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotate camera up/down
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Rotate player left/right
        transform.Rotate(Vector3.up * mouseX);
    }
}
