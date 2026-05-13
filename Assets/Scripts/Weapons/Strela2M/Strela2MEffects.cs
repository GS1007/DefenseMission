using UnityEngine;

public class Strela2MEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem _missleEjectionVFX;

    private void PlayMissleLaunchEffect()
    {
        PlayVFX(_missleEjectionVFX);
    }

    private void PlayVFX(ParticleSystem vfx)
    {
        if (vfx == null)
        {
            return;
        }

        vfx.Play();
    }
}
