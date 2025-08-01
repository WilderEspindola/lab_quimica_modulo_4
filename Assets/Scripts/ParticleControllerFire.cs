using UnityEngine;

public class ParticleControllerFire : MonoBehaviour
{
    public ParticleSystem targetParticleSystemFire; // Nombre más específico

    public void StartParticles()
    {
        if (targetParticleSystemFire != null)
        {
            targetParticleSystemFire.Play();
        }
    }

    public void StopParticles()
    {
        if (targetParticleSystemFire != null)
        {
            targetParticleSystemFire.Stop();
        }
    }
}
