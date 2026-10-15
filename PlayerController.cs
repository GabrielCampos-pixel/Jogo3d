using UnityEngine;

/// Movimento em primeira pessoa (Utilizar como base para VR depois).
/// Exige um CharacterController. "cameraPivot" é um objeto filho que contém a câmera.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform cameraPivot;
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 7f;
    [SerializeField] float jumpHeight = 1.4f;
    [SerializeField] float gravity = -20f;
    [SerializeField] float maxPitch = 80f;

    CharacterController cc;
    IPlayerInput input;
    float yVelocity;
    float pitch;

    void Awake() => cc = GetComponent<CharacterController>();

    void Start()
    {
        GameInput.Ensure();
        input = GameInput.Instance;
    }

    void Update()
    {
        if (PauseMenuController.IsPaused) return;

        // Olhar
        Vector2 look = input.Look;
        transform.Rotate(0f, look.x, 0f);
        pitch = Mathf.Clamp(pitch - look.y, -maxPitch, maxPitch);
        if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Andar
        Vector2 m = input.Move;
        Vector3 dir = Vector3.ClampMagnitude(transform.right * m.x + transform.forward * m.y, 1f);
        float speed = input.SprintHeld ? sprintSpeed : walkSpeed;

        // Pular / gravidade
        if (cc.isGrounded && yVelocity < 0f) yVelocity = -2f;
        if (cc.isGrounded && input.JumpPressed) yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        yVelocity += gravity * Time.deltaTime;

        cc.Move((dir * speed + Vector3.up * yVelocity) * Time.deltaTime);
    }
}
