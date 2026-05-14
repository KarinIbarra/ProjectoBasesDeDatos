using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform cameraTransform;
    private CharacterController controller;

    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float gravity = -9.81f;
    public float turnSpeed = 10f;
    public bool canMove = true;

    [Header("Stamina (0-100)")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 20f;   // cuánto recupera por segundo
    public float staminaDrainRate = 25f;   // cuánto gasta por segundo
    private float currentStamina;

    [Header("UI")]
    public TextMeshProUGUI staminaText;

    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        UpdateStaminaUI();
    }

    void Update()
    {
        if (!canMove) return;

        Move();
        HandleStamina();
        UpdateStaminaUI();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * z + right * x;

        bool isMoving = moveDirection.magnitude > 0.1f;

        bool isRunning = Input.GetKey(KeyCode.LeftShift)
                         && currentStamina > 0f
                         && isMoving;

        float speed = isRunning ? runSpeed : walkSpeed;

        controller.Move(moveDirection * speed * Time.deltaTime);

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 🔻 Consumir stamina SOLO al correr
        if (isRunning)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
    }

    void HandleStamina()
    {
        bool isMoving = controller.velocity.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        // 🔺 Regenerar SOLO si no corre
        if (!isRunning)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void UpdateStaminaUI()
    {
        if (staminaText != null)
        {
            staminaText.text = " " + Mathf.RoundToInt(currentStamina);
        }
    }
}