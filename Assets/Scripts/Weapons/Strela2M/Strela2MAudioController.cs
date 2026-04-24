using UnityEngine;

public class Strela2MAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _trackingSound;
    [SerializeField] private AudioClip _lockSound;
    [SerializeField] private AudioClip _fireSound;

    [SerializeField] private Strela2MBattery _battery;

    private void OnEnable()
    {
        _battery.BatteryEnabled += PlayTrackingSound;

        Strela2MEvents.BatteryDied += StopAudio;
        Strela2MEvents.TrackingStopped += StopAudio;
        Strela2MEvents.TargetLocked += PlayLockSound;
        Strela2MEvents.LockReseted += PlayTrackingSound;
        Strela2MEvents.Fired += PlayFireSound;
    }

    private void OnDisable()
    {
        Strela2MEvents.TrackingStopped -= StopAudio;
        Strela2MEvents.LockReseted -= PlayTrackingSound;
        Strela2MEvents.TargetLocked -= PlayLockSound;
        Strela2MEvents.Fired -= PlayFireSound;
        Strela2MEvents.BatteryDied -= StopAudio;

        _battery.BatteryEnabled -= PlayTrackingSound;
    }

    private void PlayTrackingSound()
    {
        PlayAudio(_trackingSound, true);
    }

    private void PlayLockSound()
    {
        PlayAudio(_lockSound, false);
    }

    private void PlayFireSound()
    {
        PlayAudio(_fireSound, false);
    }

    private void PlayAudio(AudioClip clip, bool loop)
    {
        _audioSource.clip = clip;
        _audioSource.loop = loop;

        _audioSource.Play();
    }

    private void StopAudio()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }
}
