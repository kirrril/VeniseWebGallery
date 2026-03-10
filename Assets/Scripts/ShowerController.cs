using UnityEngine;

public class ShowerController : MonoBehaviour
{
    [SerializeField] private ParticleSystem drops;
    [SerializeField] private ParticleSystem bursts;
    [SerializeField] private ParticleSystem drying;
    [SerializeField] private GameObject canvas;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        drops.Play();
        bursts.Play();
        drying.Play();
        canvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        drops.Stop();
        bursts.Stop();
        drying.Stop();
        canvas.SetActive(false);
    }
}