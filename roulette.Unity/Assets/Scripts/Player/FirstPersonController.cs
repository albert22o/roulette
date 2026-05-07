using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -19.62f;

    [Header("Камера")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalClamp = 85f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;
    private bool active = false;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;


    public void Init()
    {
        if (cameraTransform  == null)
        {
            Debug.LogError("Не назначена ссылка на камеру");
            return;
        }
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        active = true;
    }

    private void Update()
    {
        if (active)
        {
            HandleMouseLook();
            HandleMovement();    
        }
    }

    public void Freeze()
    {
        active = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    public void OnJump(InputValue value) { if (value.isPressed) jumpPressed = true; }

    private void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        var isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        var h = moveInput.x;
        var v = moveInput.y;

        var move = transform.right * h + transform.forward * v;

        var speed = moveSpeed;
        controller.Move(move * (speed * Time.deltaTime));

        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}