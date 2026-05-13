using UnityEngine;

public class Strela2MAudioController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Strela2MLauncher _launcher;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _seekerSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _fireSound;
    [SerializeField] private AudioClip _lockSound;
    [SerializeField] private AudioClip _scanSound;

    [Header("Seeker Audio Settings")]
    [SerializeField] private float _baseVolume = 0f;
    [SerializeField] private float _maxVolume = 0f;

    private AudioClip _currentClip;

    private Strela2MSeeker _seeker;

    private void OnEnable()
    {
        Strela2MLauncher.MissileLoaded += OnMissileLoad;
        Strela2MLauncher.Fired += OnFire;
        Strela2MBattery.BatteryDied += ResetAudio;
    }

    private void Update()
    {
        UpdateSeekerTone();
    }

    private void OnDisable()
    {
        Strela2MLauncher.Fired -= OnFire;
        Strela2MLauncher.MissileLoaded -= OnMissileLoad;
        Strela2MBattery.BatteryDied -= ResetAudio;
    }

    private void UpdateSeekerTone()
    {
        if (_launcher.State != LauncherState.Ready || _launcher.LoadedMissile == null)
        {
            return;
        }

        if (!_seekerSource.isPlaying) _seekerSource.Play();

        float progress = _seeker.LockProgress;
        bool isLocked = _seeker.HasLock;
        float signal = _seeker.SignalStrength;

        if (signal > 0)
        {
            _seekerSource.volume = Mathf.Lerp(_baseVolume, _maxVolume, progress + 0.2f);

            if (isLocked == true)
            {
                SetAudioClip(_lockSound);
            }
            else
            {
                SetAudioClip(_scanSound);
            }
        }
        else
        {
            _seekerSource.volume = _baseVolume;
        }
    }

    private void OnMissileLoad(Strela2MMissile missile)
    {
        _seeker = missile.Seeker;
    }

    private void OnFire()
    {
        _seekerSource.volume = _maxVolume;

        SetAudioClip(_fireSound);

        Debug.Log("Fired");
    }

    private void ResetAudio()
    {
        _seekerSource.Stop();

        _seekerSource.volume = _baseVolume;
    }

    private void SetAudioClip(AudioClip audioClip)
    {
        if (audioClip == _currentClip)
        {
            return;
        }

        _seekerSource.clip = audioClip;
        _currentClip = audioClip;

        _seekerSource.Play();
    }
}
