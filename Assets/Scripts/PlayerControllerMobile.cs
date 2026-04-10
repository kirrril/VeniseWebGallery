using UnityEngine;

public class PlayerControllerMobile : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform entryPoint;
    [SerializeField] private MoveGizmo moveGizmo;
    [SerializeField] private TurnGizmo turnGizmo;

    private float moveSpeed = 2f;
    private float lookSensitivity = 0.8f;
    private float minPitch = 1f;
    private float maxPitch = 2f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        transform.position = entryPoint.position;
        transform.rotation = entryPoint.rotation;

        yaw = transform.eulerAngles.y;
        pitch = cameraTarget.localPosition.y;

        ApplyPitch();
    }

    private void Update()
    {
        UpdateLook();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayerBody();
    }

    private void MovePlayer()
    {
        Vector2 movementInput = new Vector2(moveGizmo.speedX, moveGizmo.speedY).normalized;
        Vector3 playerVelocity =
            transform.forward * movementInput.y * moveSpeed +
            transform.right * movementInput.x * moveSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(playerVelocity.x, currentVelocity.y, playerVelocity.z);
    }

    private void UpdateLook()
    {
        yaw += turnGizmo.speedX * lookSensitivity * 3;
        pitch += turnGizmo.speedY * lookSensitivity / 40;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        ApplyPitch();
    }

    private void RotatePlayerBody()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    private void ApplyPitch()
    {
        if (cameraTarget == null)
        {
            return;
        }

        cameraTarget.localPosition = new Vector3(cameraTarget.localPosition.x, pitch, cameraTarget.localPosition.z);
    }
}
