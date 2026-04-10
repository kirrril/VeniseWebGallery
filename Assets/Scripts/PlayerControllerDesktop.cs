using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerDesktop : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private Transform cameraTarget;
    public Transform cameraPlace;
    public Transform entryPoint;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lookSensitivity = 0.08f;
    [SerializeField] private float minPitch = 1f;
    [SerializeField] private float maxPitch = 2f;

    private Vector2 playerMovement;
    private Vector2 lookInput;
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
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayerBody();
    }

    private void MovePlayer()
    {
        Vector2 movementInput = playerMovement.normalized;
        Vector3 planarVelocity =
            transform.forward * movementInput.y * moveSpeed +
            transform.right * movementInput.x * moveSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(planarVelocity.x, currentVelocity.y, planarVelocity.z);
    }

    private void UpdateLook()
    {
        yaw += lookInput.x * lookSensitivity;
        pitch += lookInput.y * lookSensitivity / 50;
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

    public void OnMove(InputAction.CallbackContext ctx)
    {
        playerMovement = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }
}
