using UnityEngine;

public class PlayDustBurst : MonoBehaviour
{
    [SerializeField] GameObject dust;
    [SerializeField] GameObject dust2;
    public void Play()
    {
        dust.GetComponent<ParticleSystem>().Play();
        dust2.GetComponent<ParticleSystem>().Play();
    }
}
