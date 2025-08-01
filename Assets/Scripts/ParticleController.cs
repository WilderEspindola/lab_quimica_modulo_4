using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem targetParticleSystem; // Nombre más específico

    public void StartParticles()
    {
        if (targetParticleSystem != null)
        {
            targetParticleSystem.Play();
        }
    }

    public void StopParticles()
    {
        if (targetParticleSystem != null)
        {
            targetParticleSystem.Stop();
        }
    }
}