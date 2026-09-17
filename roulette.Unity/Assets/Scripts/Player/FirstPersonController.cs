using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(CharacterController), typeof(Collider))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -19.62f;

    [Header("Камера")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalClamp = 85f;

    private CharacterController controller;
    private Collider playerCollider;
    private Vector3 velocity;
    private float xRotation;
    private bool active = false;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private GameStateManager gameStateManager;
    private CameraEventHandler cameraEventHandler;

    [Inject]
    public void Construct(GameStateManager gameStateManager, Camera camera, CameraEventHandler cameraEventHandler)
    {
        this.gameStateManager = gameStateManager;
        this.cameraEventHandler = cameraEventHandler;

        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
    }

    private async void Start()
    {
        await Task.Delay(10);

        gameStateManager.ChangeGameState(GameStateManager.GameState.FreeRoam);

        if (cameraTransform == null)
        {
            Debug.LogError("Не назначена ссылка на камеру");
            return;
        }
        if (gameStateManager.CurrentState == GameStateManager.GameState.FreeRoam)
            Enable();
        else
            Disable();

    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.FreeRoam)
            Enable();
        else
            Disable();
    }

    public void Enable()
    {
        active = true;
        playerCollider.enabled = true;
        cameraTransform.gameObject.SetActive(true);
    }

    public void Disable()
    {
        active = false;
        playerCollider.enabled = false;
        cameraTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (active && cameraEventHandler.IsTransitioning == false)
        {
            HandleMouseLook();
            HandleMovement();
        }
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

    private void HandleMouseLook()
    {
        if (!active) return;

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

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation, float cameraPitch)
    {
        if (controller != null)
        {
            controller.Move(Vector3.zero);
            controller.enabled = false;
        }


        Vector3 euler = rotation.eulerAngles;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, euler.y, 0));

        xRotation = Mathf.Clamp(cameraPitch, -verticalClamp, verticalClamp);
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }

        velocity = Vector3.zero;
        moveInput = Vector2.zero;
        lookInput = Vector2.zero;

        Physics.SyncTransforms();
        if (controller != null) controller.enabled = true;
        
    }
}