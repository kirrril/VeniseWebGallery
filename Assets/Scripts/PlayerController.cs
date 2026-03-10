using System.Collections;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private Transform cameraTarget;
    public Transform cameraPlace;
    public Transform entryPoint;
    private Vector2 playerMovement;
    private Vector2 mouseDelta;

    void Start()
    {
        transform.position = entryPoint.position;
        transform.rotation = entryPoint.rotation;
    }

    void FixedUpdate()
    {
        HandleWalking();
    }

    private void HandleWalking()
    {
        MovePlayer();
        RotatePlayer();
        MoveCameraTarget();
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void MovePlayer()
    {
        Vector2 movementInput = playerMovement.normalized;
        Vector3 targetVelocity = transform.forward * movementInput.y * 1.5f + transform.right * movementInput.x * 1.5f;
        rb.linearVelocity = targetVelocity;
    }

    private void RotatePlayer()
    {
        float yawDelta = mouseDelta.normalized.x * 1f;
        rb.angularVelocity = new Vector3(0, yawDelta, 0);
    }

    private void MoveCameraTarget()
    {
        float pitchDelta = mouseDelta.normalized.y * 0.5f;
        pitchDelta = Mathf.Clamp(pitchDelta, -1f, 2f);
        float pitch = cameraTarget.localPosition.y + pitchDelta * 2 * Time.fixedDeltaTime;

        cameraTarget.localPosition = new Vector3(0, pitch, 0);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        playerMovement = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
    }
}
