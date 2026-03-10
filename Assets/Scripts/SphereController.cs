using UnityEngine;

public class SphereController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject canvas;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        animator.SetBool("isBouncing", true);
        canvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        animator.SetBool("isBouncing", false);
        canvas.SetActive(false);
    }
}
