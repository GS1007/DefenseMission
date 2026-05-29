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
    [SerializeField] private AudioClip _defaultSound;
    [SerializeField] private AudioClip _targetDetectSound;

    [Header("Seeker Audio Settings")]

    [SerializeField] private float _baseVolume = 0.1f;
    [SerializeField] private float _maxVolume = 1.0f;

    private AudioClip _currentClip;
    private Strela2MSeeker _seeker;

    private bool _fireSoundPlaying;

    private void OnEnable()
    {
        Strela2MLauncher.MissileLoaded += OnMissileLoad;
        Strela2MLauncher.Fired += OnFire;
        Strela2MBattery.BatteryDied += ResetAudio;
    }

    private void OnDisable()
    {
        Strela2MLauncher.Fired -= OnFire;
        Strela2MLauncher.MissileLoaded -= OnMissileLoad;
        Strela2MBattery.BatteryDied -= ResetAudio;
    }

    private void Update()
    {
        UpdateSeekerTone();
    }

    private void UpdateSeekerTone()
    {
        if (_fireSoundPlaying)
        {
            if (!_seekerSource.isPlaying) _fireSoundPlaying = false;
            return;
        }

        if (_launcher.State != LauncherState.Ready || _launcher.LoadedMissile == null || _seeker == null)
        {
            return;
        }

        if (!_seekerSource.isPlaying) _seekerSource.Play();

        float progress = _seeker.LockProgress;
        bool isLocked = _seeker.HasLock;
        float signal = _seeker.SignalStrength;

        if (signal > 0)
        {
            _seekerSource.volume = Mathf.Lerp(_baseVolume, _maxVolume, Mathf.Clamp01(progress + 0.2f));

            if (isLocked)
            {
                SetAudioClip(_lockSound);
            }
            else
            {
                SetAudioClip(_targetDetectSound);
            }
        }
        else
        {
            _seekerSource.volume = _baseVolume;
            SetAudioClip(_defaultSound);
        }
    }

    private void OnMissileLoad(Strela2MMissile missile)
    {
        _seeker = missile.Seeker;
        _seekerSource.volume = _baseVolume;
    }

    private void OnFire()
    {
        _seeker = null;
        _fireSoundPlaying = true;

        _seekerSource.volume = _maxVolume;
        SetAudioClip(_fireSound);
    }

    private void ResetAudio()
    {
        _seekerSource.Stop();
        _seekerSource.volume = _baseVolume;
        _fireSoundPlaying = false;
        _currentClip = null;
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