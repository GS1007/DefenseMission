using UnityEngine;

public class Strela2MEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem _missleLaunchVFX;

    private void OnEnable()
    {
        Strela2MEvents.Fired += PlayMissleLaunchEffect;
    }

    private void OnDisable()
    {
        Strela2MEvents.Fired -= PlayMissleLaunchEffect;
    }

    private void PlayMissleLaunchEffect()
    {
        PlayVFX(_missleLaunchVFX);
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
